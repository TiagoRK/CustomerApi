using System.Net;
using System.Net.Http.Json;
using Ardalis.HttpClientTestExtensions;
using CustomerApi.Domain.Customers.DTO;

namespace CustomerApi.FunctionalTests.CustomerEndpoints;
public class UpdateCustomerTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task UpdateCustomer()
  {
    var request = new UpdateCustomerRequest()
    {
      Name = "name test",
      BirthDate = DateTime.Now.AddYears(-19),
      Email = "test@email.com"
    };

    var jsonContent = JsonContent.Create(request);

    var result = await _client.PutAndDeserializeAsync<GetCustomerResponse>(
      $"/customers/updateCustomer/{DataSeeder.Customers.First().Email}",
      jsonContent
    );

    Assert.Multiple(() =>
    {
      Assert.Equal(result.Name, request.Name);
      Assert.Equal(result.BirthDate.Year, request.BirthDate.Year);
      Assert.Equal(result.Email, request.Email);
    });
  }

  [Fact]
  public async Task UpdateCustomers_EnsureHttpStatusCode()
  {
    var request = new UpdateCustomerRequest()
    {
      Name = "name test",
      BirthDate = DateTime.Now.AddYears(-19),
      Email = "test2@email.com"
    };

    var jsonContent = JsonContent.Create(request);

    var result = await _client.PutAsync(
      $"/customers/updateCustomer/{DataSeeder.Customers.First().Email}",
      jsonContent
    );

    Assert.Equal(HttpStatusCode.OK, result.StatusCode);
  }

  [Fact]
  public async Task UpdateCustomers_BelowMinimumAge()
  {
    var request = new UpdateCustomerRequest()
    {
      Name = "name test",
      BirthDate = DateTime.Now,
      Email = "test@email.com"
    };

    var jsonContent = JsonContent.Create(request);

    var result = await _client.PutAsync(
      $"/customers/updateCustomer/{DataSeeder.Customers.First().Email}",
      jsonContent
    );

    Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
  }

  [Fact]
  public async Task UpdateCustomers_NonUniqueEmail()
  {
    var request = new UpdateCustomerRequest()
    {
      Name = "name test",
      BirthDate = DateTime.Now.AddYears(-19),
      Email = DataSeeder.Customers.First().Email
    };

    var jsonContent = JsonContent.Create(request);

    var result = await _client.PutAsync(
      $"/customers/updateCustomer/{DataSeeder.Customers.First().Email}",
      jsonContent
    );

    Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
  }
}
