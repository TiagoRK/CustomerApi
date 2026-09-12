using CustomerApi.Domain.Logging;
using CustomerApi.Infrastructure.Data;
using CustomerApi.Infrastructure.Logging;
using CustomerApi.LogWorker;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
      services.AddDbContext<CustomerDbContext>(options =>
          options.UseNpgsql(context.Configuration.GetConnectionString("Database")));

      services.AddSingleton<ILogEntryChannel, LogEntryChannel>();
      services.AddHostedService<LogWorkerService>();
    })
    .Build();

await host.RunAsync();
