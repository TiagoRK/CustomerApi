using System.Text.Json;
using System.Threading.Channels;
using CustomerApi.Domain.Logging;

namespace CustomerApi.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next)
{
  public async Task InvokeAsync(HttpContext context, ILogEntryChannel logEntryChannel)
  {
    var isCustomerController = context.Request.Path.Value?.Contains("/customers", StringComparison.OrdinalIgnoreCase) ?? false;
    if (!isCustomerController)
    {
      await next(context);
      return;
    }

    var callId = Guid.NewGuid();
    var endpoint = context.Request.Path.Value ?? "/";
    var method = context.Request.Method;

    string requestPayload;
    context.Request.EnableBuffering();
    using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
    {
      requestPayload = await reader.ReadToEndAsync();
      context.Request.Body.Position = 0;
    }

    var requestEntry = new LogEntry
    {
      Id = callId,
      Endpoint = endpoint,
      Method = method,
      Payload = requestPayload,
      Direction = LogDirection.Request,
      CreatedAt = DateTimeOffset.UtcNow
    };

    await logEntryChannel.WriteAsync(requestEntry, context.RequestAborted);

    var originalBody = context.Response.Body;
    using var responseBuffer = new MemoryStream();
    context.Response.Body = responseBuffer;

    await next(context);

    responseBuffer.Position = 0;
    var responsePayload = await new StreamReader(responseBuffer).ReadToEndAsync();
    responseBuffer.Position = 0;
    await responseBuffer.CopyToAsync(originalBody);
    context.Response.Body = originalBody;

    var responseEntry = new LogEntry
    {
      Id = Guid.NewGuid(),
      CorrelationId = callId,
      Endpoint = endpoint,
      Method = method,
      Payload = responsePayload,
      StatusCode = context.Response.StatusCode,
      Direction = LogDirection.Response,
      CreatedAt = DateTimeOffset.UtcNow
    };

    await logEntryChannel.WriteAsync(responseEntry, context.RequestAborted);
  }
}
