namespace CustomerApi.SharedKernel;

public abstract class BusinessValidator<T>
{
  private readonly List<(Func<T, bool> ValidationFunc, Func<T, Error> ValidationErrorFactory)> _syncBusinessRules = [];
  private readonly List<(Func<T, Task<bool>> ValidationFunc, Func<T, Error> ValidationErrorFactory)> _asyncBusinessRules = [];
  private readonly List<(string Key, Func<T, Task<(bool IsValid, object Value)>> ValidationFunc, Func<T, Error> ValidationErrorFactory)> _valueReturningAsyncBusinessRules = [];

  protected void AddSyncBusinessRule(Func<T, bool> validationFunc, Func<T, Error> validationErrorFactory)
  {
    _syncBusinessRules.Add((validationFunc, validationErrorFactory));
  }

  protected void AddAsyncBusinessRule(Func<T, Task<bool>> validationFunc, Func<T, Error> validationErrorFactory)
  {
    _asyncBusinessRules.Add((validationFunc, validationErrorFactory));
  }

  protected void AddValueReturningAsyncBusinessRule(string key, Func<T, Task<(bool IsValid, object Value)>> validationFunc, Func<T, Error> validationErrorFactory)
  {
    _valueReturningAsyncBusinessRules.Add((key, validationFunc, validationErrorFactory));
  }

  protected async Task<(List<Error> Errors, Dictionary<string, object?> Values)> Validate(T command, CancellationToken cancellationToken)
  {
    var errors = new List<Error>();
    var values = new Dictionary<string, object?>();

    foreach (var (validationFunc, validationErrorFactory) in _syncBusinessRules)
    {
      var isValid = validationFunc(command);
      if (!isValid)
      {
        errors.Add(validationErrorFactory(command));
        return (errors, values);
      }
    }

    var asyncTasks = _asyncBusinessRules
        .Select(rule => Task.Run(async () =>
        {
          var isValid = await rule.ValidationFunc(command);
          return (isValid, rule.ValidationErrorFactory(command));
        }, cancellationToken))
        .ToList();

    while (asyncTasks.Count != 0)
    {
      var completedTask = await Task.WhenAny(asyncTasks);
      var (isValid, validationError) = await completedTask;

      if (!isValid)
      {
        errors.Add(validationError);
        cancellationToken.ThrowIfCancellationRequested();
        return (errors, values);
      }

      asyncTasks.Remove(completedTask);
    }

    var valueReturningTasks = _valueReturningAsyncBusinessRules
        .Select(rule => Task.Run(async () =>
        {
          var (isValid, value) = await rule.ValidationFunc(command);
          return (isValid, rule.ValidationErrorFactory(command), rule.Key, value);
        }, cancellationToken))
        .ToList();

    while (valueReturningTasks.Count != 0)
    {
      var completedTask = await Task.WhenAny(valueReturningTasks);
      var (isValid, validationError, key, value) = await completedTask;

      if (!isValid)
      {
        errors.Add(validationError);
        cancellationToken.ThrowIfCancellationRequested();
        return (errors, values);
      }

      values[key] = value;
      valueReturningTasks.Remove(completedTask);
    }

    return (errors, values);
  }
}
