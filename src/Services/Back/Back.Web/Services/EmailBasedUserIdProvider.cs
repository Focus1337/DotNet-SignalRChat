using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using OpenIddict.Abstractions;

namespace Back.Web.Services;

public class EmailBasedUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) =>
        connection.User.FindFirstValue(OpenIddictConstants.Claims.Email);
}