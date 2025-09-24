using CustomerApi.Application.Commands.Customers.Create;
using CustomerApi.Application.Commands.Customers.Delete;
using CustomerApi.Application.Commands.Customers.Update;
using CustomerApi.Application.Queries.Customers.GetByEmail;
using CustomerApi.Application.Queries.Customers.GetPaged;
using CustomerApi.Domain.Customers.DTO;
using CustomerApi.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Web.Controllers;

[ApiController]
[Route("/customers")]
public class CustomerController(IMediator mediator) : ApiController
{
  private readonly IMediator _mediator = mediator;

  [Produces("application/json")]
  [ProducesResponseType(StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(Error), StatusCodes.Status422UnprocessableEntity)]
  [HttpPost("createCustomer", Name = "CreateCustomer")]
  public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request)
  {
    var command = new CreateCustomerCommand()
    {
      Name = request.Name,
      BirthDate = request.BirthDate,
      Email = request.Email,
    };

    var result = await _mediator.Send(command);

    if (result.IsSuccess)
    {
      return Created(string.Empty, result.Value);
    }

    return CustomResponse(result);
  }

  [Produces("application/json")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(typeof(Error), StatusCodes.Status422UnprocessableEntity)]
  [HttpPut("updateCustomer/{email}", Name = "UpdateCustomer")]
  public async Task<IActionResult> UpdateCustomer([FromRoute] string email, [FromBody] UpdateCustomerRequest request)
  {
    var command = new UpdateCustomerCommand()
    {
      CurrentEmail = email,
      Name = request.Name,
      BirthDate = request.BirthDate,
      Email = request.Email,
    };

    var result = await _mediator.Send(command);

    return CustomResponse(result);
  }

  [Produces("application/json")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [HttpDelete("deleteCustomerByEmail/{email}", Name = "DeleteCustomerByEmail")]
  public async Task<IActionResult> DeleteCustomerByEmail([FromRoute] string email)
  {
    var command = new DeleteCustomerByEmailCommand()
    {
      Email = email
    };

    var result = await _mediator.Send(command);

    if (result.Value is null && result.IsSuccess)
    {
      return Ok();
    }

    return CustomResponse(result);
  }

  [Produces("application/json")]
  [ProducesResponseType(typeof(GetCustomerResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [HttpGet("getCustomerByEmail", Name = "GetCustomerByEmail")]
  public async Task<IActionResult> GetCustomerByEmail([FromQuery] string email)
  {
    var query = new GetByEmailQuery()
    {
      Email = email
    };

    var result = await _mediator.Send(query);

    return CustomResponse(result);
  }

  [Produces("application/json")]
  [ProducesResponseType(typeof(PagedResponse<GetCustomerResponse>), StatusCodes.Status200OK)]
  [HttpGet("getCustomerPaged", Name = "GetCustomerPaged")]
  public async Task<IActionResult> GetCustomerPaged([FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
  {
    var query = new GetPagedQuery()
    {
      PageSize = pageSize,
      PageNumber = pageNumber
    };

    var result = await _mediator.Send(query);

    return CustomResponse(result);
  }
}
