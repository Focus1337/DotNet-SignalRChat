using System.Security.Claims;
using Back.Core.Interfaces;
using Back.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

namespace Back.Web.Services;

public class UserService : IUserService<User>
{
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<User>> GetUsers() =>
        await _userManager.Users.ToListAsync();

    public async Task<User?> GetCurrentUser()
    {
        var email = _httpContextAccessor.HttpContext?.User.FindFirstValue(OpenIddictConstants.Claims.Email);
        return email is not null ? await _userManager.FindByEmailAsync(email) : null;
    }
}