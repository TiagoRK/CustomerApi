namespace CustomerApi.Domain.Customers.DTO;
public record DeleteCustomerRequest
{
  public string Email { get; set; }
}
