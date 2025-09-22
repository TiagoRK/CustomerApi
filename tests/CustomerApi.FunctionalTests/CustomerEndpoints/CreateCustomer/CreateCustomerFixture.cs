namespace CustomerApi.FunctionalTests.CustomerEndpoints.CreateCustomer;
public abstract class CreateCustomerFixture(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  public readonly HttpClient _client = factory.CreateClient();
}
