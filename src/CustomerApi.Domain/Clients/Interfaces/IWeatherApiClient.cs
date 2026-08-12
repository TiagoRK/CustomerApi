using CustomerApi.Domain.Clients.Models;

namespace CustomerApi.Domain.Clients.Interfaces;

public interface IWeatherApiClient
{
  Task<WeatherForecastResult?> GetCurrentWeatherAsync(double latitude, double longitude, CancellationToken cancellationToken = default);
}