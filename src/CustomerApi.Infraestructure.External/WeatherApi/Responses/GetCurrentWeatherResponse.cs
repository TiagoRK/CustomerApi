using System.Text.Json.Serialization;

namespace CustomerApi.Infraestructure.External.WeatherApi.Responses;

public class GetCurrentWeatherResponse
{
  [JsonPropertyName("current_weather")]
  public CurrentWeatherData? CurrentWeather { get; init; }
}

public class CurrentWeatherData
{
  [JsonPropertyName("temperature")]
  public double Temperature { get; init; }

  [JsonPropertyName("windspeed")]
  public double WindSpeed { get; init; }

  [JsonPropertyName("weathercode")]
  public int WeatherCode { get; init; }

  [JsonPropertyName("time")]
  public DateTime Time { get; init; }
}
