using Google.Protobuf.Collections;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcClient;

using var channel = GrpcChannel.ForAddress("http://localhost:5029");
var client = new Greeter.GreeterClient(channel);

// await UnaryOperation(client);
await Oneof(client);
Console.WriteLine("Press any key to exit...");
Console.ReadKey();

static async Task Oneof(Greeter.GreeterClient client)
{
    var response = await client.ResponseWithOneofAsync(new HelloRequest { Name = "Focus", Age = 20 });
    if (response.ResultCase == ResponseMessage.ResultOneofCase.Error)
    {
        Console.WriteLine(response.Error.Name);
        return;
    }

    Console.WriteLine($"{response.Person.Name}: {response.Person.Age}");
}

static async Task UnaryOperation(Greeter.GreeterClient client)
{
    var attributes = new Dictionary<string, string> { { "Name", "Unary Operation" }, { "Type", "Task" } };
    var request = new HelloRequest { Name = "Focus", Age = 20 };
    request.Attributes.Add(attributes);

    var reply = await client.SayHelloAsync(request);
    Console.WriteLine($"{reply.Message} | {reply.Description} | {reply.SentDate}");
    foreach (var synonym in reply.Synonyms)
        Console.WriteLine(synonym);
}

static async Task ServerStreaming(Greeter.GreeterClient client)
{
    var reply = client.StreamingFromServer(new HelloRequest { Name = "Focus", Age = 20 });
    await foreach (var response in reply.ResponseStream.ReadAllAsync())
        Console.WriteLine(
            $"Result: {response.Message} | {response.SentDate} | Even? {response.Description ?? "No info"}");
}

static async Task ClientStreaming(Greeter.GreeterClient client)
{
    using var call = client.StreamingFromClient();
    for (var i = 0; i < 5; i++)
        await call.RequestStream.WriteAsync(new HelloRequest { Name = "Name", Age = i });
    await call.RequestStream.CompleteAsync();
    Console.WriteLine($"Result: {(await call.ResponseAsync).Message}");
}

static async Task BothWaysStreaming(Greeter.GreeterClient client)
{
    using var call = client.StreamingBothWays();
    for (var i = 0; i < 5; i++)
        await call.RequestStream.WriteAsync(new HelloRequest { Name = "Alex", Age = i });
    await call.RequestStream.CompleteAsync();

    await foreach (var response in call.ResponseStream.ReadAllAsync())
        Console.WriteLine($"Server said: {response.Message}");
}