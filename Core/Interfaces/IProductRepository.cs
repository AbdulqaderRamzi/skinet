using Core.Entities;

namespace Core.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAsync(string? brand, string? type, string? sort);
    Task<Product?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<string>> GetBrandsAsync();
    Task<IReadOnlyList<string>> GetTypesAsync();
    Task AddAsync(Product product);
    void Update(Product product);
    void Delete(Product product);
    Task<bool> IsExistsAsync(Guid id);
    Task<bool> SaveChangesAsync();
}