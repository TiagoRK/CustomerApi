using CustomerApi.Domain.Clients.Models;
using CustomerApi.SharedKernel;
using MediatR;

namespace CustomerApi.Application.Queries.Weather.GetCurrent;

public class GetCurrentWeatherQuery : IRequest<Result<WeatherForecastResult, Error>>
{
  public double Latitude { get; set; }
  public double Longitude { get; set; }
}
