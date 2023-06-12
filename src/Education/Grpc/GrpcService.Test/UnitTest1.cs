using System.Globalization;
using GrpcService.Services;
using GrpcService.Test.Helpers;
using Moq;

namespace GrpcService.Test;

public class UnitTest1
{
    [Fact]
    public async Task SayHelloTest()
    {
        // Arrange
        var mockGreeter = new Mock<IGreeter>();
        mockGreeter.Setup(
            m => m.Greet(It.IsAny<string>())).Returns((string s) => $"Super{s}");
        var service = new GreeterService(mockGreeter.Object);

        // Act
        var response = await service.SayHello(
            new HelloRequest
            {
                Name = "Joe", Age = 18,
                Attributes =
                {
                    { "language", "english" },
                    { "time", Convert.ToString(DateTime.Now, CultureInfo.InvariantCulture) }
                }
            }, TestServerCallContext.Create());

        // Assert
        mockGreeter.Verify(v => v.Greet("Joe"));
        Assert.Equal("Hello, SuperJoe. Your age: 18.", response.Message);
    }
}