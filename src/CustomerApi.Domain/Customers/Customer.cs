using CustomerApi.SharedKernel;

namespace CustomerApi.Domain.Customers;

public sealed class Customer(string name, DateTime birthDate, string email) : Entity
{
  public string Name { get; set; } = name;
  public DateTime BirthDate { get; set; } = DateTime.SpecifyKind(birthDate, DateTimeKind.Utc);
  public string Email { get; set; } = email;
}
