using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using YanyCart.Data;
using YanyCart.Models;
var builder = WebApplication.CreateBuilder(args);
Batteries.Init();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// get all products
app.MapGet("/products", async (AppDbContext db) =>
    await db.Products.ToListAsync());


// post product

app.MapPost("/products", async (AppDbContext db, Product product) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

// add a cart

app.MapPost("/carts", async (AppDbContext db, ShoppingCart cart) =>
{
    db.ShoppingCarts.Add(cart);
    await db.SaveChangesAsync();
    return Results.Created($"/carts/{cart.Id}", cart);
});

// get cart by id
app.MapGet("/carts/{id}", async (AppDbContext db, int id) =>
{
    var cart = await db.ShoppingCarts
        .Include(c => c.Items)
        .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(c => c.Id == id);

    return cart is not null ? Results.Ok(cart) : Results.NotFound();
});

// update cart 
app.MapPut("/carts/{id}", async (AppDbContext db, int id, ShoppingCart updatedCart) =>
{
    var cart = await db.ShoppingCarts
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == id);

    if (cart is null)
        return Results.NotFound();

    cart.UserId = updatedCart.UserId;

    // Remove old items
    db.CartItems.RemoveRange(cart.Items);

    // Add new items safely
    cart.Items = updatedCart.Items.Select(i => new CartItem
    {
        ProductId = i.ProductId,
        Quantity = i.Quantity
    }).ToList();

    await db.SaveChangesAsync();

    return Results.Ok(cart);
});


// remove cart
app.MapDelete("/carts/{id}", async (AppDbContext db, int id) =>
{
    var cart = await db.ShoppingCarts
        .Include(c => c.Items)
        .FirstOrDefaultAsync(c => c.Id == id);

    if (cart is null)
        return Results.NotFound();
    db.CartItems.RemoveRange(cart.Items);
    db.ShoppingCarts.Remove(cart);
    await db.SaveChangesAsync();

    return Results.NoContent();
});


//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast");

app.Run();

//internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
//{
//    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
//}
