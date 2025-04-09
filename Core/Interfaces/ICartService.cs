using Core.Entities;

namespace Core.Interfaces;

public interface ICartService
{
    Task<ShoppingCart?> GetAsync(string key);
    Task<ShoppingCart?> SetAsync(ShoppingCart cart);
    Task<bool> DeleteAsync(string key);
    
}