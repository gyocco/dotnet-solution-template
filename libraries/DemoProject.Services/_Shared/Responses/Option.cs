namespace DemoProject.Services._Shared.Responses;

public class Option
{
  public int OptionId { get; set; }
  public string Category { get; set; }
  public string Code { get; set; }
  public string Description { get; set; }
  public string DisplayText { get; set; }
  public int Order { get; set; }
  public bool IsActive { get; set; }
  public DateTime CreatedAt { get; set; }
  public DateTime? UpdatedAt { get; set; }
}
