using System.Net;
using System.Net.Http.Json;
using Ardalis.HttpClientTestExtensions;
using CustomerApi.Domain.Customers.DTO;

namespace CustomerApi.FunctionalTests.CustomerEndpoints;
public class CreateCustomerTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task CreateCustomer()
  {
    var request = new CreateCustomerRequest()
    {
      Name = "name test",
      BirthDate = DateTime.Now.AddYears(-19),
      Email = "test@email.com"
    };

    var jsonContent = JsonContent.Create(request);

    var result = await _client.PostAndDeserializeAsync<GetCustomerResponse>("/customers/createCustomer", jsonContent);

    Assert.Multiple(() =>
    {
      Assert.Equal(result.Name, request.Name);
      Assert.Equal(result.BirthDate.Year, request.BirthDate.Year);
      Assert.Equal(result.Email, request.Email);
    });
  }

  [Fact]
  public async Task CreateCustomer_EnsureHttpStatusCode()
  {
    var request = new CreateCustomerRequest()
    {
      Name = "name test",
      BirthDate = DateTime.Now.AddYears(-19),
      Email = "test2@email.com"
    };

    var jsonContent = JsonContent.Create(request);

    var result = await _client.PostAsync("/customers/createCustomer", jsonContent);

    Assert.Equal(HttpStatusCode.Created, result.StatusCode);
  }

  [Fact]
  public async Task CreateCustomer_BelowMinimumAge()
  {
    var request = new CreateCustomerRequest()
    {
      Name = "name test",
      BirthDate = DateTime.Now,
      Email = "test@email.com"
    };

    var jsonContent = JsonContent.Create(request);

    var result = await _client.PostAsync("/customers/createCustomer", jsonContent);

    Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
  }

  [Fact]
  public async Task CreateCustomer_NonUniqueEmail()
  {
    var request = new CreateCustomerRequest()
    {
      Name = "name test",
      BirthDate = DateTime.Now.AddYears(-19),
      Email = DataSeeder.Customers.First().Email
    };

    var jsonContent = JsonContent.Create(request);

    var result = await _client.PostAsync("/customers/createCustomer", jsonContent);

    Assert.Equal(HttpStatusCode.UnprocessableEntity, result.StatusCode);
  }
}
