using System.Net.Mime;
using Back.Core.Interfaces;
using Back.Core.Models;
using Back.Web.Dto.Message;
using Back.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;

namespace Back.Web.Controllers;

[ApiController]
[Route("[controller]"), OpenIddictAuthorize]
public class MessagesController : CustomControllerBase
{
    private readonly IMessageService _messageService;
    private readonly IUserService<User> _userService;

    public MessagesController(IMessageService messageService, IUserService<User> userService)
    {
        _messageService = messageService;
        _userService = userService;
    }

    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(OpenIddictResponse))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DisplayMessageDto>))]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (await _userService.GetCurrentUser() is null)
            return BadRequestDueToToken();

        var result = (from message in await _messageService.GetMessages()
            select new DisplayMessageDto
                { Text = message.Text, Username = message.User.Email!, SentTime = message.SentTime }).ToList();

        return Ok(result);
    }

    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(OpenIddictResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DisplayMessageDto))]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        if (await _userService.GetCurrentUser() is null)
            return BadRequestDueToToken();

        if (await _messageService.FindById(id) is not { } message)
            return NotFound();

        return Ok(new DisplayMessageDto
            { Text = message.Text, SentTime = message.SentTime, Username = message.User.Email! });
    }

    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ModelStateDictionary))]
    // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<Error>))]
    // [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(OpenIddictResponse))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] MessageDto messageDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // ModelState.Values.First(x => x.)

        if (await _userService.GetCurrentUser() is not { } user)
            return BadRequestDueToToken();

        var message = new Message
        {
            Text = messageDto.Text,
            SentTime = DateTime.Now,
            User = user
        };

        var result = await _messageService.CreateMessage(message);
        if (!result.IsSuccess)
            return BadRequest(result.Errors);
        return CreatedAtAction(nameof(Get), new
        {
            id = result.Value.Id
        }, result.Value.Id);
    }

    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(OpenIddictResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] MessageDto messageDto)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        if (await _userService.GetCurrentUser() is null)
            return BadRequestDueToToken();

        if (await _messageService.FindById(id) is not { } message)
            return NotFound();

        message.Text = messageDto.Text;
        var result = await _messageService.UpdateMessage(message);
        if (!result.IsSuccess)
            return BadRequest(result.Errors.First().Message);

        return NoContent();
    }

    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(OpenIddictResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        if (await _userService.GetCurrentUser() is null)
            return BadRequestDueToToken();

        if (await _messageService.FindById(id) is not { } message)
            return NotFound();

        await _messageService.DeleteMessage(message);

        return NoContent();
    }
}