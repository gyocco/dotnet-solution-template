using DemoProject.Data.Models;
using DemoProject.Data.Repository.Interfaces;
using DemoProject.Data.Repository.Implementation.Base;
using DemoProject.Data.DbContext;

namespace DemoProject.Data.Repository.Implementation;

public class DemoRepository : BaseRepositoryWithSearch<Demo, int, DemoSearchFilters>, IDemoRepository
{
    public DemoRepository(ApplicationDbContext context) : base(context)
    {
    }

    protected override IQueryable<Demo> ApplyFilters(IQueryable<Demo> query, DemoSearchFilters filters)
    {
        if (!string.IsNullOrWhiteSpace(filters.Name))
        {
            query = query.Where(d => d.Name.Contains(filters.Name));
        }
        return query;
    }
}

