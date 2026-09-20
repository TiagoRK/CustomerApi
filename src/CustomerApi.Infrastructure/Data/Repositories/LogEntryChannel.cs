using System.Runtime.CompilerServices;
using System.Text.Json;
using CustomerApi.Domain.Idempotency;
using CustomerApi.Domain.Logging;
using StackExchange.Redis;

namespace CustomerApi.Infrastructure.Data.Repositories;

public class LogEntryChannel(IConnectionMultiplexer redis) : ILogEntryChannel
{
  private const string QueueName = "customer-api:log-entries";
  private readonly IDatabase _database = redis.GetDatabase();

  public ValueTask WriteAsync(LogEntry entry, CancellationToken cancellationToken = default)
    => new(WriteEntryAsync(entry, cancellationToken));

  public async IAsyncEnumerable<LogEntry> ReadAllAsync(
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    while (!cancellationToken.IsCancellationRequested)
    {
      var value = await _database.ListLeftPopAsync(QueueName);
      if (value.IsNullOrEmpty)
      {
        await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
        continue;
      }

      var entry = JsonSerializer.Deserialize<LogEntry>(value!);
      if (entry is not null)
        yield return entry;
    }
  }

  public async ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default)
  {
    while (!cancellationToken.IsCancellationRequested)
    {
      if (await _database.ListLengthAsync(QueueName) > 0)
        return true;

      await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
    }

    return false;
  }

  private async Task WriteEntryAsync(LogEntry entry, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();
    var payload = JsonSerializer.Serialize(entry);
    await _database.ListRightPushAsync(QueueName, payload);
  }
}
