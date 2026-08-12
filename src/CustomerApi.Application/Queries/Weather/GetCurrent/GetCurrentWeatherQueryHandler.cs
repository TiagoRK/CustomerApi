using CustomerApi.Domain.Clients.Interfaces;
using CustomerApi.Domain.Clients.Models;
using CustomerApi.SharedKernel;
using MediatR;

namespace CustomerApi.Application.Queries.Weather.GetCurrent;

public class GetCurrentWeatherQueryHandler(IWeatherApiClient weatherApiClient) : IRequestHandler<GetCurrentWeatherQuery, Result<WeatherForecastResult, Error>>
{
  private readonly IWeatherApiClient _weatherApiClient = weatherApiClient;

  public async Task<Result<WeatherForecastResult, Error>> Handle(GetCurrentWeatherQuery request, CancellationToken cancellationToken)
  {
    var weather = await _weatherApiClient.GetCurrentWeatherAsync(request.Latitude, request.Longitude, cancellationToken);

    if (weather is null)
    {
      return Result<WeatherForecastResult, Error>.SuccessWithNull();
    }

    return weather;
  }
}
