namespace CustomerApi.Domain.Logging;

public class LogEntry
{
  public Guid Id { get; set; }
  public Guid? CorrelationId { get; set; }
  public string Endpoint { get; set; } = string.Empty;
  public string Method { get; set; } = string.Empty;
  public string? Payload { get; set; }
  public int? StatusCode { get; set; }
  public LogDirection Direction { get; set; }
  public DateTimeOffset CreatedAt { get; set; }
}
