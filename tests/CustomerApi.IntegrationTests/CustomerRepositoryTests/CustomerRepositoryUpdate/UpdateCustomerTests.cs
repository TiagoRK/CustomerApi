﻿namespace CustomerApi.IntegrationTests.CustomerRepositoryTests.CustomerRepositoryUpdate;
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

  [Test]
  public async Task UpdateCustomer_EmailNotUnique_Fail()
  {
    var id = await _customerRepository.Create(_fakeCustomer);

    var customer = await _customerRepository.GetById(id);

    var isUnique = await _customerRepository.IsEmailUnique(customer.Email);

    Assert.That(isUnique, Is.False);
  }
}
