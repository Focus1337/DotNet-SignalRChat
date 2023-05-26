using System.ComponentModel.DataAnnotations;

namespace Back.Web.Dto.Message;

public class DisplayMessageDto
{
    [Required]
    public string Text { get; set; } = null!;
    [Required]
    public DateTime SentTime { get; set; }
    [Required]
    public string Username { get; set; } = null!;
}