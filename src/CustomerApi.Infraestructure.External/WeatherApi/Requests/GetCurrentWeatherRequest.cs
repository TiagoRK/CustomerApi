namespace CustomerApi.Infraestructure.External.WeatherApi.Requests;

public sealed record GetCurrentWeatherRequest(double Latitude, double Longitude);