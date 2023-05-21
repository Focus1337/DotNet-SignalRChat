using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Back.Web.Dto.User;

public class LoginUserDto
{
    [Required, JsonPropertyName("username")]
    public string UserName { get; set; } = null!;

    [Required, JsonPropertyName("password")]
    public string Password { get; set; } = null!;

    [JsonPropertyName("grant_type"), DefaultValue("password")]
    public string GrantType { get; set; } = "password";
}