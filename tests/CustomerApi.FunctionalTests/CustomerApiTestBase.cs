using Ardalis.HttpClientTestExtensions;
using CustomerApi.Domain.Customers.DTO;
using CustomerApi.SharedKernel;

namespace CustomerApi.FunctionalTests;

public class CustomerApiTestBase(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task GetPagedCustomers()
  {
    var result = await _client.GetAndDeserializeAsync<PagedResponse<GetCustomerResponse>>(
        "/customers/getCustomerPaged?pageSize=10&pageNumber=1");

    Assert.Equal(10, result.Data.Count);

    for (var i = 0; i < result.Data.Count; i++)
    {
      Assert.Multiple(() =>
      {
        Assert.Equal(result.Data[i].Name, DataSeeder.Customers[i].Name);
        Assert.Equal(result.Data[i].BirthDate.Year, DataSeeder.Customers[i].BirthDate.Year);
        Assert.Equal(result.Data[i].Email, DataSeeder.Customers[i].Email);
      });
    }
  }
}


