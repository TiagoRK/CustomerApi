using CustomerApi.Domain.Clients.Interfaces;
using CustomerApi.Infraestructure.External.WeatherApi.Client;
using CustomerApi.Infraestructure.External.WeatherApi.Client.Handlers;
using CustomerApi.Infraestructure.External.WeatherApi.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CustomerApi.Infraestructure.External.IOC;

public static class ExternalInfrastructureServiceExtensions
{
  public static IServiceCollection AddExternalInfrastructureServices(this IServiceCollection services, IConfiguration configuration, ILogger logger)
  {
    AddWeatherApiClient(services, configuration, logger);

    return services;
  }

  private static void AddWeatherApiClient(IServiceCollection services, IConfiguration configuration, ILogger logger)
  {
    services.Configure<WeatherApiOptions>(configuration.GetSection(WeatherApiOptions.SectionName));

    services.AddTransient<WeatherApiLoggingHandler>();

    services
      .AddHttpClient<IWeatherApiClient, WeatherApiClient>((serviceProvider, client) =>
      {
        var weatherApiOptions = serviceProvider.GetRequiredService<IOptions<WeatherApiOptions>>().Value;
        client.BaseAddress = new Uri(weatherApiOptions.BaseUrl);
      })
      .AddPolicyHandler((serviceProvider, _) =>
      {
        var weatherApiOptions = serviceProvider.GetRequiredService<IOptions<WeatherApiOptions>>().Value;
        return WeatherApiResiliencePolicies.CreateRetryPolicy(weatherApiOptions);
      })
      .AddPolicyHandler((serviceProvider, _) =>
      {
        var weatherApiOptions = serviceProvider.GetRequiredService<IOptions<WeatherApiOptions>>().Value;
        return WeatherApiResiliencePolicies.CreateCircuitBreakerPolicy(weatherApiOptions);
      })
      .AddHttpMessageHandler<WeatherApiLoggingHandler>();

    logger.LogInformation("External WeatherApi client registered");
  }
}
