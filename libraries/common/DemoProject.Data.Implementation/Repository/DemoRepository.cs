using DemoProject.Data.Models;
using DemoProject.Data.Repository;
using DemoProject.Data.Implementation.Repository.Base;
using DemoProject.Data.Implementation.DbContext;

namespace DemoProject.Data.Implementation.Repository;

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

