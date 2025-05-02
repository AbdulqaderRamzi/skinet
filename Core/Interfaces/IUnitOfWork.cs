using Core.Entities;

namespace Core.Interfaces;

public interface IUnitOfWork : IDisposable // no need for it
{
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
    Task<bool> CompleteAsync();
}