using DemoProject.Data.Repository.Base;
using DemoProject.Data.Repository.Base.Models;
using DemoProject.Data.Implementation.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace DemoProject.Data.Implementation.Repository.Base;

public class BaseRepositoryWithSearch<TEntity, TKey>(ApplicationDbContext context) : BaseRepository<TEntity, TKey>(context), IRepositoryWithSearch<TEntity, TKey>
    where TEntity : class
{
    public virtual async Task<SearchResponse<TEntity>> Search<TFilters>(SearchRequest<TFilters> request)
    {
        var query = _dbSet.AsQueryable();

        // Apply filters if provided and entity has filterable properties
        if (request.Filters != null)
        {
            query = ApplyFilters(query, request.Filters);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply ordering
        if (!string.IsNullOrWhiteSpace(request.OrderByColumn))
        {
            query = ApplyOrdering(query, request.OrderByColumn, request.OrderDescending);
        }

        // Apply pagination
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new SearchResponse<TEntity>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    protected virtual IQueryable<TEntity> ApplyFilters<TFilters>(IQueryable<TEntity> query, TFilters filters)
    {
        // This is a base implementation that can be overridden in derived classes
        // for entity-specific filtering logic
        return query;
    }

    protected virtual IQueryable<TEntity> ApplyOrdering(IQueryable<TEntity> query, string orderByColumn, bool descending)
    {
        if (string.IsNullOrWhiteSpace(orderByColumn))
            return query;

        try
        {
            var entityType = typeof(TEntity);
            var property = entityType.GetProperty(orderByColumn, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property == null)
                return query;

            var parameter = Expression.Parameter(entityType, "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            var methodName = descending ? "OrderByDescending" : "OrderBy";
            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { entityType, property.PropertyType },
                query.Expression,
                Expression.Quote(orderByExpression));

            return query.Provider.CreateQuery<TEntity>(resultExpression);
        }
        catch
        {
            // If ordering fails, return the original query
            return query;
        }
    }
}
