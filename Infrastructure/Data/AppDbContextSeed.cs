using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class AppDbContextSeed
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!db.Products.Any())
        {
            var productsJson = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
            var products = JsonSerializer.Deserialize<List<Product>>(productsJson);
            if (products is null) return;
            db.Products.AddRange(products);
            await db.SaveChangesAsync();
        }

        if (!db.DeliveryMethods.Any())
        {
            var methodsJson = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/delivery.json");
            var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(methodsJson);
            if (methods is null) return;
            db.DeliveryMethods.AddRange(methods);
            await db.SaveChangesAsync();
        }
    }
}