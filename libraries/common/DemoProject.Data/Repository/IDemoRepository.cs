using DemoProject.Data.Models;
using DemoProject.Data.Repository.Base;

namespace DemoProject.Data.Repository;

public interface IDemoRepository : IRepositoryWithSearch<Demo, int, DemoSearchFilters>
{
}

public class DemoSearchFilters
{
  public string Name { get; set; }
}
