using DemoProject.Data.Repository.Interfaces;
using DemoProject.Data.Repository.Interfaces.Base;
using DemoProject.Data.DbContext;
using Microsoft.EntityFrameworkCore.Storage;

namespace DemoProject.Data.Repository.Implementation.Base;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction _transaction;
    private IDemoRepository _demos;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IDemoRepository Demos => _demos ??= new DemoRepository(_context);

    public async Task BeginTransaction()
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Transaction is already started.");
        }

        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransaction()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction to commit.");
        }

        try
        {
            await _context.SaveChangesAsync();
            await _transaction.CommitAsync();
        }
        catch
        {
            await _transaction.RollbackAsync();
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransaction()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction to rollback.");
        }

        try
        {
            await _transaction.RollbackAsync();
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context?.Dispose();
    }
}