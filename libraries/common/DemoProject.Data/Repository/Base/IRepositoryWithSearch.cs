using DemoProject.Data.Repository.Base.Models;

namespace DemoProject.Data.Repository.Base;

public interface IRepositoryWithSearch<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
{
    Task<SearchResponse<TEntity>> Search<TFilters>(SearchRequest<TFilters> request);
}