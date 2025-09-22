using System.Net;
using Ardalis.HttpClientTestExtensions;
using CustomerApi.Domain.Customers.DTO;

namespace CustomerApi.FunctionalTests.CustomerEndpoints.GetCustomer;
public class GetCustomerByEmailTests(CustomWebApplicationFactory<Program> factory) : GetCustomerFixture(factory)
{
  [Fact]
  public async Task GetCustomer()
  {
    var result = await _client.GetAndDeserializeAsync<GetCustomerResponse>(
        $"/customers/getCustomerByEmail?email={DataSeeder.Customers.First().Email}");

    Assert.Multiple(() =>
    {
      Assert.Equal(result.Name, DataSeeder.Customers.First().Name);
      Assert.Equal(result.BirthDate.Year, DataSeeder.Customers.First().BirthDate.Year);
      Assert.Equal(result.Email, DataSeeder.Customers.First().Email);
    });
  }

  [Fact]
  public async Task GetCustomer_NotFound()
  {
    var result = await _client.GetAndEnsureNotFoundAsync(
        $"/customers/getCustomerByEmail?email=falseemail@email.com");

    Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
  }
}
