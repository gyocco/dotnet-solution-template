namespace DemoProject.Data.Repository.Base;

public interface IUnitOfWork : IDisposable
{
    IDemoRepository Demos { get; }
    Task BeginTransaction();
    Task CommitTransaction();
    Task RollbackTransaction();
}