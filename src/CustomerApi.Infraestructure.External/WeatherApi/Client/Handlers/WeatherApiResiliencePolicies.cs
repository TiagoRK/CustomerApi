using System.Net;
using CustomerApi.Infraestructure.External.WeatherApi.Options;
using Polly;
using Polly.Extensions.Http;

namespace CustomerApi.Infraestructure.External.WeatherApi.Client.Handlers;

public static class WeatherApiResiliencePolicies
{
  public static IAsyncPolicy<HttpResponseMessage> CreateRetryPolicy(WeatherApiOptions options)
  {
    var retryOptions = options.Polly.RetryPolicy;

    var configuredDelays = retryOptions.RetryTimerInSeconds
      .Select(seconds => TimeSpan.FromSeconds(seconds))
      .ToList();

    return HttpPolicyExtensions
      .HandleTransientHttpError()
      .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
      .WaitAndRetryAsync(configuredDelays);
  }

  public static IAsyncPolicy<HttpResponseMessage> CreateCircuitBreakerPolicy(WeatherApiOptions options)
  {
    var circuitOptions = options.Polly.CircuitBreak;

    var breakDuration = TimeSpan.FromSeconds(circuitOptions.BreakDurationInSeconds);

    return HttpPolicyExtensions
      .HandleTransientHttpError()
      .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
      .CircuitBreakerAsync(
        circuitOptions.ExceptionsAllowedBeforeBreak,
        breakDuration
      );
  }
}
