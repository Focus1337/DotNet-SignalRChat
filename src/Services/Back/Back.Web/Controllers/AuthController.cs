using System.Net.Mime;
using System.Security.Claims;
using Back.Application.Interfaces;
using Back.Application.Models;
using Back.Web.Dto.User;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Back.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IUserService<User> _userService;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager,
        IUserService<User> userService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userService = userService;
    }

    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
    {
        if (!ModelState.IsValid) return BadRequest();

        if (await _userService.ValidateCredentials(registerUserDto.Email, checkPassword: false))
            return BadRequest("User with same Email already exists");

        var user = new User(registerUserDto.Email, registerUserDto.Email);

        var result = await _userManager.CreateAsync(user, registerUserDto.Password);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status403Forbidden,
                JsonConvert.SerializeObject(result.Errors.Select(e => e.Description).ToList()));

        return StatusCode(StatusCodes.Status201Created, user.Id);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [Consumes("application/x-www-form-urlencoded")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(OpenIddictResponse))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OpenIddictResponse))]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginUserDto userDto)
    {
        if (!ModelState.IsValid) return BadRequest();
        var request = HttpContext.GetOpenIddictServerRequest() ?? throw new Exception("OpenIdDict config is wrong");
        if (!await _userService.ValidateCredentials(userDto.UserName, userDto.Password, true))
            return Forbid(new AuthenticationProperties(new Dictionary<string, string?>
            {
                [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                    "Incorrect email or password"
            }), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);


        var userPrincipal =
            await _signInManager.CreateUserPrincipalAsync((await _userService.FindByEmail(userDto.UserName))!);
        userPrincipal.SetScopes(new[]
        {
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles
        }.Intersect(request.GetScopes()));
        foreach (var claim in userPrincipal.Claims)
            claim.SetDestinations(GetDestinations(claim, userPrincipal));
        return SignIn(userPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
        {
            switch (claim.Type)
            {
                case OpenIddictConstants.Claims.Name:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Profile))
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;
                case OpenIddictConstants.Claims.Email:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Email))
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;
                case OpenIddictConstants.Claims.Role:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    if (principal.HasScope(OpenIddictConstants.Permissions.Scopes.Roles))
                        yield return OpenIddictConstants.Destinations.IdentityToken;
                    yield break;
                case "AspNet.Identity.SecurityStamp":
                    yield break;
                default:
                    yield return OpenIddictConstants.Destinations.AccessToken;
                    yield break;
            }
        }
    }
}