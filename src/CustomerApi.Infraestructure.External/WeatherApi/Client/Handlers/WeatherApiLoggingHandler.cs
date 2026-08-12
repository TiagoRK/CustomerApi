using Microsoft.Extensions.Logging;

namespace CustomerApi.Infraestructure.External.WeatherApi.Client.Handlers;

public class WeatherApiLoggingHandler(ILogger<WeatherApiLoggingHandler> logger) : DelegatingHandler
{
  private readonly ILogger<WeatherApiLoggingHandler> _logger = logger;

  protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
  {
    var requestPayload = request.Content is null
      ? "{}"
      : await request.Content.ReadAsStringAsync(cancellationToken);

    var endpoint = request.RequestUri?.PathAndQuery;
    if (string.IsNullOrWhiteSpace(endpoint))
    {
      endpoint = request.RequestUri?.ToString() ?? "/";
    }

    _logger.LogInformation(
      "WeatherApi request. Method: {Method}. Endpoint: {Endpoint}. RequestAtUtc: {RequestAtUtc}. Payload: {Payload}",
      request.Method.Method,
      endpoint,
      DateTime.UtcNow,
      requestPayload
    );

    var response = await base.SendAsync(request, cancellationToken);

    var responsePayload = response.Content is null
      ? "{}"
      : await response.Content.ReadAsStringAsync(cancellationToken);

    _logger.LogInformation(
      "WeatherApi response. Method: {Method}. Endpoint: {Endpoint}. ResponseAtUtc: {ResponseAtUtc}. StatusCode: {StatusCode}. Payload: {Payload}",
      request.Method.Method,
      endpoint,
      DateTime.UtcNow,
      (int)response.StatusCode,
      responsePayload
    );

    return response;
  }
}
