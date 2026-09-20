using CustomerApi.Domain.Logging;

namespace CustomerApi.Domain.Idempotency;

public interface ILogEntryChannel
{
  ValueTask WriteAsync(LogEntry entry, CancellationToken cancellationToken = default);
  IAsyncEnumerable<LogEntry> ReadAllAsync(CancellationToken cancellationToken = default);
  ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default);
}
