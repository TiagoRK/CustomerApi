using CustomerApi.Infrastructure.Logging;
using CustomerApi.Domain.Logging;
using CustomerApi.Infrastructure.Data;
using CustomerApi.Infrastructure.Data.Repositories;
using CustomerApi.LogWorker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<CustomerDbContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));

        services.AddSingleton<ILogEntryChannel, LogEntryChannel>();
        services.AddHostedService<LogWorkerService>();
    })
    .Build();

await host.RunAsync();
