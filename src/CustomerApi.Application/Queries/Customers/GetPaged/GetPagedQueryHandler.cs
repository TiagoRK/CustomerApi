using CustomerApi.Domain.Customers;
using CustomerApi.Domain.Customers.DTO;
using CustomerApi.SharedKernel;
using MediatR;

namespace CustomerApi.Application.Queries.Customers.GetPaged;
public class GetPagedQueryHandler(ICustomerRepository customerRepository) : IRequestHandler<GetPagedQuery, Result<PagedResponse<GetCustomerResponse>, Error>>
{
  private readonly ICustomerRepository _customerRepository = customerRepository;

  public async Task<Result<PagedResponse<GetCustomerResponse>, Error>> Handle(GetPagedQuery request, CancellationToken cancellationToken)
  {
    var pagedResult = await _customerRepository.GetPaginated(request.PageSize, request.PageNumber);

    var customersDTO = pagedResult.Data.Select(customer => new GetCustomerResponse()
    {
      Id = customer.Id,
      Name = customer.Name,
      BirthDate = customer.BirthDate,
      Email = customer.Email,
    }).ToList();

    return new PagedResponse<GetCustomerResponse>(
      customersDTO,
      pagedResult.PageNumber,
      pagedResult.PageSize,
      pagedResult.TotalRecords
    );
  }
}
