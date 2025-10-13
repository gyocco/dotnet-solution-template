namespace DemoProject.Services.Shared.Models;

public class SearchRequest<TFilters>
{
  public TFilters Filters { get; set; } = default!;
  public string OrderBy { get; set; }
  public bool OrderByDescending { get; set; } = false;
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
}