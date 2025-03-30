using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ProductRepository(AppDbContext db) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> 
        GetAsync(string? brand, string? type, string? sort)
    {
        var query = db.Products.AsQueryable();
        
        if(!string.IsNullOrWhiteSpace(brand))
            query = query.Where(p => p.Brand.Contains(brand));
        
        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(p => p.Type.Contains(type));

        query = sort switch
        {
            "priceAsc" => query.OrderBy(p => p.Price),
            "priceDesc" => query.OrderByDescending(p => p.Price),
            _ => query.OrderBy(p => p.Name)
        };

        return await query.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
       return await db.Products.FindAsync(id);
    }

    public async Task<IReadOnlyList<string>> GetBrandsAsync()
    {
        return await db.Products.Select(p => p.Brand)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetTypesAsync()
    {
        return await db.Products.Select(p => p.Type)
            .Distinct()
            .ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        await db.Products.AddAsync(product);
    }

    public void Update(Product product)
    {
        db.Entry(product).State = EntityState.Modified;
    }

    public void Delete(Product product)
    {
        db.Products.Remove(product);
    }

    public async Task<bool> IsExistsAsync(Guid id)
    {
        return await db.Products.AnyAsync(e => e.Id == id);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await db.SaveChangesAsync() > 0;
    }
}