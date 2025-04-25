using Api.Middleware;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? throw new Exception("Cannot get Postgres connection string");
    opt.UseNpgsql(connectionString);
});

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddCors();
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
     var connectionString = builder.Configuration.GetConnectionString("Redis")
        ?? throw new Exception("Cannot get Redis connection string");
     var configuration = ConfigurationOptions.Parse(connectionString, true);
     return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<ICartService, CartService>();
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithOrigins("http://localhost:4200"));

app.MapControllers();
app.MapGroup("api").MapIdentityApi<AppUser>();

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
