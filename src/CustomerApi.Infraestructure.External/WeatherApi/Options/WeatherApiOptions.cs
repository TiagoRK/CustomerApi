namespace CustomerApi.Infraestructure.External.WeatherApi.Options;

public class WeatherApiOptions
{
  public const string SectionName = "WeatherApi";

  public string BaseUrl { get; set; } = string.Empty;
  public PollyOptions Polly { get; set; } = new();
}

public class PollyOptions
{
  public RetryPolicyOptions RetryPolicy { get; set; } = new();
  public CircuitBreakOptions CircuitBreak { get; set; } = new();
}

public class RetryPolicyOptions
{
  public List<int> RetryTimerInSeconds { get; set; } = [3, 5, 10];
}

public class CircuitBreakOptions
{
  public int ExceptionsAllowedBeforeBreak { get; set; } = 3;
  public int BreakDurationInSeconds { get; set; } = 30;
}
