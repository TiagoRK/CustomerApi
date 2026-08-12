using CustomerApi.Web.Idempotency;

namespace CustomerApi.Domain.Idempotency;

public class IdempotencyRedisEntry
{
  public IdempotencyRequestStatusEnum Status { get; set; }
  public DateTimeOffset UpdatedAt { get; set; }
  public string? LockToken { get; set; }
}
