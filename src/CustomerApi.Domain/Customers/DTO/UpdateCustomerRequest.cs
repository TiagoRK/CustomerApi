namespace CustomerApi.Domain.Customers.DTO;
public record UpdateCustomerRequest
{
  public string Name { get; set; }
  public DateTime BirthDate { get; set; }
  public string Email { get; set; }
}
