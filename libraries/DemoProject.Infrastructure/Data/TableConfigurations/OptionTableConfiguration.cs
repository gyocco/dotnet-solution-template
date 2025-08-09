using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DemoProject.Domain.Entities;

namespace DemoProject.Infrastructure.Data.TableConfigurations;

public class OptionTableConfiguration : IEntityTypeConfiguration<OptionEntity>
{
  public void Configure(EntityTypeBuilder<OptionEntity> builder)
  {
    builder.ToTable("Option", "Shared");

    builder.HasKey(e => e.OptionId);

    builder.Property(e => e.OptionId)
      .ValueGeneratedOnAdd();

    builder.Property(e => e.Category)
      .IsRequired()
      .HasMaxLength(100)
      .HasDefaultValue(string.Empty);

    builder.Property(e => e.Code)
      .IsRequired()
      .HasMaxLength(100)
      .HasDefaultValue(string.Empty);

    builder.Property(e => e.Description)
      .IsRequired()
      .HasMaxLength(500)
      .HasDefaultValue(string.Empty);

    builder.Property(e => e.DisplayText)
      .IsRequired()
      .HasMaxLength(200)
      .HasDefaultValue(string.Empty);

    builder.Property(e => e.Order)
      .HasDefaultValue(0);

    builder.Property(e => e.IsActive)
      .HasDefaultValue(true);

    builder.Property(e => e.CreatedAt)
      .HasDefaultValueSql("GETUTCDATE()");

    builder.Property(e => e.UpdatedAt)
      .IsRequired(false);

    // Indexes
    builder.HasIndex(e => new { e.Category, e.Code })
      .IsUnique()
      .HasDatabaseName("IX_Option_Category_Code");

    builder.HasIndex(e => new { e.Category, e.Order })
      .HasDatabaseName("IX_Option_Category_Order");
  }
}
