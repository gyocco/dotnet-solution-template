using DemoProject.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DemoProject.Data.TableConfigurations;

public class DemoTableConfiguration : IEntityTypeConfiguration<Demo>
{
  public void Configure(EntityTypeBuilder<Demo> builder)
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
