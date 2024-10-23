using CustomerApi.Domain.Customers;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Infrastructure.Data.Repositories;

public class CustomerRepository(CustomerDbContext dbContext) : ICustomerRepository
{
  public CustomerDbContext DbContext { get; set; } = dbContext;

  public async Task<long> CreateCustomer(Customer customer)
  {
    await DbContext.Customers.AddAsync(customer);
    await DbContext.SaveChangesAsync();

    return customer.Id;
  }

  public async Task<Customer?> GetCustomerById(long id)
  {
    var customer = await DbContext.Customers.FirstOrDefaultAsync(customer => customer.Id == id);

    if (customer == null)
    {
      return null;
    }

    return customer;
  }

  public async Task<Customer?> GetCustomerByName(string name)
  {
    var customer = await DbContext.Customers.FirstOrDefaultAsync(customer => customer.Name == name);

    if (customer == null)
    {
      return null;
    }

    return customer;
  }

  public async Task<bool> IsEmailUnique(string email)
  {
    return await DbContext.Customers.AnyAsync(customer => customer.Email.Equals(email));
  }
}
