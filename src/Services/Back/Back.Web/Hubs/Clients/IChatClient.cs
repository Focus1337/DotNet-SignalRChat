using Back.Web.Models;

namespace Back.Web.Hubs.Clients;

public interface IChatClient
{
    public Task ReceiveMessage(MessageResponse response);
    public Task<string> GetMessage();
    public Task GetCurrentTime(string currentTime);
}