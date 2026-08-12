using System.Collections.Concurrent;
using CustomerApi.Domain.Idempotency;
using CustomerApi.Web.Idempotency;
using Microsoft.Extensions.Caching.Memory;

namespace CustomerApi.Application.Services.Idempotency;

public sealed class IdempotencyStoreService(IMemoryCache cache) : IIdempotencyStoreService
{
  //Pra fins de simplicidade, usando imemorycache mas futuramente trocar pra redis
  private readonly IMemoryCache _cache = cache;
  //ConcurrentDictionary pra multithread safety
  private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

  public async Task<IdempotencyStartResultEnum> TryStartAsync(string key, TimeSpan ttl, CancellationToken cancellationToken)
  {
    //Usa dict de semaphore pra criar um lock pra cada key, dessa forma se chegar 2x a mesma request com mesma key, a primeira bloqueia a segunda até terminar de adicionar
    //evitando que as duas deixem a request passar
    var keyLock = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

    await keyLock.WaitAsync(cancellationToken);

    try
    {
      if (_cache.TryGetValue<IdempotencyEntry>(key, out var entry))
      {
        if (entry!.Status == IdempotencyRequestStatusEnum.InProgress)
        {
          return IdempotencyStartResultEnum.AlreadyInProgress;
        }

        if (entry.Status == IdempotencyRequestStatusEnum.Completed)
        {
          return IdempotencyStartResultEnum.AlreadyCompleted;
        }
      }

      _cache.Set(key, new IdempotencyEntry(IdempotencyRequestStatusEnum.InProgress, DateTimeOffset.UtcNow), ttl);
      return IdempotencyStartResultEnum.Started;
    }
    finally
    {
      keyLock.Release();
    }
  }

  public async Task SetStatusAsync(string key, IdempotencyRequestStatusEnum status, TimeSpan ttl, CancellationToken cancellationToken)
  {
    var keyLock = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
    await keyLock.WaitAsync(cancellationToken);

    try
    {
      _cache.Set(key, new IdempotencyEntry(status, DateTimeOffset.UtcNow), ttl);
    }
    finally
    {
      keyLock.Release();
    }
  }
}
