using DemoProject.Services.Shared.Models;

namespace DemoProject.Services.Modules.Demo.Requests;

public class SearchDemosRequest : SearchRequest<SearchDemoFilters>
{
}

public class SearchDemoFilters
{
  public string Name { get; set; }
}
