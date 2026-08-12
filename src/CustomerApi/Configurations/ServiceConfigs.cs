using CustomerApi.Application.Services.Idempotency;
using CustomerApi.Domain.Idempotency;
using StackExchange.Redis;

namespace CustomerApi.Web.Configurations;

public static class ServiceConfigs
{
  public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration, ILogger logger)
  {
    services.AddMediatrConfigs();

    var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";

    services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnectionString));
    services.AddSingleton<IIdempotencyStoreService, IdempotencyStoreService>();

    logger.LogInformation("Mediatr registered");
    logger.LogInformation("Redis connection registered");
    logger.LogInformation("Idempotency services registered");

    return services;
  }
}
