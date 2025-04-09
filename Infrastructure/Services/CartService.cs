using System.Text.Json;
using Core.Entities;
using Core.Interfaces;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class CartService(IConnectionMultiplexer redis) : ICartService
{
    private readonly IDatabase _db = redis.GetDatabase();
    
    public async Task<ShoppingCart?> GetAsync(string key)
    {
        var data = await _db.StringGetAsync(key);
        
        return data.IsNullOrEmpty ? null 
            : JsonSerializer.Deserialize<ShoppingCart>(data!);
    }

    public async Task<ShoppingCart?> SetAsync(ShoppingCart cart)
    {
        var isCreated  = await _db.StringSetAsync(cart.Id, 
            JsonSerializer.Serialize(cart), TimeSpan.FromDays(30));
        return isCreated ? await GetAsync(cart.Id) : null;
    }

    public async Task<bool> DeleteAsync(string key)
    {
        return await _db.KeyDeleteAsync(key);
    }
}