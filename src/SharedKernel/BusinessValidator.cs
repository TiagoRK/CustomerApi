namespace CustomerApi.SharedKernel;

public abstract class BusinessValidator<T>
{
  private readonly List<(Func<T, bool> ValidationFunc, Error ValidationError)> _syncBusinessRules = [];
  private readonly List<(Func<T, Task<bool>> ValidationFunc, Error ValidationError)> _asyncBusinessRules = [];

  protected void AddAsyncBusinessRule(Func<T, Task<bool>> validationFunc, Error validationError)
  {
    _asyncBusinessRules.Add((validationFunc, validationError));
  }

  protected void AddSyncBusinessRule(Func<T, bool> validationFunc, Error validationError)
  {
    _syncBusinessRules.Add((validationFunc, validationError));
  }

  protected async Task<List<Error>> Validate(T command, CancellationToken cancellationToken)
  {
    var errors = new List<Error>();

    foreach (var (validationFunc, validationError) in _syncBusinessRules)
    {
      var isValid = validationFunc(command);
      if (!isValid)
      {
        errors.Add(validationError);
        return errors;
      }
    }

    var tasks = _asyncBusinessRules
        .Select(rule => Task.Run(async () =>
        {
          var isValid = await rule.ValidationFunc(command);
          return (isValid, rule.ValidationError);
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
