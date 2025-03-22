using CustomerApi.Domain.Customers;

namespace CustomerApi.UnitTests.Customers;
public abstract class CustomerTestBase : TestBase
{
  public Customer _fakeCustomer { get; set; }

  [SetUp]
  public new void Setup()
  {
    _fakeCustomer = new Customer(
        _faker.Name.FullName(),
        _faker.Date.Past(30, DateTime.Now.AddYears(-18)),
        _faker.Internet.Email()
      );
  }
}
