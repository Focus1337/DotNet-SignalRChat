using Microsoft.AspNetCore.Identity;

namespace Back.Application.Models;

public class User : IdentityUser<Guid>
{
#pragma warning disable CS8765
    public sealed override string Email { get; set; }
    public sealed override string UserName { get; set; }
#pragma warning restore CS8765


    // ReSharper disable once UnusedMember.Local
#pragma warning disable CS8618
    private User()
#pragma warning restore CS8618
    {
    }

    public User(string email, string userName)
    {
        Email = email;
        UserName = userName;
    }
}