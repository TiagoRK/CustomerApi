namespace CustomerApi.IntegrationTests.CustomerRepositoryTests.CustomerRepositoryUpdate;
public class UpdateCustomerTests : CustomerRepositoryTestBase
{
  [Test]
  public async Task UpdateCustomer_Success()
  {
    var id = await _customerRepository.Create(_fakeCustomer);

    var customer = await _customerRepository.GetById(id);

    customer.UpdateName("test");
    customer.UpdateBirthdate(DateTime.Now.AddYears(-18));
    customer.UpdateEmail("test@email.com");


    await _customerRepository.Update(customer);

    var customerAfterUpdate = await _customerRepository.GetById(id);

    Assert.Multiple(() =>
    {
      Assert.That(customerAfterUpdate.Name, Is.EqualTo("test"));
      Assert.That(customerAfterUpdate.BirthDate.Year, Is.EqualTo(DateTime.Now.AddYears(-18).Year));
      Assert.That(customerAfterUpdate.Email, Is.EqualTo("test@email.com"));
    });
  }

  // TODO
  [Test]
  public async Task UpdateMultipleCustomers_Success()
  {
    var (_, idList) = await CreateMultipleCustomer(9);

    var customersFromDb = await Task.WhenAll(idList.Select(id => _customerRepository.GetById(id)));

    foreach (var customerToUpdate in customersFromDb)
    {
      customerToUpdate.UpdateName(_faker.Name.FullName());
      customerToUpdate.UpdateBirthdate(_faker.Date.Past(30, DateTime.Now.AddYears(-18)));
      customerToUpdate.UpdateEmail(_faker.Internet.Email());

      await _customerRepository.Update(customerToUpdate);
    }

    var customersAfterUpdate = await Task.WhenAll(idList.Select(id => _customerRepository.GetById(id)));

    for (var i = 0; i < customersFromDb.Length; i++)
    {
      var afterUpdate = customersAfterUpdate[i];
      var beforeUpdate = customersFromDb[i];

      Assert.Multiple(() =>
      {
        Assert.That(afterUpdate.Name, Is.Not.EqualTo(beforeUpdate.Name));
        Assert.That(afterUpdate.BirthDate, Is.Not.EqualTo(beforeUpdate.BirthDate));
        Assert.That(afterUpdate.Email, Is.Not.EqualTo(beforeUpdate.Email));
      });
    }
  }

  [Test]
  public async Task UpdateCustomer_EmailNotUnique_Fail()
  {
    var id = await _customerRepository.Create(_fakeCustomer);

    var customer = await _customerRepository.GetById(id);

    var isUnique = await _customerRepository.IsEmailUnique(customer.Email);

    Assert.That(isUnique, Is.False);
  }
}
