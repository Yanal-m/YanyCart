using Microsoft.EntityFrameworkCore;
using MyShoppingCart.Models;

namespace MyShoppingCart.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<ShoppingCart> ShoppingCarts => Set<ShoppingCart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
    }
}
