using CustomerApi.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerApi.Infrastructure.Data.Mappings
{
    public class CustomerMapping : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> customer)
        {
            customer.ToTable("Customer");

            customer.HasKey(p => p.Id);

            customer.Property(p => p.Id).ValueGeneratedOnAdd();
            customer.Property(p => p.Name);
            customer.Property(p => p.BirthDate);
        }
    }
}
