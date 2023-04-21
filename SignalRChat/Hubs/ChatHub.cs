using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs.Clients;

namespace SignalRChat.Hubs;

public class TotalLengthRequest
{
    public string Param1 { get; set; } = null!;
}

public class MessageRequest
{
    public string Name { get; set; } = null!;
    public string Text { get; set; } = null!;
    public string ConnectionId { get; set; } = null!;
}

public class ChatHub : Hub<IChatClient>
{
    public async Task SendMessage(string name, string text, string connectionId)
    {
        await Clients.All.ReceiveMessage(new MessageRequest { Name = name, Text = text, ConnectionId = connectionId });
    }

    public async Task<string> WaitForMessage(string connectionId)
    {
        var message = await Clients.Client(connectionId).GetMessage();
        return message;
    }

    public Task<int> GetTotalLength(TotalLengthRequest req)
    {
        return Task.FromResult(req.Param1.Length);
    }
}