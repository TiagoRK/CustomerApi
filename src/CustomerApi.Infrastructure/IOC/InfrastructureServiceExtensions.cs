﻿using CustomerApi.Domain.Customers;
using CustomerApi.Infrastructure.Data;
using CustomerApi.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CustomerApi.Infrastructure.IOC;

public static class InfrastructureServiceExtensions
{
  public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, ConfigurationManager config, ILogger logger)
  {
    var connectionString = config.GetConnectionString("Database");

    services.AddDbContext<CustomerDbContext>(options => options.UseNpgsql(connectionString));

    logger.LogInformation("Database context added");

    services.AddScoped<ICustomerRepository, CustomerRepository>();

    logger.LogInformation("Repositories registered");

    return services;
  }
}
