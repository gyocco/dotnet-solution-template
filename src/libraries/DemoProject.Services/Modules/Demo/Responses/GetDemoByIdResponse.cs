namespace DemoProject.Services.Modules.Demo.Responses;

public class GetDemoByIdResponse
{
  public Demo Demo { get; set; }

  public class DemoResponse
  {
    public int DemoId { get; set; }
    public string Name { get; set; } = string.Empty;
  }
}
