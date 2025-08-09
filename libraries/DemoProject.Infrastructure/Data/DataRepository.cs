using System.Linq.Expressions;
using DemoProject.Services._Shared.InfrastructureInterfaces;
using DemoProject.Services._Shared.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DemoProject.Infrastructure.Data;

public class DataRepository<T> : IDataRepository<T> where T : class
{
  private readonly ApplicationDbContext _context;
  private IDbContextTransaction _transaction;

  public DataRepository(ApplicationDbContext context)
  {
    _context = context;
  }

  public async Task<T> GetById(object id)
  {
    return await _context.Set<T>().FindAsync(id);
  }

  public async Task<IEnumerable<T>> GetAll()
  {
    return await _context.Set<T>().ToListAsync();
  }

  public async Task<IEnumerable<T>> Get(Expression<Func<T, bool>> predicate)
  {
    return await _context.Set<T>().Where(predicate).ToListAsync();
  }

  public async Task<PageResponse<T>> Search(Expression<Func<T, bool>> filter = null, Expression<Func<T, object>> orderBy = null, bool orderByDescending = false, int pageNumber = 1, int pageSize = 10)
  {
    if (pageNumber <= 0) pageNumber = 1;
    if (pageSize <= 0) pageSize = 10;

    IQueryable<T> query = _context.Set<T>();

    if (filter != null)
    {
      query = query.Where(filter);
    }

    int totalItems = await query.CountAsync();

    if (orderBy != null)
    {
      query = orderByDescending
          ? query.OrderByDescending(orderBy)
          : query.OrderBy(orderBy);
    }

    var items = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PageResponse<T>
    {
      Items = items,
      PageNumber = pageNumber,
      PageSize = pageSize,
      TotalItems = totalItems
    };
  }

  public async Task Add(T entity)
  {
    await _context.Set<T>().AddAsync(entity);

    if (_transaction == null)
    {
      await _context.SaveChangesAsync();
    }
  }

  public async Task AddRange(IEnumerable<T> entities)
  {
    await _context.Set<T>().AddRangeAsync(entities);

    if (_transaction == null)
    {
      await _context.SaveChangesAsync();
    }
  }

  public async Task Remove(T entity)
  {
    _context.Set<T>().Remove(entity);

    if (_transaction == null)
    {
      await _context.SaveChangesAsync();
    }
  }

  public async Task RemoveRange(IEnumerable<T> entities)
  {
    _context.Set<T>().RemoveRange(entities);

    if (_transaction == null)
    {
      await _context.SaveChangesAsync();
    }
  }

  public async Task Update(T entity)
  {
    _context.Set<T>().Update(entity);

    if (_transaction == null)
    {
      await _context.SaveChangesAsync();
    }
  }

  public async Task BeginTransaction()
  {
    if (_transaction == null)
      _transaction = await _context.Database.BeginTransactionAsync();
  }

  public async Task CommitTransaction()
  {
    try
    {
      await _context.SaveChangesAsync();
      if (_transaction != null)
        await _transaction.CommitAsync();
    }
    finally
    {
      if (_transaction != null)
      {
        await _transaction.DisposeAsync();
        _transaction = null;
      }
    }
  }

  public async Task RollbackTransaction()
  {
    if (_transaction != null)
    {
      await _transaction.RollbackAsync();
      await _transaction.DisposeAsync();
      _transaction = null;
    }
  }
}