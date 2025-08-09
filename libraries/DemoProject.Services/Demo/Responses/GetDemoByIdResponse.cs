namespace DemoProject.Services.Demo.Responses;

public class GetDemoByIdResponse
{
  public DemoProject.Services._Shared.Responses.Demo Demo { get; set; }

  public class DemoResponse
  {
    public int DemoId { get; set; }
    public string Name { get; set; } = string.Empty;
  }
}
