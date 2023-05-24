using Microsoft.AspNetCore.SignalR;

namespace Back.Web.Hubs.Filters;

public class CustomFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object?>> next)
    {
        return await next(invocationContext);
    }

    // Optional method
    public Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
    {
        Console.WriteLine($"{context.Context.ConnectionId} is connected.");
        return next(context);
    }

    // Optional method
    public Task OnDisconnectedAsync(
        HubLifetimeContext context, Exception? exception, Func<HubLifetimeContext, Exception, Task> next)
    {
        Console.WriteLine($"{context.Context.ConnectionId} is disconnected.");
        return next(context, exception!);
    }
}