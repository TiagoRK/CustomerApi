namespace CustomerApi.Domain.Customers.DTO;
public record GetCustomerResponse
{
  public long Id { get; set; }
  public string Name { get; set; }
  public DateTime BirthDate { get; set; }
  public string Email { get; set; }
}
