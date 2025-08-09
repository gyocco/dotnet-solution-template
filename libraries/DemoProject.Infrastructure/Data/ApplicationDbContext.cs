using DemoProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DemoProject.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    // Apply all configurations from the assembly
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    base.OnModelCreating(modelBuilder);
  }

  public DbSet<OptionEntity> Options { get; set; }
  public DbSet<DemoEntity> Demos { get; set; }
}
