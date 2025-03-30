using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class AppDbContextSeed
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!db.Products.Any())
        {
            var products = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
            var productsJson = JsonSerializer.Deserialize<List<Product>>(products);
            if (productsJson is null) return;
            db.Products.AddRange(productsJson);
            await db.SaveChangesAsync();
        }
    }
}