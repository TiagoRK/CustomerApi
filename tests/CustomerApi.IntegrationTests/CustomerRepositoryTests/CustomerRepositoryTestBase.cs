using Bogus;
using CustomerApi.Domain.Customers;
using CustomerApi.Infrastructure.Data;
using CustomerApi.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerApi.IntegrationTests.CustomerRepositoryTests;
public abstract class CustomerRepositoryTestBase
{
  protected CustomerDbContext _dbContext;
  protected CustomerRepository _customerRepository;
  protected Customer _fakeCustomer { get; set; }
  protected Faker _faker;

  public CustomerRepositoryTestBase()
  {
    var options = GetInMemoryContextOptions();

    _dbContext = new CustomerDbContext(options);
    _faker = new Faker("pt_BR");
  }

  [SetUp]
  public void Setup()
  {
    _customerRepository = new CustomerRepository(_dbContext);

    _fakeCustomer = new Customer(
      _faker.Name.FullName(),
      _faker.Date.Past(30, DateTime.Now.AddYears(-18)),
      _faker.Internet.Email()
    );
  }

  protected async Task<(List<Customer>, List<long>)> CreateMultipleCustomer(int customerToCreateCount)
  {
    var customersToCreate = new List<Customer>();
    var idList = new List<long>();

    for (var i = 0; i <= customerToCreateCount; i++)
    {
      var newCustomer = new Customer(
          _faker.Name.FullName(),
          _faker.Date.Past(30, DateTime.Now.AddYears(-18)),
          _faker.Internet.Email()
      );

      customersToCreate.Add(newCustomer);
    }

    foreach (var customerToCreate in customersToCreate)
    {
      var id = await _customerRepository.Create(customerToCreate);
      idList.Add(id);
    }

    return (customersToCreate, idList);
  }

  private static DbContextOptions<CustomerDbContext> GetInMemoryContextOptions()
  {
    var serviceProvider = new ServiceCollection()
        .AddEntityFrameworkInMemoryDatabase()
        .BuildServiceProvider();

    var builder = new DbContextOptionsBuilder<CustomerDbContext>();

    var inMemoryCollectionName = Guid.NewGuid().ToString();

    builder.UseInMemoryDatabase(inMemoryCollectionName)
           .UseInternalServiceProvider(serviceProvider);

    return builder.Options;
  }
}
