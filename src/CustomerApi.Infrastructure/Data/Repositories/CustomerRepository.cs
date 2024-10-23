using CustomerApi.Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Infrastructure.Data.Repositories;

public class CustomerRepository(CustomerDbContext dbContext) : ICustomerRepository
{
  private readonly CustomerDbContext _dbContext = dbContext;

  public async Task<long> CreateCustomer(Customer customer)
  {
    await _dbContext.Customers.AddAsync(customer);
    await _dbContext.SaveChangesAsync();

    return customer.Id;
  }

  public async Task<Customer?> GetCustomerById(long id)
  {
    var customer = await _dbContext.Customers.FirstOrDefaultAsync(customer => customer.Id == id);

    if (customer == null)
    {
      return null;
    }

    return customer;
  }

  public async Task<Customer?> GetCustomerByName(string name)
  {
    var customer = await _dbContext.Customers.FirstOrDefaultAsync(customer => customer.Name == name);

    if (customer == null)
    {
      return null;
    }

    return customer;
  }

  public async Task<bool> IsEmailUnique(string email)
  {
    return !await _dbContext.Customers.AnyAsync(customer => customer.Email.Equals(email));
  }
}
