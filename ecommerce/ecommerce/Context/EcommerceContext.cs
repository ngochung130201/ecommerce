using ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Context
{
    public class EcommerceContext : DbContext
    {
        public EcommerceContext(DbContextOptions<EcommerceContext> options) : base(options)
        {
        }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<ProductReview> ProductReviews { get; set; }
        public virtual DbSet<RevenueReport> RevenueReports { get; set; }
        public virtual DbSet<Wishlist> Wishlists { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<History> Histories { get; set; }
        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<BlogDetail> BlogDetails { get; set; }
        public virtual DbSet<BlogCategory> BlogCategories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().Property(p => p.TotalPrice).HasColumnType("decimal(20,7)");
            modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(20,7)");
            modelBuilder.Entity<CartItem>().Property(p => p.TotalPrice).HasColumnType("decimal(20,7)");
            modelBuilder.Entity<Order>().Property(p => p.TotalPrice).HasColumnType("decimal(20,7)");
            modelBuilder.Entity<RevenueReport>().Property(p => p.TotalRevenue).HasColumnType("decimal(20,7)");
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasColumnType("decimal(20,7)");
            modelBuilder.Entity<OrderItem>().Property(p => p.PriceAtTimeOfOrder).HasColumnType("decimal(20,7)");
        }

    }
}
