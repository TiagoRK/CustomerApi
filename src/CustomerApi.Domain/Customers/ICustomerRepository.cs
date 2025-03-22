using CustomerApi.SharedKernel;

namespace CustomerApi.Domain.Customers;

public interface ICustomerRepository
{
  Task<long> Create(Customer customer);
  Task<Customer?> GetById(long id, bool noTracking = false);
  Task<Customer?> GetByEmail(string name, bool noTracking = false);
  Task<bool> IsEmailUnique(string email);
  Task<PagedResponse<Customer>> GetPaginated(int pageSize, int pageNumber);
  Task Delete(Customer customer);
  Task Update(Customer customer);

}
