using DemoProject.Data.Repository.Interfaces.Base.Models;

namespace DemoProject.Data.Repository.Interfaces.Base;

public interface IRepositoryWithSearch<TEntity, TKey, TSearchFilters> : IRepository<TEntity, TKey> where TEntity : class
{
    Task<SearchResponse<TEntity>> Search(SearchRequest<TSearchFilters> request);
}