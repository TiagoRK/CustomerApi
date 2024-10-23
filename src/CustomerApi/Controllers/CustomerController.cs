using CustomerApi.Application.Commands.Customers.Create;
using CustomerApi.Domain.Customers.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomerController(IMediator mediator) : ApiController
{
  private readonly IMediator _mediator = mediator;

  [Produces("application/json")]
  [HttpPost(Name = "CreateCustomer")]
  public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDTO request)
  {
    var command = new CreateCustomerCommand()
    {
      Name = request.Name,
      BirthDate = request.BirthDate,
      Email = request.Email,
    };

    var result = await _mediator.Send(command);

    return CustomResponse(result);
  }
}
