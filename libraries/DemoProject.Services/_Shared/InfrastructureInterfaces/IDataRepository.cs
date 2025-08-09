using System.Linq.Expressions;
using DemoProject.Services._Shared.Responses;

namespace DemoProject.Services._Shared.InfrastructureInterfaces;

public interface IDataRepository<T> where T : class
{
  Task<T> GetById(object id);
  Task<IEnumerable<T>> GetAll();
  Task<IEnumerable<T>> Get(Expression<Func<T, bool>> predicate);

  Task Add(T entity);
  Task AddRange(IEnumerable<T> entities);

  Task Remove(T entity);
  Task RemoveRange(IEnumerable<T> entities);

  Task Update(T entity);

  Task<PageResponse<T>> Search(
      Expression<Func<T, bool>> filter = null,
      Expression<Func<T, object>> orderBy = null,
      bool orderByDescending = false,
      int pageNumber = 1,
      int pageSize = 10
  );

  Task BeginTransaction();
  Task CommitTransaction();
  Task RollbackTransaction();
}