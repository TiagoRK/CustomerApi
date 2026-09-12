namespace CustomerApi.Domain.Logging;

public interface ILogEntryChannel
{
  ValueTask WriteAsync(LogEntry entry, CancellationToken cancellationToken = default);
  IAsyncEnumerable<LogEntry> ReadAllAsync(CancellationToken cancellationToken = default);
}
