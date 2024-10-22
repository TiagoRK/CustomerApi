using CustomerApi.SharedKernel;

namespace CustomerApi.Domain.Customers
{
    public static class CustomerErrors
    {
        public static Error EmailIsNotUnique() => new(
            "Customer.EmailIsNotUnique", $"The provided email is not unique");
        public static Error BirthDateIsNotValid() => new(
            "Customer.BirthDateIsNotValid", "Customer must be at least 18 years old.");
    }
}
