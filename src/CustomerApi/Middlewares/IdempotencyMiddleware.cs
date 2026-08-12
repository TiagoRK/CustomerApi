using CustomerApi.Domain.Idempotency;
using CustomerApi.Web.Attributes;
using CustomerApi.Web.Idempotency;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Web.Middlewares;

public sealed class IdempotencyMiddleware(RequestDelegate next)
{
  private readonly RequestDelegate _next = next;

  public async Task InvokeAsync(HttpContext context, IIdempotencyStoreService idempotencyStoreService)
  {
    var hasIdempotentAttribute = context.GetEndpoint()?.Metadata.GetMetadata<IsIdempotentAttribute>();
    if (hasIdempotentAttribute is null)
    {
      await _next(context);
      return;
    }

    if (!context.Request.Headers.TryGetValue(hasIdempotentAttribute.HeaderName, out var keyHeader) || string.IsNullOrWhiteSpace(keyHeader))
    {
      await WriteProblemDetailsResponse(
        context,
        StatusCodes.Status400BadRequest,
        "Missing idempotency key",
        $"The header '{hasIdempotentAttribute.HeaderName}' is required for this endpoint.");

      return;
    }

    var requestKey = keyHeader.ToString().Trim();
    var idempotencyKey = $"{context.Request.Method}:{context.Request.Path}:{requestKey}";
    var ttl = TimeSpan.FromMinutes(hasIdempotentAttribute.ExpirationInMinutes);
    var tryStart = await idempotencyStoreService.TryStartAsync(idempotencyKey, ttl, context.RequestAborted);

    if (tryStart == IdempotencyStartResultEnum.AlreadyInProgress)
    {
      await WriteProblemDetailsResponse(
        context,
        StatusCodes.Status409Conflict,
        "Request already in progress",
        "A request with the same idempotency key is already being processed.");

      return;
    }

    if (tryStart == IdempotencyStartResultEnum.AlreadyCompleted)
    {
      await WriteProblemDetailsResponse(
        context,
        StatusCodes.Status409Conflict,
        "Request already processed",
        "A request with this idempotency key has already been successfully processed.");

      return;
    }

    try
    {
      await _next(context);

      //Aqui é debativel, uma request que falha uma validação pode não ser tratada como falha
      //por causa disso, coloquei pra verificar se for qualquer código de erro mesmo (500 pra cima) e tratar como falha
      var status = context.Response.StatusCode >= StatusCodes.Status500InternalServerError
        ? IdempotencyRequestStatusEnum.Failed
        : IdempotencyRequestStatusEnum.Completed;

      await idempotencyStoreService.SetStatusAsync(idempotencyKey, status, ttl, context.RequestAborted);
    }
    catch
    {
      await idempotencyStoreService.SetStatusAsync(idempotencyKey, IdempotencyRequestStatusEnum.Failed, ttl, context.RequestAborted);
      throw;
    }
  }

  private static Task WriteProblemDetailsResponse(HttpContext context, int statusCode, string title, string detail)
  {
    var problemDetails = new ProblemDetails
    {
      Status = statusCode,
      Title = title,
      Detail = detail
    };

    context.Response.StatusCode = statusCode;

    return context.Response.WriteAsJsonAsync(problemDetails);
  }
}
