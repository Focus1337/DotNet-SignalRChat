using Back.Hubs;
using Back.Hubs.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Back.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    private readonly IHubContext<ChatHub, IChatClient> _hubContext;

    public HomeController(IHubContext<ChatHub, IChatClient> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpGet("{connectionId}")]
    public async Task<string> Get(string connectionId)
    {
        var currentTime = DateTime.Now.ToString("f");
        await _hubContext.Clients.Client(connectionId).GetCurrentTime(currentTime);
        return currentTime;
    }
}