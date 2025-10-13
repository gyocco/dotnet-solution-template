namespace DemoProject.Services.Modules.Demo.Responses;

public class SearchDemosResponse
{
  public List<Demo> Results { get; set; } = new();
  public int PageNumber { get; set; }
  public int PageSize { get; set; }
  public int TotalItems { get; set; }
  public int TotalPages { get; set; }

  public class DemoResponse
  {
    public int DemoId { get; set; }
    public string Name { get; set; } = string.Empty;
  }
}
