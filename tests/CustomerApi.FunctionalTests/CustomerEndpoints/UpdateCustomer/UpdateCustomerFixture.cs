namespace CustomerApi.FunctionalTests.CustomerEndpoints.UpdateCustomer;
public abstract class UpdateCustomerFixture(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  public readonly HttpClient _client = factory.CreateClient();
}
