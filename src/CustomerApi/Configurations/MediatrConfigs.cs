using System.Reflection;
using CustomerApi.Application.Commands.Customers.Create;
using CustomerApi.Domain.Customers;

namespace CustomerApi.Web.Configurations;

public static class MediatrConfigs
{
  public static IServiceCollection AddMediatrConfigs(this IServiceCollection services)
  {
    var mediatRAssemblies = new[]
      {
        Assembly.GetAssembly(typeof(Customer)),
        Assembly.GetAssembly(typeof(CreateCustomerCommand))
      };

    services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies!));

    return services;
  }
}
