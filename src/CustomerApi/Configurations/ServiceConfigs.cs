using CustomerApi.Application.Services.Idempotency;
using CustomerApi.Domain.Idempotency;

namespace CustomerApi.Web.Configurations;

public static class ServiceConfigs
{
  public static IServiceCollection AddServices(this IServiceCollection services, ILogger logger)
  {
    services.AddMediatrConfigs();
    services.AddMemoryCache();
    services.AddSingleton<IIdempotencyStoreService, IdempotencyStoreService>();

    logger.LogInformation("Mediatr registered");
    logger.LogInformation("Idempotency services registered");

    return services;
  }
}
