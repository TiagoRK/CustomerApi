using System.Net.Http.Json;
using CustomerApi.Domain.Clients.Interfaces;
using CustomerApi.Domain.Clients.Models;
using CustomerApi.Infraestructure.External.WeatherApi.Requests;
using CustomerApi.Infraestructure.External.WeatherApi.Responses;

namespace CustomerApi.Infraestructure.External.WeatherApi.Client;

public class WeatherApiClient(HttpClient httpClient) : IWeatherApiClient
{
  private readonly HttpClient _httpClient = httpClient;

  public async Task<WeatherForecastResult?> GetCurrentWeatherAsync(double latitude, double longitude, CancellationToken cancellationToken = default)
  {
    var request = new GetCurrentWeatherRequest(latitude, longitude);

    var endpoint = $"/v1/forecast?latitude={request.Latitude}&longitude={request.Longitude}&current_weather=true";

    var response = await _httpClient.GetFromJsonAsync<GetCurrentWeatherResponse>(endpoint, cancellationToken);

    var currentWeather = response?.CurrentWeather;
    if (currentWeather is null)
    {
      return null;
    }

    return new WeatherForecastResult(
      currentWeather.Temperature,
      currentWeather.WindSpeed,
      currentWeather.WeatherCode,
      DateTime.SpecifyKind(currentWeather.Time, DateTimeKind.Utc)
    );
  }
}
