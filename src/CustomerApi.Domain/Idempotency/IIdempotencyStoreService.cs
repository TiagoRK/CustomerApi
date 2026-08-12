using CustomerApi.Web.Idempotency;

namespace CustomerApi.Domain.Idempotency;

public interface IIdempotencyStoreService
{
  Task<IdempotencyStartResultEnum> TryStartAsync(string key, TimeSpan ttl, CancellationToken cancellationToken);

  Task SetStatusAsync(string key, IdempotencyRequestStatusEnum status, TimeSpan ttl, CancellationToken cancellationToken);
}
