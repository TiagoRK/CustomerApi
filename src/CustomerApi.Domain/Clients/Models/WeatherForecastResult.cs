namespace CustomerApi.Domain.Clients.Models;

public sealed record WeatherForecastResult(
  double Temperature,
  double WindSpeed,
  int WeatherCode,
  DateTime ObservationTimeUtc
);