using CustomerApi.Application.Queries.Weather.GetCurrent;
using CustomerApi.Domain.Clients.Models;
using CustomerApi.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Web.Controllers;

[ApiController]
[Route("/weather")]
public class WeatherController(IMediator mediator) : ApiController
{
  private readonly IMediator _mediator = mediator;

  [Produces("application/json")]
  [ProducesResponseType(typeof(WeatherForecastResult), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(Error), StatusCodes.Status422UnprocessableEntity)]
  [HttpGet("current", Name = "GetCurrentWeather")]
  public async Task<IActionResult> GetCurrentWeather([FromQuery] double latitude, [FromQuery] double longitude)
  {
    var query = new GetCurrentWeatherQuery
    {
      Latitude = latitude,
      Longitude = longitude,
    };

    var result = await _mediator.Send(query);

    return CustomResponse(result);
  }
}