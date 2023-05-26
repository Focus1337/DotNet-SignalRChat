using System.Net.Mime;
using System.Security.Claims;
using Back.Core.Models;
using Back.Web.Dto.User;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Back.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(List<IdentityError>))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto registerUserDto)
    {
        if (!ModelState.IsValid) return BadRequest();

        var user = new User { Email = registerUserDto.Email, UserName = registerUserDto.Email };

        var result = await _userManager.CreateAsync(user, registerUserDto.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        result = await _userManager.AddClaimsAsync(user,
            new[]
            {
                new Claim(ClaimTypes.Email, registerUserDto.Email),
                new Claim(ClaimTypes.Name, registerUserDto.Email)
            });
        if (!result.Succeeded)
            return BadRequest(result.Errors);

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
    [ProducesResponseType(StatusCodes.Status403Forbidden, Type = typeof(OpenIddictResponse))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OpenIddictResponse))]
    [SwaggerOperation(
        Summary = "Access Token generation operation",
        Description = "Content-Type: application/x-www-form-urlencoded | Body: username, password, grant_type",
        OperationId = "Exchange"
    )]
    [HttpPost("~/connect/token")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest() ?? throw new Exception("OpenIdDict config is wrong");
        User? user;

        if (request.IsPasswordGrantType())
        {
            if (request.Username is null || request.Password is null)
                return BadRequest(new OpenIddictResponse
                {
                    Error = OpenIddictConstants.Errors.InvalidRequest,
                    ErrorDescription = "Username or password is null."
                });

            user = await _userManager.FindByNameAsync(request.Username);
            if (user is null)
                return BadRequest(new OpenIddictResponse
                {
                    Error = OpenIddictConstants.Errors.InvalidGrant,
                    ErrorDescription = "The username/password couple is invalid."
                });

            if (!await _signInManager.CanSignInAsync(user))
                return BadRequest(new OpenIddictResponse
                {
                    Error = OpenIddictConstants.Errors.InvalidGrant,
                    ErrorDescription = "The specified user is not allowed to sign in."
                });

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
                return BadRequest(new OpenIddictResponse
                {
                    Error = OpenIddictConstants.Errors.InvalidGrant,
                    ErrorDescription = "The username/password couple is invalid."
                });
        }
        else if (request.IsRefreshTokenGrantType())
        {
            var authResult =
                await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            user = await _userManager.GetUserAsync(authResult.Principal ??
                                                   throw new InvalidOperationException("No auth result was provided"));

            if (user is null)
                return Forbid(new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The refresh token is no longer valid."
                }!), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            if (!await _signInManager.CanSignInAsync(user))
                return Forbid(new AuthenticationProperties(new Dictionary<string, string>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = OpenIddictConstants.Errors.InvalidGrant,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The user is no longer allowed to sign in."
                }!), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        else
            return BadRequest(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.UnsupportedGrantType,
                ErrorDescription = "The specified grant type is not supported."
            });

        // Create a new authentication ticket.
        var ticket = await CreateTicketAsync(request, user);

        if (request.IsRefreshTokenGrantType())
            return SignIn(ticket.Principal, ticket.AuthenticationScheme);

        return SignIn(ticket.Principal, ticket.Properties, ticket.AuthenticationScheme);
    }

    private async Task<AuthenticationTicket> CreateTicketAsync(OpenIddictRequest request, User user)
    {
        // Create a new ClaimsPrincipal containing the claims that
        // will be used to create an id_token, a token or a code.
        var principal = await _signInManager.CreateUserPrincipalAsync(user);

        if (!request.IsRefreshTokenGrantType())
        {
            // Set the list of scopes granted to the client application.
            // Note: the offline_access scope must be granted
            // to allow OpenIddict to return a refresh token.
            principal.SetScopes(new[]
            {
                OpenIddictConstants.Scopes.Email,
                OpenIddictConstants.Scopes.Profile,
                OpenIddictConstants.Scopes.Roles,
                OpenIddictConstants.Scopes.OfflineAccess
            }.Intersect(request.GetScopes()));
        }

        principal.SetResources("resource_server");

        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.
        foreach (var claim in principal.Claims)
        {
            var destinations = new List<string>
            {
                OpenIddictConstants.Destinations.AccessToken
            };

            // Only add the iterated claim to the id_token if the corresponding scope was granted to the client application.
            // The other claims will only be added to the access_token, which is encrypted when using the default format.
            if ((claim.Type == OpenIddictConstants.Claims.Name &&
                 principal.HasScope(OpenIddictConstants.Scopes.Profile)) ||
                (claim.Type == OpenIddictConstants.Scopes.Email &&
                 principal.HasScope(OpenIddictConstants.Scopes.Email)) ||
                (claim.Type == OpenIddictConstants.Claims.Role &&
                 principal.HasScope(OpenIddictConstants.Scopes.Roles)))
                destinations.Add(OpenIddictConstants.Destinations.IdentityToken);

            claim.SetDestinations(destinations);
        }

        // Create a new authentication ticket holding the user identity.
        if (request.IsRefreshTokenGrantType())
            return new AuthenticationTicket(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        return new AuthenticationTicket(principal, new AuthenticationProperties
            {
                IssuedUtc = DateTimeOffset.UtcNow,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30),
                AllowRefresh = false
            },
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}