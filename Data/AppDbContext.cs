using Microsoft.EntityFrameworkCore;
using ProductsApi.Models;

namespace ProductsApi.Data;

/// <summary>
/// The EF Core DbContext — your session with the database. Exposes one DbSet per table and tracks
/// changes so SaveChangesAsync() persists them in a transaction.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();       // the Products table
    public DbSet<Category> Categories => Set<Category>();   // the Categories table

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed categories first, then products that reference them by CategoryId.
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Peripherals" },
            new Category { Id = 2, Name = "Displays" });

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Keyboard", Price = 29.99m, CategoryId = 1 },
            new Product { Id = 2, Name = "Mouse", Price = 14.50m, CategoryId = 1 },
            new Product { Id = 3, Name = "Monitor", Price = 199.00m, CategoryId = 2 });
    }
}
