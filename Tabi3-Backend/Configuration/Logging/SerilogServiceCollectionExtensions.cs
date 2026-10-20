using Serilog;

namespace Tabi3_Backend.Configuration.Logging;

public static class SerilogServiceCollectionExtensions
{
    public static IServiceCollection AddTabi3Serilog(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var serilogConfiguration = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Serilog:WriteTo:1:Args:path"] = Path.Combine(
                    environment.ContentRootPath,
                    "logs",
                    "logs-.json")
            })
            .Build();

        services.AddSerilog((registeredServices, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(serilogConfiguration)
            .ReadFrom.Services(registeredServices)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Environment", environment.EnvironmentName));

        return services;
    }
}
