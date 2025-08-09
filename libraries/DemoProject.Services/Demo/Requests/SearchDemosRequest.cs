using DemoProject.Services._Shared.Requests;

namespace DemoProject.Services.Demo.Requests;

public class SearchDemosRequest : SearchRequest
{
  public string Query { get; set; } = string.Empty;
}
