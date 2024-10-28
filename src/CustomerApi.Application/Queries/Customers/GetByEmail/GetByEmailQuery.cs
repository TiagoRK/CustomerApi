using CustomerApi.Domain.Customers.DTO;
using CustomerApi.SharedKernel;
using MediatR;

namespace CustomerApi.Application.Queries.Customers.GetByEmail;
public class GetByEmailQuery : IRequest<Result<GetCustomerResponse, Error>>
{
  public string Email { get; set; }
}
