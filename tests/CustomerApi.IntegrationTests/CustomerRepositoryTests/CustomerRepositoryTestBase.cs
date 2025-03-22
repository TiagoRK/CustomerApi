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
  public Customer _fakeCustomer { get; set; }
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

  private static DbContextOptions<CustomerDbContext> GetInMemoryContextOptions()
  {
    var serviceProvider = new ServiceCollection()
        .AddEntityFrameworkInMemoryDatabase()
        .BuildServiceProvider();

    var builder = new DbContextOptionsBuilder<CustomerDbContext>();
    builder.UseInMemoryDatabase("mockdatabase")
           .UseInternalServiceProvider(serviceProvider);

    return builder.Options;
  }
}
