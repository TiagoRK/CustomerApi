namespace CustomerApi.Web.Idempotency;

public enum IdempotencyRequestStatusEnum
{
  InProgress = 0,
  Completed = 1,
  Failed = 2,
}
