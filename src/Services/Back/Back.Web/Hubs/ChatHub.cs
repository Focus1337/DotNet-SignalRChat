using System.Runtime.CompilerServices;
using System.Security.Claims;
using Back.Core.Models;
using Back.Web.Filters;
using Back.Web.Hubs.Clients;
using Back.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using OpenIddict.Abstractions;

namespace Back.Web.Hubs;

[OpenIddictAuthorize]
public class ChatHub : Hub<IChatClient>
{
    private readonly UserManager<User> _userManager;

    public ChatHub(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<string> SendMessage(MessageRequest request)
    {
        var name = Context.User?.FindFirstValue(OpenIddictConstants.Claims.Email);
        Console.WriteLine(name);
        if (name is null)
            return "Error";

        if (await _userManager.FindByNameAsync(name) is not { } user)
            return "Error";

        if (user.Email is null)
            return "Error";

        await Clients.All.ReceiveMessage(new MessageResponse
            { Name = user.Email, Text = request.Text, SentTime = DateTime.Now });

        return "Sent";
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

    public async IAsyncEnumerable<int> Counter(int count, int delay,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        for (var i = 0; i < count; i++)
        {
            // Check the cancellation token regularly so that the server will stop
            // producing items if the client disconnects.
            cancellationToken.ThrowIfCancellationRequested();

            yield return i;

            // Use the cancellationToken in other APIs that accept cancellation
            // tokens so the cancellation can flow down to them.
            await Task.Delay(delay, cancellationToken);
        }
    }
}