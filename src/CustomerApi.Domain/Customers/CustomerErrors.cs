using CustomerApi.SharedKernel;
using System.Globalization;
using System.Resources;

namespace CustomerApi.Domain.Customers;

public static class CustomerErrors
{
    private static readonly ResourceManager _resourceManager =
        new ResourceManager(
            "CustomerApi.Domain.Resources.CustomerErrors",
            typeof(CustomerErrors).Assembly);

    public static Error EmailIsNotUnique() => new(
        "Customer.EmailIsNotUnique",
        _resourceManager.GetString(nameof(EmailIsNotUnique), CultureInfo.CurrentUICulture)
            ?? "The provided email is not unique.");

    public static Error BirthDateIsNotValid() => new(
        "Customer.BirthDateIsNotValid",
        _resourceManager.GetString(nameof(BirthDateIsNotValid), CultureInfo.CurrentUICulture)
            ?? "Customer must be at least 18 years old.");

    public static Error CustomerWithEmailNotFound(string email) => new(
        "Customer.CustomerWithEmailNotFound",
        string.Format(
            CultureInfo.CurrentUICulture,
            _resourceManager.GetString(nameof(CustomerWithEmailNotFound), CultureInfo.CurrentUICulture)
                ?? "Customer with {0} was not found.",
            email));
}
