namespace CustomerApi.Domain.Customers
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetCustomerByName(string name);
        Task<Customer?> GetCustomerById(long id);
        Task<long> CreateCustomer(Customer customer);
        Task<bool> IsEmailUnique(string email);
    }
}
