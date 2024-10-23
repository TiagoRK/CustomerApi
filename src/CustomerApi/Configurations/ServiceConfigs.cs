using CustomerApi.Infrastructure.IOC;

namespace CustomerApi.Web.Configurations;

public static class ServiceConfigs
{
  public static IServiceCollection AddServiceConfigs(this IServiceCollection services, ILogger logger, WebApplicationBuilder builder)
  {
    services.AddInfrastructureServices(builder.Configuration, logger)
            .AddMediatrConfigs();

    logger.LogInformation("{Project} services registered", "Mediatr");

    return services;
  }
}
