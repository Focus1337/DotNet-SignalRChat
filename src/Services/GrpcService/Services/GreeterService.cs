using System.Text;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcService;

namespace GrpcService.Services;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
    {
        var description = string.Join(" & ", request.Attributes.Select(pair => $"{pair.Key}: {pair.Value}"));
        return Task.FromResult(new HelloResponse
        {
            Message = $"Hello, {request.Name}. Your age: {request.Age}.",
            SentDate = Timestamp.FromDateTime(DateTime.UtcNow),
            Description = description,
            Synonyms = { "Privet", "Bonjour", "Salam" }
        });
    }

    public override async Task StreamingFromServer(HelloRequest request,
        IServerStreamWriter<HelloResponse> responseStream,
        ServerCallContext context)
    {
        for (var i = 0; i < 5; i++)
        {
            await responseStream.WriteAsync(new HelloResponse
            {
                Message = i.ToString(), SentDate = Timestamp.FromDateTime(DateTime.UtcNow),
                Description = i % 2 == 0 ? "It's even!" : null
            });
            await Task.Delay(1000);
        }
    }

    public override async Task<HelloResponse> StreamingFromClient(IAsyncStreamReader<HelloRequest> requestStream,
        ServerCallContext context)
    {
        var nameBuilder = new StringBuilder();
        var ageBuilder = new StringBuilder();
        await foreach (var message in requestStream.ReadAllAsync())
        {
            nameBuilder.AppendJoin(" ", message.Name);
            ageBuilder.AppendJoin(" ", message.Age);
        }

        return new HelloResponse
            { Message = $"{nameBuilder}. {ageBuilder}.", SentDate = Timestamp.FromDateTime(DateTime.UtcNow) };
    }

    public override async Task StreamingBothWays(IAsyncStreamReader<HelloRequest> requestStream,
        IServerStreamWriter<HelloResponse> responseStream, ServerCallContext context)
    {
        await foreach (var message in requestStream.ReadAllAsync())
        {
            await responseStream.WriteAsync(new HelloResponse
                { Message = $"{message.Name}, you're a dead man", SentDate = Timestamp.FromDateTime(DateTime.UtcNow) });
            await responseStream.WriteAsync(new HelloResponse
            {
                Message = $"And you owe me {message.Age} honeybunz", SentDate = Timestamp.FromDateTime(DateTime.UtcNow)
            });
        }
    }
}