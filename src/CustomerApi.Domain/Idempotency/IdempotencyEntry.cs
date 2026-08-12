namespace CustomerApi.Web.Idempotency;

public sealed record IdempotencyEntry(IdempotencyRequestStatusEnum Status, DateTimeOffset UpdatedAt);
