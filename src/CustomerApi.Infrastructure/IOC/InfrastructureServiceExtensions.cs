using CustomerApi.Domain.Customers;
using CustomerApi.Infrastructure.Data;
using CustomerApi.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CustomerApi.Infrastructure.IOC;

public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    ConfigurationManager config,
    ILogger logger)
  {
    string? connectionString = config.GetConnectionString("DbConnection");
    services.AddDbContext<CustomerDbContext>(options =>
     options.UseNpgsql(connectionString));

    services.AddScoped<ICustomerRepository, CustomerRepository>();

    logger.LogInformation("{Project} services registered", "Infrastructure");

    return services;
  }
}
