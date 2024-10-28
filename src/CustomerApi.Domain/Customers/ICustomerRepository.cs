using CustomerApi.SharedKernel;

namespace CustomerApi.Domain.Customers;

public interface ICustomerRepository
{
  Task<long> Create(Customer customer);
  Task<Customer?> GetById(long id);
  Task<Customer?> GetByEmail(string name);
  Task<bool> IsEmailUnique(string email);
  Task<PagedResponse<Customer>> GetPaginated(int pageSize, int pageNumber);
  Task Delete(Customer customer);
}
