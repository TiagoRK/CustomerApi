using CustomerApi.Domain.Customers;
using CustomerApi.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Infrastructure.Data.Repositories;

public class CustomerRepository(CustomerDbContext dbContext) : ICustomerRepository
{
  private readonly CustomerDbContext _dbContext = dbContext;

  public async Task<long> Create(Customer customer)
  {
    await _dbContext.Customers.AddAsync(customer);
    await _dbContext.SaveChangesAsync();

    return customer.Id;
  }

  public Task<Customer?> GetById(long id, bool noTracking = false)
  {
    var query = _dbContext.Customers.AsQueryable();

    if (noTracking)
      query = query.AsNoTracking();

    return query.FirstOrDefaultAsync(customer => customer.Id == id);
  }

  public Task<Customer?> GetByEmail(string email, bool noTracking = false)
  {
    var query = _dbContext.Customers.AsQueryable();

    if (noTracking)
      query = query.AsNoTracking();

    return query.FirstOrDefaultAsync(customer => customer.Email == email);
  }

  public async Task<bool> IsEmailUnique(string email)
  {
    return !await _dbContext.Customers.AsNoTracking().AnyAsync(customer => customer.Email.Equals(email));
  }

  public async Task<PagedResponse<Customer>> GetPaginated(int pageSize, int pageNumber)
  {
    var totalRecords = await _dbContext.Customers.AsNoTracking().CountAsync();

    var customers = await _dbContext.Customers.AsNoTracking()
        .OrderBy(x => x.Id)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    var pagedResponse = new PagedResponse<Customer>(customers, pageNumber, pageSize, totalRecords);

    return pagedResponse;
  }

  public async Task Delete(Customer customer)
  {
    _dbContext.Customers.Remove(customer);
    await _dbContext.SaveChangesAsync();
  }

  public async Task Update(Customer customer)
  {
    _dbContext.Customers.Update(customer);
    await _dbContext.SaveChangesAsync();
  }
}
