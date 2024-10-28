using CustomerApi.Domain.Customers;
using CustomerApi.Domain.Customers.DTO;
using CustomerApi.SharedKernel;
using MediatR;

namespace CustomerApi.Application.Queries.Customers.GetByEmail;
public class GetByEmailQueryHandler(ICustomerRepository customerRepository) : IRequestHandler<GetByEmailQuery, Result<GetCustomerResponse, Error>>
{
  private readonly ICustomerRepository _customerRepository = customerRepository;

  public async Task<Result<GetCustomerResponse, Error>> Handle(GetByEmailQuery request, CancellationToken cancellationToken)
  {
    var customer = await _customerRepository.GetByEmail(request.Email);

    if (customer == null)
    {
      return Result<GetCustomerResponse, Error>.SuccessWithNull();
    }

    return new GetCustomerResponse()
    {
      Name = customer.Name,
      BirthDate = customer.BirthDate,
      Email = customer.Email,
    };
  }
}
