namespace DemoProject.Services._Shared.Requests;
public class SearchRequest
{
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  public string OrderBy { get; set; } = string.Empty;
  public bool OrderByDescending { get; set; } = false;
}
