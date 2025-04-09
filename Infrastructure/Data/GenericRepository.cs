using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class GenericRepository<T>(AppDbContext db)
    : IGenericRepository<T> where T : BaseEntity
{
    
    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await db.Set<T>().ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await db.Set<T>().FindAsync(id);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).ToListAsync(); 
    }

    public async Task<T?> GetEntityAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<TResult>> GetAllAsync<TResult>(ISpecification<T, TResult> spec)
    {
        return await ApplySpecification(spec).ToListAsync();
    }

    public async Task<TResult?> GetEntityAsync<TResult>(ISpecification<T, TResult> spec)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync();
    }

    public async Task AddAsync(T entity)
    {
        await db.Set<T>().AddAsync(entity); 
    }

    public void Update(T entity)
    {
        db.Set<T>().Attach(entity); // happened automatically at the bottom 
        db.Entry(entity).State = EntityState.Modified;
    }

    public void Remove(T entity)
    {
        db.Set<T>().Remove(entity);
    }

    public async Task<bool> SaveAsync()
    {
        return await db.SaveChangesAsync() > 0;
    }

    public Task<bool> IsExistsAsync(Guid id)
    {
        return db.Set<T>().AnyAsync(e => e.Id == id);
    }

    public async Task<int> CountAsync(ISpecification<T> spec)
    {
        var query = db.Set<T>().AsQueryable();
        query = spec.ApplyCriteria(query);
        return await query.CountAsync();
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        return SpecificationEvaluator<T>.GetQuery(db.Set<T>().AsQueryable(), spec);
    }

    private IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T, TResult> spec)
    {
        return SpecificationEvaluator<T>.GetQuery(db.Set<T>().AsQueryable(), spec);
    }
}