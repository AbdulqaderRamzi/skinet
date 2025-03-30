using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    opt.UseNpgsql(connectionString);
});

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();

var app = builder.Build();

app.MapControllers();
await SeedData();
app.Run();

return;

async Task SeedData()
{
    try
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var db = services.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync(); // Create db && Apply migrations 
        await AppDbContextSeed.SeedAsync(db);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }
}
