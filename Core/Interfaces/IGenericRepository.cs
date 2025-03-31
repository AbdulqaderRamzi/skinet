using Core.Entities;

namespace Core.Interfaces;

public interface IGenericRepository<T> where T : BaseEntity
{
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> GetAllAsync(ISpecification<T> spec);
    Task<T?> GetEntityAsync(ISpecification<T> spec); 
    Task<IReadOnlyList<TResult>> GetAllAsync<TResult>(ISpecification<T, TResult> spec);
    Task<TResult?> GetEntityAsync<TResult>(ISpecification<T, TResult> spec);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    Task<bool> SaveAsync();
    Task<bool> IsExistsAsync(Guid id);
    Task<int> CountAsync(ISpecification<T> spec);
}