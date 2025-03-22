namespace CustomerApi.IntegrationTests.CustomerRepositoryTests.CustomerRepositoryDelete;
public class DeleteCustomerTests : CustomerRepositoryTestBase
{
  [Test]
  public async Task DeleteCustomer_Success()
  {
    var id = await _customerRepository.Create(_fakeCustomer);

    var customer = await _customerRepository.GetById(id);

    await _customerRepository.Delete(customer);

    var customerAfterDelete = await _customerRepository.GetById(id);

    Assert.That(customerAfterDelete, Is.Null);
  }

  [Test]
  public async Task DeleteMultipleCustomers_Success()
  {
    var (_, idList) = await CreateMultipleCustomer(9);

    var customersFromDb = await Task.WhenAll(idList.Select(id => _customerRepository.GetById(id)));

    foreach (var customerToDelete in customersFromDb)
    {
      await _customerRepository.Delete(customerToDelete);
    }

    var customersAfterDelete = await Task.WhenAll(idList.Select(id => _customerRepository.GetById(id)));

    Assert.That(customersAfterDelete, Has.All.Null);
  }
}
