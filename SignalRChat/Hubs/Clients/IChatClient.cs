namespace SignalRChat.Hubs.Clients;

public interface IChatClient
{
    public Task ReceiveMessage(string name, string text);
}