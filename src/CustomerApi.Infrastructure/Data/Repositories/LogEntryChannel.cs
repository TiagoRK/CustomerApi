using System.Threading.Channels;
using CustomerApi.Domain.Logging;

namespace CustomerApi.Infrastructure.Logging;

public class LogEntryChannel : ILogEntryChannel
{
  private readonly Channel<LogEntry> _channel = Channel.CreateUnbounded<LogEntry>(new UnboundedChannelOptions
  {
    SingleReader = true,
    SingleWriter = false
  });

  public ValueTask WriteAsync(LogEntry entry, CancellationToken cancellationToken = default)
    => _channel.Writer.WriteAsync(entry, cancellationToken);

  public IAsyncEnumerable<LogEntry> ReadAllAsync(CancellationToken cancellationToken = default)
    => _channel.Reader.ReadAllAsync(cancellationToken);
}
