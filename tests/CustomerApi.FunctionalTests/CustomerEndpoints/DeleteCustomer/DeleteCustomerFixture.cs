namespace CustomerApi.FunctionalTests.CustomerEndpoints.DeleteCustomer;
public abstract class DeleteCustomerFixture(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  public readonly HttpClient _client = factory.CreateClient();
}

