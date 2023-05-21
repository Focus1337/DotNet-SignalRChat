namespace Back.Hubs.Clients;

public interface IChatClient
{
    public Task ReceiveMessage(MessageRequest req);
    public Task<string> GetMessage();
    public Task GetCurrentTime(string currentTime);
}