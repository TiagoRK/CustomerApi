namespace CustomerApi.Web.Configurations;

public static class ServiceConfigs
{
  public static IServiceCollection AddServices(this IServiceCollection services, ILogger logger)
  {
    services.AddMediatrConfigs();

    logger.LogInformation("Mediatr registered");

    return services;
  }
}
