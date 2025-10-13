namespace DemoProject.Data.Repository.Base;

public interface IRepository<TEntity, TKey> where TEntity : class
{
    Task<TEntity> GetById(TKey id);
    Task<IEnumerable<TEntity>> GetAll();
    Task Create(TEntity entity);
    Task Update(TEntity entity);
    Task Delete(TEntity entity);
}