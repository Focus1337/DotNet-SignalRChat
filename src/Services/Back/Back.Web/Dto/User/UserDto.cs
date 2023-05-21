using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Back.Web.Dto.User;

public class UserDto
{
    [Required] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
}