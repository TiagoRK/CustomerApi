using CustomerApi.SharedKernel;
using MediatR;

namespace CustomerApi.Application.Commands.Customers.Delete;
public class DeleteCustomerByEmailCommand : IRequest<Result<object, Error>>
{
  public string Email { get; set; }
}
