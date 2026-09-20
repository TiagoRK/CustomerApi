using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using CustomerApi.Infrastructure.Data;
using CustomerApi.Domain.Customers;

namespace CustomerApi.IntegrationTests
{
    [TestFixture]
    public class AutoBoardAcceptance_7c7970731c0592b2
    {
        [Test]
        public async Task test_AutoBoardAcceptance_7c7970731c0592b2_C1()
        {
            // Part 1: Structural check — CustomerDbContext must not expose LogEntry in any form.
            var customerDbContextType = typeof(CustomerDbContext);

            var logEntryProperties = customerDbContextType
                .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                .Where(p =>
                    p.PropertyType.FullName != null &&
                    p.PropertyType.FullName.Contains("LogEntry"))
                .ToList();

            Assert.That(
                logEntryProperties,
                Is.Empty,
                $"CustomerDbContext must not expose any DbSet<LogEntry> property. Found: {string.Join(", ", logEntryProperties.Select(p => p.Name))}");

            var logEntryFields = customerDbContextType
                .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Where(f =>
                    f.FieldType.FullName != null &&
                    f.FieldType.FullName.Contains("LogEntry"))
                .ToList();

            Assert.That(
                logEntryFields,
                Is.Empty,
                $"CustomerDbContext must not have any LogEntry fields. Found: {string.Join(", ", logEntryFields.Select(f => f.Name))}");

            // Part 2: Behavioural check — Customer persisted without any LogEntry-related error.
            var options = new DbContextOptionsBuilder<CustomerDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Exception? thrownException = null;
            Customer? persisted = null;
            try
            {
                await using var db = new CustomerDbContext(options);

                // Use the correct constructor: Customer(string name, DateTime birthDate, string email)
                var customer = new Customer(
                    "Test Customer",
                    new DateTime(1990, 1, 1),
                    "test@example.com");

                db.Customers.Add(customer);
                await db.SaveChangesAsync();

                persisted = await db.Customers.FirstOrDefaultAsync(c => c.Name == "Test Customer");
            }
            catch (Exception ex)
            {
                thrownException = ex;
            }

            Assert.That(
                thrownException,
                Is.Null,
                $"Customer operation through CustomerDbContext raised an error: {thrownException?.Message}");

            Assert.That(persisted, Is.Not.Null, "Customer should be persisted through CustomerDbContext.");
            Assert.That(persisted!.Name, Is.EqualTo("Test Customer"));
            Assert.That(persisted.Email, Is.EqualTo("test@example.com"));
        }
    }
}
