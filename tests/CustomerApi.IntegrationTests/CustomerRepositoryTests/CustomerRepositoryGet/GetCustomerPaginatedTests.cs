namespace CustomerApi.IntegrationTests.CustomerRepositoryTests.CustomerRepositoryGet;
public class GetCustomerPaginatedTests : CustomerRepositoryTestBase
{
  [Test]
  public async Task GetByCustomerPaginated_Success()
  {
    var (customersToCreate, idList) = await CreateMultipleCustomer(9);

    var customerPaged = await _customerRepository.GetPaginated(10, 1);

    for (var i = 0; i < customersToCreate.Count; i++)
    {
      var expected = customersToCreate[i];
      var actual = customerPaged.Data[i];

      Assert.Multiple(() =>
      {
        Assert.That(actual.Id, Is.EqualTo(idList[i]));
        Assert.That(actual.Name, Is.EqualTo(expected.Name));
        Assert.That(actual.BirthDate, Is.EqualTo(expected.BirthDate));
        Assert.That(actual.Email, Is.EqualTo(expected.Email));
      });
    }

    Assert.Multiple(() =>
    {
      Assert.That(customerPaged.PageSize, Is.EqualTo(10));
      Assert.That(customerPaged.PageNumber, Is.EqualTo(1));
      Assert.That(customerPaged.TotalPages, Is.EqualTo(1));
      Assert.That(customerPaged.TotalRecords, Is.EqualTo(10));
    });
  }
}
