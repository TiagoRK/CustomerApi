using System;
using System.Linq;
using System.Threading.Tasks;
using CustomerApi.Infrastructure.Data;
using CustomerApi.Domain.Logging;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CustomerApi.IntegrationTests
{
    [TestFixture]
    public class AutoBoardAcceptance_8ab49b12aedcdc7a
    {
        [Test]
        public async Task test_AutoBoardAcceptance_8ab49b12aedcdc7a_C1()
        {
            // Arrange: build an in-memory LogDbContext
            var options = new DbContextOptionsBuilder<LogDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new LogDbContext(options);

            // Verify DbSet<LogEntry> is exposed
            Assert.That(context.LogEntries, Is.Not.Null,
                "LogDbContext must expose a DbSet<LogEntry> property.");

            // Verify the model contains LogEntry entity type (mapping applied)
            var entityTypes = context.Model.GetEntityTypes().Select(e => e.ClrType).ToList();
            Assert.That(entityTypes, Does.Contain(typeof(LogEntry)),
                "LogDbContext.OnModelCreating must apply LogEntry mapping so LogEntry appears in the model.");

            // Verify the model does NOT contain Customer entity type (no Customer mapping)
            var customerType = entityTypes.FirstOrDefault(t => t.Name == "Customer");
            Assert.That(customerType, Is.Null,
                "LogDbContext must NOT apply Customer mapping; Customer should not appear in the model.");

            // Discover the actual properties of LogEntry via reflection to build a valid fixture
            var logEntryType = typeof(LogEntry);
            var logEntryInstance = Activator.CreateInstance(logEntryType)!;

            // Set Id if available
            var idProp = logEntryType.GetProperty("Id");
            var entryId = Guid.NewGuid();
            if (idProp != null && idProp.CanWrite)
                idProp.SetValue(logEntryInstance, entryId);

            // Set a string property that is NOT named "Message" — find the first writable string prop
            var firstStringProp = logEntryType.GetProperties()
                .FirstOrDefault(p => p.PropertyType == typeof(string) && p.CanWrite);
            string? stringPropName = firstStringProp?.Name;
            string? stringPropValue = null;
            if (firstStringProp != null)
            {
                stringPropValue = "test log entry value";
                firstStringProp.SetValue(logEntryInstance, stringPropValue);
            }

            // Set DateTime props if available
            foreach (var prop in logEntryType.GetProperties()
                         .Where(p => (p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?)) && p.CanWrite))
            {
                prop.SetValue(logEntryInstance, DateTime.UtcNow);
            }

            var logEntry = (LogEntry)logEntryInstance;

            // Act: persist the LogEntry fixture
            await context.LogEntries.AddAsync(logEntry);
            var saved = await context.SaveChangesAsync();

            // Assert: row persisted without error
            Assert.That(saved, Is.GreaterThanOrEqualTo(1),
                "SaveChangesAsync must return at least 1 indicating the LogEntry row was persisted.");

            // Assert: row is retrievable
            LogEntry? persisted = null;
            if (idProp != null)
            {
                persisted = await context.LogEntries
                    .FirstOrDefaultAsync(e => e.Id == entryId);
            }
            else
            {
                persisted = await context.LogEntries.FirstOrDefaultAsync();
            }

            Assert.That(persisted, Is.Not.Null,
                "The persisted LogEntry must be retrievable via LogDbContext.LogEntries.");

            // Assert string field roundtrips correctly if present
            if (firstStringProp != null && stringPropValue != null)
            {
                var retrieved = firstStringProp.GetValue(persisted);
                Assert.That(retrieved, Is.EqualTo(stringPropValue),
                    $"Persisted LogEntry.{stringPropName} must match the fixture value.");
            }
        }
    }
}
