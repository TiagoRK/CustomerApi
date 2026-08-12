namespace CustomerApi.Web.Idempotency;

public enum IdempotencyStartResultEnum
{
  Started = 0,
  AlreadyInProgress = 1,
  AlreadyCompleted = 2,
}
