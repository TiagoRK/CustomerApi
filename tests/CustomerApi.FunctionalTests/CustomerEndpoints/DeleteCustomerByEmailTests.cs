using System.Net;
using Ardalis.HttpClientTestExtensions;

namespace CustomerApi.FunctionalTests.CustomerEndpoints;
public class DeleteCustomerByEmailTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task DeleteCustomer()
  {
    var result = await _client.DeleteAsync(
        $"/customers/deleteCustomerByEmail/{DataSeeder.Customers.First().Email}");

    var getResult = await _client.GetAndEnsureNotFoundAsync(
        $"/customers/getCustomerByEmail?email={DataSeeder.Customers.First().Email}");

    Assert.Multiple(() =>
    {
      Assert.Equal(HttpStatusCode.OK, result.StatusCode);
      Assert.Equal(HttpStatusCode.NotFound, getResult.StatusCode);
    });
  }

  [Fact]
  public async Task DeleteCustomer_CustomerNotFound()
  {
    var result = await _client.DeleteAsync(
        $"/customers/deleteCustomerByEmail/falseemail@email.com");

    Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
  }
}
