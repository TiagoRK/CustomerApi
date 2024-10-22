using CustomerApi.Domain.Customers;
using CustomerApi.Infrastructure.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Infrastructure.Data
{
    public class CustomerDbContext(DbContextOptions<CustomerDbContext> options) : DbContext(options)
    {
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerMapping());
        }
    }
}
