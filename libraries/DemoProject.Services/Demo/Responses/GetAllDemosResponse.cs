namespace DemoProject.Services.Demo.Responses;

public class GetAllDemosResponse
{
  public List<DemoProject.Services._Shared.Responses.Demo> Demos { get; set; } = new();

  public class DemoResponse
  {
    public int DemoId { get; set; }
    public string Name { get; set; } = string.Empty;
  }
}
