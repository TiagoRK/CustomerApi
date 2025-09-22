using System.Net;
using Ardalis.HttpClientTestExtensions;
using CustomerApi.Domain.Customers.DTO;
using CustomerApi.SharedKernel;

namespace CustomerApi.FunctionalTests.CustomerEndpoints.GetCustomer;

public class GetCustomerPagedTests(CustomWebApplicationFactory<Program> factory) : GetCustomerFixture(factory)
{
  [Fact]
  public async Task GetPagedCustomers()
  {
    var result = await _client.GetAndDeserializeAsync<PagedResponse<GetCustomerResponse>>(
        "/customers/getCustomerPaged?pageSize=10&pageNumber=1");

    Assert.Equal(10, result.Data.Count);
    Assert.Equal(10, result.PageSize);
    Assert.Equal(1, result.PageNumber);


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

  [Fact]
  public async Task GetPagedCustomers_PageSizeEleven()
  {
    var result = await _client.GetAndDeserializeAsync<PagedResponse<GetCustomerResponse>>(
        "/customers/getCustomerPaged?pageSize=11&pageNumber=1");

    Assert.Equal(11, result.Data.Count);
    Assert.Equal(11, result.PageSize);
    Assert.Equal(1, result.PageNumber);


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

  [Fact]
  public async Task GetPagedCustomers_PageWithoutData()
  {
    var result = await _client.GetAsync("/customers/getCustomerPaged?pageSize=10&pageNumber=3");

    Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
  }
}


