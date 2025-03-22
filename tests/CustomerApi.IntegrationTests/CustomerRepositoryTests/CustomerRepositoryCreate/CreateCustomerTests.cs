namespace CustomerApi.IntegrationTests.CustomerRepositoryTests.CustomerRepositoryCreate;
public class CreateCustomerTests : CustomerRepositoryTestBase
{
  [Test]
  public async Task CreateCustomer_Success()
  {
    var id = await _customerRepository.Create(_fakeCustomer);

    var customer = await _customerRepository.GetById(id);

    Assert.Multiple(() =>
    {
      Assert.That(_fakeCustomer.Id, Is.EqualTo(id));
      Assert.That(_fakeCustomer.Name, Is.EqualTo(customer.Name));
      Assert.That(_fakeCustomer.BirthDate, Is.EqualTo(customer.BirthDate));
      Assert.That(_fakeCustomer.Email, Is.EqualTo(customer.Email));
    });
  }

  [Test]
  public async Task CreateMultipleCustomers_Success()
  {
    var (customersToCreate, idList) = await CreateMultipleCustomer(9);

    var customersFromDb = await Task.WhenAll(idList.Select(id => _customerRepository.GetById(id)));

    Assert.That(customersFromDb, Has.Length.EqualTo(customersToCreate.Count));

    for (var i = 0; i < customersToCreate.Count; i++)
    {
      var expected = customersToCreate[i];
      var actual = customersFromDb[i];

      Assert.Multiple(() =>
      {
        Assert.That(actual.Id, Is.EqualTo(idList[i]));
        Assert.That(actual.Name, Is.EqualTo(expected.Name));
        Assert.That(actual.BirthDate, Is.EqualTo(expected.BirthDate));
        Assert.That(actual.Email, Is.EqualTo(expected.Email));
      });
    }
  }
}
