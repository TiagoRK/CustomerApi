namespace CustomerApi.IntegrationTests.CustomerRepositoryTests.CustomerRepositoryCreate;
public class CreateCustomerTests : CustomerRepositoryTestBase
{
  [Test]
  public async Task CreateCustomer_Success()
  {
    await _customerRepository.Create(_fakeCustomer);

    var customer = await _customerRepository.GetByEmail(_fakeCustomer.Email);

    Assert.Multiple(() =>
    {
      Assert.That(_fakeCustomer.Name, Is.EqualTo(customer.Name));
      Assert.That(_fakeCustomer.BirthDate, Is.EqualTo(customer.BirthDate));
      Assert.That(_fakeCustomer.Email, Is.EqualTo(customer.Email));
    });
  }
}
