using CustomerApi.Domain.Customers.DTO;
using CustomerApi.SharedKernel;
using MediatR;

namespace CustomerApi.Application.Queries.Customers.GetPaged;
public class GetPagedQuery : IRequest<Result<PagedResponse<GetCustomerResponse>, Error>>
{
  private int _pageSize = 10;
  private int _pageNumber = 1;

  public int PageSize
  {
    get => _pageSize;
    set
    {
      _pageSize = value;

      if (_pageSize < 1) _pageSize = 10;
      if (_pageSize > 50) _pageSize = 50;
    }
  }

  public int PageNumber
  {
    get => _pageNumber;
    set
    {
      _pageNumber = value;

      if (_pageNumber < 1) _pageNumber = 1;
    }
  }
}
