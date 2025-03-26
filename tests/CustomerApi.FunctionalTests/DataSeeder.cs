using CustomerApi.Domain.Customers;
using CustomerApi.Infrastructure.Data;

namespace CustomerApi.FunctionalTests;
public static class DataSeeder
{
  public static readonly List<Customer> Customers = [];

  public static async Task PopulateTestData(CustomerDbContext dbContext, int customerToCreateCount)
  {
    for (var i = 0; i <= customerToCreateCount; i++)
    {
      var newCustomer = new Customer(
          $"Nome Teste {i}",
          DateTime.Now.AddYears(-18),
          $"emailTeste{i}@gmail.com"
      );

      Customers.Add(newCustomer);
    }

    dbContext.Customers.AddRange(Customers);

    await dbContext.SaveChangesAsync();
  }
}
