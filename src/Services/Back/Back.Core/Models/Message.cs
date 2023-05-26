namespace Back.Core.Models;

public class Message
{
    public Guid Id { get; } = Guid.NewGuid();
    public required string Text { get; set; }
    public required DateTime SentTime { get; set; }

    private User? _user;

    public User User
    {
        get => _user ?? throw new InvalidOperationException("Not initialized");
        set => _user = value;
    }
}