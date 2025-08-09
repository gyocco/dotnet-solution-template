using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DemoProject.Domain.Entities;

namespace DemoProject.Infrastructure.Data.TableConfigurations;

public class DemoTableConfiguration : IEntityTypeConfiguration<DemoEntity>
{
  public void Configure(EntityTypeBuilder<DemoEntity> builder)
  {
    builder.ToTable("Demo", "Demo");

    builder.HasKey(e => e.DemoId);

    builder.Property(e => e.DemoId)
      .ValueGeneratedOnAdd();

    builder.Property(e => e.Name)
      .IsRequired()
      .HasMaxLength(100);
  }
}
