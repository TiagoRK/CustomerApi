using CustomerApi.Domain.Logging;
using CustomerApi.Infrastructure.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Infrastructure.Data;

public class LogDbContext(DbContextOptions<LogDbContext> options) : DbContext(options)
{
  public DbSet<LogEntry> LogEntries { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfiguration(new LogEntryMapping());
  }
}
