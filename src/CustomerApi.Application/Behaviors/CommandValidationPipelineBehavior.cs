using System.Reflection;
using CustomerApi.SharedKernel;
using MediatR;

namespace CustomerApi.Application.Behaviors;

public class CommandValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
  where TRequest : notnull
{
  public async Task<TResponse> Handle(
    TRequest request,
    RequestHandlerDelegate<TResponse> next,
    CancellationToken cancellationToken)
  {
    if (request is CommandValidator command && !command.IsValid())
    {
      return CreateValidationResponse(command.ValidationErrors);
    }

    return await next();
  }

  private static TResponse CreateValidationResponse(List<Error> errors)
  {
    var responseType = typeof(TResponse);

    if (responseType.IsGenericType
      && responseType.GetGenericTypeDefinition() == typeof(Result<,>)
      && responseType.GetGenericArguments()[1] == typeof(Error))
    {
      var failureCtor = responseType.GetConstructor(
        BindingFlags.Instance | BindingFlags.NonPublic,
        binder: null,
        types: [typeof(List<Error>)],
        modifiers: null);

      if (failureCtor is not null)
      {
        return (TResponse)failureCtor.Invoke([errors]);
      }
    }

    throw new InvalidOperationException(
      $"{nameof(CommandValidationPipelineBehavior<TRequest, TResponse>)} requires responses of type Result<TValue, Error>. " +
      $"Current response type: {responseType.FullName}");
  }
}