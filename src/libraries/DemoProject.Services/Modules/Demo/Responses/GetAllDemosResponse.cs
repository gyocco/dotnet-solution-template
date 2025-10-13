namespace DemoProject.Services.Modules.Demo.Responses;

public class GetAllDemosResponse
{
  public List<Demo> Demos { get; set; } = new();

  public class DemoResponse
  {
    public int DemoId { get; set; }
    public string Name { get; set; } = string.Empty;
  }
}
