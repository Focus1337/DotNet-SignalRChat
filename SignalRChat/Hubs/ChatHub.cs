using Microsoft.AspNetCore.SignalR;
using SignalRChat.Hubs.Clients;

namespace SignalRChat.Hubs;

public class ChatHub : Hub<IChatClient>
{
    public async Task SendMessage(string name, string text)
    {
        await Clients.All.ReceiveMessage(name, text);
    }
}