using DemoProject.Data.Models;
using DemoProject.Data.Repository.Interfaces.Base;

namespace DemoProject.Data.Repository.Interfaces;

public interface IDemoRepository : IRepositoryWithSearch<Demo, int, DemoSearchFilters>
{
}

public class DemoSearchFilters
{
  public string Name { get; set; }
}
