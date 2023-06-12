namespace GrpcService.Services;

public class Greeter : IGreeter
{
    private readonly ILogger<Greeter> _logger;

    public Greeter(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<Greeter>();
    }

    public string Greet(string name)
    {
        _logger.LogInformation("Creating greeting to {Name}", name);
        return $"Super{name}";
    }
}