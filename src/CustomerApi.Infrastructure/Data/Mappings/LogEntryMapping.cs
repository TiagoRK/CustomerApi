using CustomerApi.Domain.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerApi.Infrastructure.Data.Mappings;

public class LogEntryMapping : IEntityTypeConfiguration<LogEntry>
{
  public void Configure(EntityTypeBuilder<LogEntry> builder)
  {
    builder.ToTable("LogEntries");

    builder.HasKey(e => e.Id);

    builder.Property(e => e.Id)
           .HasColumnName("Id")
           .IsRequired();

    builder.Property(e => e.CorrelationId)
           .HasColumnName("CorrelationId");

    builder.Property(e => e.Endpoint)
           .HasColumnName("Endpoint")
           .IsRequired()
           .HasMaxLength(500);

    builder.Property(e => e.Method)
           .HasColumnName("Method")
           .IsRequired()
           .HasMaxLength(10);

    builder.Property(e => e.Payload)
           .HasColumnName("Payload");

    builder.Property(e => e.StatusCode)
           .HasColumnName("StatusCode");

    builder.Property(e => e.Direction)
           .HasColumnName("Direction")
           .IsRequired();

    builder.Property(e => e.CreatedAt)
           .HasColumnName("CreatedAt")
           .IsRequired();
  }
}
