using CustomerApi.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CustomerApi.FunctionalTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
  protected override IHost CreateHost(IHostBuilder builder)
  {
    builder.UseEnvironment("Development");

    ConfigureWebHost(builder);

    var host = builder.Build();
    host.Start();

    var serviceProvider = host.Services;

    using (var scope = serviceProvider.CreateScope())
    {
      var scopedServices = scope.ServiceProvider;
      var db = scopedServices.GetRequiredService<CustomerDbContext>();

      var logger = scopedServices
          .GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

      db.Database.EnsureDeleted();
      db.Database.EnsureCreated();

      try
      {
        DataSeeder.PopulateTestData(db, 10).Wait();
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "An error occurred seeding the " +
                            "database with test messages. Error: {exceptionMessage}", ex.Message);
      }
    }

    return host;
  }

  protected void ConfigureWebHost(IHostBuilder builder)
  {
    builder.ConfigureServices(services =>
    {
      var descriptor = services.SingleOrDefault(
          d => d.ServiceType == typeof(DbContextOptions<CustomerDbContext>));

      if (descriptor != null)
      {
        services.Remove(descriptor);
      }

      var inMemoryCollectionName = Guid.NewGuid().ToString();

      var serviceProvider = new ServiceCollection()
          .AddEntityFrameworkInMemoryDatabase()
          .BuildServiceProvider();

      services.AddDbContext<CustomerDbContext>(options =>
      {
        options.UseInMemoryDatabase(inMemoryCollectionName)
               .UseInternalServiceProvider(serviceProvider);
      });
    });
  }
}
