using CustomerApi.Infrastructure.Data;
using CustomerApi.Infrastructure.IOC;
using CustomerApi.LogWorker;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
      using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
      var logger = loggerFactory.CreateLogger("Program");

      services.AddDbContext<LogDbContext>(options =>
          options.UseNpgsql(context.Configuration.GetConnectionString("Database")));

      var redisConnectionString = context.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
      services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnectionString));

      services.AddInfrastructureServices(context.Configuration, logger);

      services.AddHostedService<LogWorkerService>();
    })
    .Build();

await host.RunAsync();

