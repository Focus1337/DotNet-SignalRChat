using System.ComponentModel.DataAnnotations;

namespace Back.Web.Dto.Message;

public class MessageDto
{
    [Required]
    public string Text { get; set; } = null!;
}