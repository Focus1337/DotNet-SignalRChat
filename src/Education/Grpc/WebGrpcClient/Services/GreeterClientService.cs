using Grpc.Core;
using Grpc.Net.ClientFactory;

namespace WebGrpcClient.Services;

public class GreeterClientService
{
    private readonly Greeter.GreeterClient _client;

    public GreeterClientService(GrpcClientFactory clientFactory)
    {
        _client = clientFactory.CreateClient<Greeter.GreeterClient>("WebClient");
    }

    public async Task Oneof()
    {
        var response = await _client.ResponseWithOneofAsync(new HelloRequest { Name = "Focus", Age = 20 });
        if (response.ResultCase == ResponseMessage.ResultOneofCase.Error)
        {
            Console.WriteLine(response.Error.Name);
            return;
        }

        Console.WriteLine($"{response.Person.Name}: {response.Person.Age}");
    }

    public async Task UnaryOperation()
    {
        var attributes = new Dictionary<string, string> { { "Name", "Unary Operation" }, { "Type", "Task" } };
        var request = new HelloRequest { Name = "Focus", Age = 20 };
        request.Attributes.Add(attributes);

        var reply = await _client.SayHelloAsync(request,
            deadline: DateTime.UtcNow.AddSeconds(10));
        Console.WriteLine($"{reply.Message} | {reply.Description} | {reply.SentDate}");
        foreach (var synonym in reply.Synonyms)
            Console.WriteLine(synonym);
    }

    public async Task ServerStreaming()
    {
        var reply = _client.StreamingFromServer(new HelloRequest { Name = "Focus", Age = 20 });
        await foreach (var response in reply.ResponseStream.ReadAllAsync())
            Console.WriteLine(
                $"Result: {response.Message} | {response.SentDate} | Even? {response.Description ?? "No info"}");
    }

    public async Task ClientStreaming()
    {
        using var call = _client.StreamingFromClient();
        for (var i = 0; i < 5; i++)
            await call.RequestStream.WriteAsync(new HelloRequest { Name = "Name", Age = i });
        await call.RequestStream.CompleteAsync();
        Console.WriteLine($"Result: {(await call.ResponseAsync).Message}");
    }

    public async Task BothWaysStreaming()
    {
        using var call = _client.StreamingBothWays();
        for (var i = 0; i < 5; i++)
            await call.RequestStream.WriteAsync(new HelloRequest { Name = "Alex", Age = i });
        await call.RequestStream.CompleteAsync();

        await foreach (var response in call.ResponseStream.ReadAllAsync())
            Console.WriteLine($"Server said: {response.Message}");
    }
}