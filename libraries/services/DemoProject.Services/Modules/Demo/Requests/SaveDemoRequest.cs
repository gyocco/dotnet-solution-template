using System.ComponentModel.DataAnnotations;

namespace DemoProject.Services.Modules.Demo.Requests;

public class SaveDemoRequest
{
  [Required]
  [StringLength(100)]
  public string Name { get; set; }
}