namespace DemoProject.Data.Repository.Base.Models;

public class SearchRequest<TFilters>
{
    public TFilters Filters { get; set; } = default!;
    public string OrderByColumn { get; set; }
    public bool OrderDescending { get; set; } = false;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}