using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TBP_Backend.Models;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<ProductImg> ProductImg { get; set; }
    public DbSet<ProductVariant> ProductVariant { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItems> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().ToTable("Category");
        modelBuilder.Entity<Product>().ToTable("Product");
        modelBuilder.Entity<ProductImg>().ToTable("ProductImg");
        modelBuilder.Entity<ProductVariant>().ToTable("ProductVariant");
        modelBuilder.Entity<Cart>().ToTable("Carts");
        modelBuilder.Entity<CartItem>().ToTable("CartItem");
        modelBuilder.Entity<Order>().ToTable("Orders");
    }
}
