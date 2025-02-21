namespace CustomerApi.SharedKernel;

public abstract class BusinessValidator<T> : IBusinessValidator
{
  private readonly List<(Func<T, bool> ValidationFunc, Func<T, Error> ValidationErrorFunc)> _syncBusinessRules = [];
  private readonly List<(Func<T, Task<bool>> ValidationFunc, Func<T, Error> ValidationErrorFunc)> _asyncBusinessRules = [];

  public abstract void AddBusinessRules();

  protected void AddAsyncBusinessRule(Func<T, Task<bool>> validationFunc, Func<T, Error> validationErrorFunc)
  {
    _asyncBusinessRules.Add((validationFunc, validationErrorFunc));
  }

  protected void AddAsyncBusinessRule(Func<T, Task<bool>> validationFunc, Error validationError)
  {
    _asyncBusinessRules.Add((validationFunc, _ => validationError));
  }

  protected void AddSyncBusinessRule(Func<T, bool> validationFunc, Func<T, Error> validationErrorFunc)
  {
    _syncBusinessRules.Add((validationFunc, validationErrorFunc));
  }

  protected void AddSyncBusinessRule(Func<T, bool> validationFunc, Error validationError)
  {
    _syncBusinessRules.Add((validationFunc, _ => validationError));
  }

  protected async Task<List<Error>> Validate(T command)
  {
    var errors = new List<Error>();

    foreach (var (validationFunc, validationErrorFunc) in _syncBusinessRules)
    {
      var isValid = validationFunc(command);
      if (!isValid)
      {
        errors.Add(validationErrorFunc(command));
        return errors;
      }
    }

    using var validationCts = new CancellationTokenSource();
    var cancellationToken = validationCts.Token;

    var tasks = _asyncBusinessRules
        .Select(rule => Task.Run(async () =>
        {
          var isValid = await rule.ValidationFunc(command);
          return (isValid, rule.ValidationErrorFunc(command));
        }, cancellationToken))
        .ToList();

    while (tasks.Count != 0)
    {
      var completedTask = await Task.WhenAny(tasks);

      var (isValid, validationError) = await completedTask;

      if (!isValid)
      {
        errors.Add(validationError);
        cancellationToken.ThrowIfCancellationRequested();
      }

      tasks.Remove(completedTask);
    }

    return errors;
  }
}
