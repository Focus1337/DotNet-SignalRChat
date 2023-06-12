using Grpc.Core;
using Grpc.Net.Client.Configuration;
using WebGrpcClient;
using WebGrpcClient.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGrpcClient<Greeter.GreeterClient>("WebClient",
        opts => opts.Address = new Uri("http://localhost:5029"))
    .ConfigureChannel(opts => opts.ServiceConfig = new ServiceConfig
    {
        MethodConfigs =
        {
            new MethodConfig
            {
                Names = { MethodName.Default },
                RetryPolicy = new RetryPolicy
                {
                    MaxAttempts = 5,
                    InitialBackoff = TimeSpan.FromSeconds(1),
                    MaxBackoff = TimeSpan.FromSeconds(5),
                    BackoffMultiplier = 1.5,
                    RetryableStatusCodes = { StatusCode.Unavailable }
                }
            }
        }
    })
    .EnableCallContextPropagation();

builder.Services.AddTransient<GreeterClientService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Web Grpc Client - Minimal API");
app.MapGet("/oneof", async (GreeterClientService client) => { await client.Oneof(); })
    .WithName("Oneof")
    .WithOpenApi();

app.MapGet("/unary-operation", async (GreeterClientService client) =>
{
    try
    {
        await client.UnaryOperation();
    }
    catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
    {
        Console.WriteLine("Deadline");
    }
});
app.MapGet("/client-streaming", async (GreeterClientService client) => { await client.ClientStreaming(); });
app.MapGet("/server-streaming", async (GreeterClientService client) => { await client.ServerStreaming(); });
app.MapGet("/both-ways-streaming", async (GreeterClientService client) => { await client.BothWaysStreaming(); });

app.Run();