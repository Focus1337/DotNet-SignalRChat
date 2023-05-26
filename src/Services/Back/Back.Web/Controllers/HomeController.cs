using Back.Core.Interfaces;
using Back.Core.Models;
using Back.Web.Filters;
using Back.Web.Hubs;
using Back.Web.Hubs.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Back.Web.Controllers;

[OpenIddictAuthorize]
[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly IHubContext<ChatHub, IChatClient> _hubContext;
    private readonly IUserService<User> _userService;

    public HomeController(IHubContext<ChatHub, IChatClient> hubContext, IUserService<User> userService)
    {
        _hubContext = hubContext;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (await _userService.GetCurrentUser() is not { } user)
            return NotFound();

        if (user.Email is null)
            return BadRequest();

        var currentTime = DateTime.Now.ToString("f");
        await _hubContext.Clients.User(user.Email).GetCurrentTime(currentTime);
        return Ok();
    }
}