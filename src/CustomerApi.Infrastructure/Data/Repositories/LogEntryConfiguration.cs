using CustomerApi.Domain.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerApi.Infrastructure.Data.Configurations;

public class LogEntryConfiguration : IEntityTypeConfiguration<LogEntry>
{
  public void Configure(EntityTypeBuilder<LogEntry> builder)
  {
    builder.ToTable("LogEntries");
    builder.HasKey(x => x.Id);
    builder.Property(x => x.Endpoint).HasMaxLength(512).IsRequired();
    builder.Property(x => x.Method).HasMaxLength(10).IsRequired();
    builder.Property(x => x.Payload).HasColumnType("text");
    builder.Property(x => x.Direction).HasConversion<string>().HasMaxLength(20);
  }
}
