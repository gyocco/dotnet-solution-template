using DemoProject.Data.Models;
using DemoProject.Data.Repository;
using DemoProject.Data.Implementation.Repository.Base;
using DemoProject.Data.Implementation.DbContext;

namespace DemoProject.Data.Implementation.Repository;

public class DemoRepository : BaseRepositoryWithSearch<Demo, int>, IDemoRepository
{
    public DemoRepository(ApplicationDbContext context) : base(context)
    {
    }

    // Override ApplyFilters for Demo-specific filtering if needed
    protected override IQueryable<Demo> ApplyFilters<TFilters>(IQueryable<Demo> query, TFilters filters)
    {
        // Example: If filters contain a Query property, filter by Name
        if (filters != null)
        {
            var filtersType = typeof(TFilters);
            var queryProperty = filtersType.GetProperty("Name");

            if (queryProperty != null)
            {
                var queryValue = queryProperty.GetValue(filters) as string;
                if (!string.IsNullOrWhiteSpace(queryValue))
                {
                    query = query.Where(d => d.Name.Contains(queryValue));
                }
            }
        }

        return query;
    }
}
