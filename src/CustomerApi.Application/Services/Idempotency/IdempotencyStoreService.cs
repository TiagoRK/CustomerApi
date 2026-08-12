using System.Text.Json;
using CustomerApi.Domain.Idempotency;
using CustomerApi.Web.Idempotency;
using StackExchange.Redis;

namespace CustomerApi.Application.Services.Idempotency;

public class IdempotencyStoreService(IConnectionMultiplexer redis) : IIdempotencyStoreService
{
  private readonly IDatabase _database = redis.GetDatabase();

  //funcionamento geral dessa arquitetura:
  //  lockKey = exclusão / coordenação
  //  stateKey = resultado / status

  //  o lock evita duplicidade
  //  o token evita conflito entre instâncias na hora de apagar o lock, garantindo que eu só apago o lock que estou mexendo nessa instância
  //  o state guarda o status da operação

  public async Task<IdempotencyStartResultEnum> TryStartAsync(string key, TimeSpan ttl, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    var lockKey = BuildLockKey(key);
    var stateKey = BuildStateKey(key);

    //To string n remove traços
    var lockToken = Guid.NewGuid().ToString("N");

    var hasLock = await _database.StringSetAsync(lockKey, lockToken, ttl, When.NotExists);

    //Haslock vai ser falso se não setou a string, se não setou é porquê já existe, então só resolve o status atual
    if (!hasLock)
    {
      return await ResolveExistingStatusAsync(stateKey);
    }

    var entry = new IdempotencyRedisEntry
    {
      Status = IdempotencyRequestStatusEnum.InProgress,
      UpdatedAt = DateTimeOffset.UtcNow,
      LockToken = lockToken
    };

    await _database.StringSetAsync(stateKey, JsonSerializer.Serialize(entry), ttl);
    return IdempotencyStartResultEnum.Started;
  }

  public async Task SetStatusAsync(string key, IdempotencyRequestStatusEnum status, TimeSpan ttl, CancellationToken cancellationToken)
  {
    cancellationToken.ThrowIfCancellationRequested();

    //Essa função é chamada pelo middleware de idempotency depois de completar a request
    //Ela vai pegar a key, buildar a chave de lock e a chave de estado usando ela
    //Depois, vai buscar o estado pela state key pois no redis a lockKey tem como valor o lockToken que identifica quem criou a lock, já o state key tem como valor o json da entry contendo o status, data de update e o lockToken
    //Então, usando a state key ele obtem o estado atual (entry)

    var lockKey = BuildLockKey(key);
    var stateKey = BuildStateKey(key);
    var currentState = await ReadStateAsync(stateKey);

    //Cria uma nova entry pro mesmo lockToken caso ele exista mas com nova data de update e o novo status
    var next = new IdempotencyRedisEntry
    {
      Status = status,
      UpdatedAt = DateTimeOffset.UtcNow,
      LockToken = currentState?.LockToken ?? string.Empty
    };

    //Seta esse novo na base
    await _database.StringSetAsync(stateKey, JsonSerializer.Serialize(next), ttl);

    //Se o novo tem lockToken, ou seja, já existia na base pois achou o currentState, tenta dar um release na lock usando a key + o lockToken obtido,
    //dessa forma evita que uma instancia pegue a mesma key com lockToken diferente e apague um lock sendo usado por outra instancia
    if (!string.IsNullOrWhiteSpace(next.LockToken))
    {
      await ReleaseLockAsync(lockKey, next.LockToken);
    }
  }

  private async Task<IdempotencyStartResultEnum> ResolveExistingStatusAsync(string stateKey)
  {
    var existing = await ReadStateAsync(stateKey);

    //fallback pra se não achar key, considera em progresso
    if (existing is null)
    {
      return IdempotencyStartResultEnum.AlreadyInProgress;
    }

    return existing.Status switch
    {
      IdempotencyRequestStatusEnum.InProgress => IdempotencyStartResultEnum.AlreadyInProgress,
      IdempotencyRequestStatusEnum.Completed => IdempotencyStartResultEnum.AlreadyCompleted,
      _ => IdempotencyStartResultEnum.AlreadyInProgress
    };
  }

  private async Task ReleaseLockAsync(string lockKey, string expectedToken)
  {
    var currentValue = await _database.StringGetAsync(lockKey);

    //Acima pega a lock pela chave, o valor dela é o token que identifica qual instância criou o lock, abaixo, se o lock não foi o lock criado por mim, só retorno
    if (currentValue.IsNullOrEmpty || !currentValue.Equals(expectedToken))
    {
      return;
    }

    //Aqui, se o lock foi criado por mim, crio uma transação na condição onde só vai rodar onde tem a minha chave e meu token e então deleto aquele conjunto chave/valor
    var transaction = _database.CreateTransaction();
    transaction.AddCondition(Condition.StringEqual(lockKey, expectedToken));
    var deleteTask = transaction.KeyDeleteAsync(lockKey);

    await transaction.ExecuteAsync();
    await deleteTask;
  }

  private async Task<IdempotencyRedisEntry?> ReadStateAsync(string stateKey)
  {
    var raw = await _database.StringGetAsync(stateKey);
    if (raw.IsNullOrEmpty)
    {
      return null;
    }

    return JsonSerializer.Deserialize<IdempotencyRedisEntry>(raw!);
  }

  private static string BuildLockKey(string key) => $"idempotency:lock:{key}";

  private static string BuildStateKey(string key) => $"idempotency:state:{key}";
}
