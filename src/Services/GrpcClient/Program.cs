using Grpc.Core;
using Grpc.Net.Client;
using GrpcClient;

using var channel = GrpcChannel.ForAddress("http://localhost:5029");
var client = new Greeter.GreeterClient(channel);

// Unary operation
// var reply = await client.SayHelloAsync(new HelloRequest { Name = "Focus", Age = 20 });

// Server side stream
// var reply = client.StreamingFromServer(new HelloRequest { Name = "Focus", Age = 20 });
// await foreach (var response in reply.ResponseStream.ReadAllAsync()) 
//     Console.WriteLine($"Result: {response.Message}");

// Client side stream
// using var call = client.StreamingFromClient();
// for (var i = 0; i < 5; i++) 
//     await call.RequestStream.WriteAsync(new HelloRequest { Name = "Name", Age = i });
// await call.RequestStream.CompleteAsync();
// Console.WriteLine($"Result: {(await call.ResponseAsync).Message}");

// Both way streaming
// using var call = client.StreamingBothWays();
// for (var i = 0; i < 5; i++)
//     await call.RequestStream.WriteAsync(new HelloRequest { Name = "Alex", Age = i });
// await call.RequestStream.CompleteAsync();
//
// await foreach (var response in call.ResponseStream.ReadAllAsync())
//     Console.WriteLine($"Server said: {response.Message}");

Console.WriteLine("Press any key to exit...");
Console.ReadKey();