using System.Collections.Concurrent;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data;

public class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    private readonly ConcurrentDictionary<string, object> _repositories = new();
    
    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        var type = typeof(TEntity).Name;
        return (IGenericRepository<TEntity>)_repositories.GetOrAdd(type, t =>
        {
            var repositoryType = typeof(GenericRepository<>).MakeGenericType(typeof(TEntity));
            return Activator.CreateInstance(repositoryType, db) 
                ?? throw new InvalidOperationException($"Cannot create repository instance for type {type}");
        });
    }

    public async Task<bool> CompleteAsync()
    {
        return await db.SaveChangesAsync() > 0;
    }
    
    public void Dispose()
    {
        db.Dispose();
    }
}