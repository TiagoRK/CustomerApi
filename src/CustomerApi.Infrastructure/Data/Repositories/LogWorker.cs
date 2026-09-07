using CustomerApi.Domain.Logging;
using CustomerApi.Infrastructure.Data;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CustomerApi.Infrastructure.Logging;

public class LogWorker(ILogEntryChannel logEntryChannel, IServiceScopeFactory scopeFactory, ILogger<LogWorker> logger) : BackgroundService
{
  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    await foreach (var entry in logEntryChannel.ReadAllAsync(stoppingToken))
    {
      try
      {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
        dbContext.LogEntries.Add(entry);
        await dbContext.SaveChangesAsync(stoppingToken);
      }
      catch (Exception ex) when (ex is not OperationCanceledException)
      {
        logger.LogError(ex, "Error persisting log entry {Id}", entry.Id);
      }
    }
  }
}
