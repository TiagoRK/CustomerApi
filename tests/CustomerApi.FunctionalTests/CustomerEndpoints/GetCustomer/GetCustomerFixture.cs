namespace CustomerApi.FunctionalTests.CustomerEndpoints.GetCustomer;
public abstract class GetCustomerFixture(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  public readonly HttpClient _client = factory.CreateClient();
}
