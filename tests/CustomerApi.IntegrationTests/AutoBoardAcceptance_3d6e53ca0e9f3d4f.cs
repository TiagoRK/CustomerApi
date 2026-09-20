using System;
using CustomerApi.Domain.Logging;
using CustomerApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace CustomerApi.IntegrationTests;

[TestFixture]
public class AutoBoardAcceptance_3d6e53ca0e9f3d4f
{
    [Test]
    public void test_AutoBoardAcceptance_3d6e53ca0e9f3d4f_C1()
    {
        var options = new DbContextOptionsBuilder<LogDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var ctx = new LogDbContext(options);

        // Use the first valid enum value from LogDirection (Request or Response)
        // as confirmed by the contract: enum LogDirection { Request, Response }
        var entry = new LogEntry
        {
            Id = Guid.NewGuid(),
            CorrelationId = Guid.NewGuid(),
            Endpoint = "/api/test",
            Method = "POST",
            Payload = "{\"key\":\"value\"}",
            StatusCode = 200,
            Direction = LogDirection.Request,
            CreatedAt = DateTimeOffset.UtcNow
        };

        Assert.DoesNotThrow(() =>
        {
            ctx.LogEntries.Add(entry);
            ctx.SaveChanges();
        });

        var saved = ctx.LogEntries.Find(entry.Id);
        Assert.That(saved, Is.Not.Null);
        Assert.That(saved!.CorrelationId, Is.EqualTo(entry.CorrelationId));
        Assert.That(saved.Endpoint, Is.EqualTo("/api/test"));
        Assert.That(saved.Method, Is.EqualTo("POST"));
        Assert.That(saved.Payload, Is.EqualTo("{\"key\":\"value\"}"));
        Assert.That(saved.StatusCode, Is.EqualTo(200));
        Assert.That(saved.Direction, Is.EqualTo(LogDirection.Request));
        Assert.That(saved.CreatedAt, Is.EqualTo(entry.CreatedAt));
    }
}
