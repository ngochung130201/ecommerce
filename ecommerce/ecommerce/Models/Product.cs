using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce.Models
{
    [Table("product")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("name", TypeName = "nvarchar(255)")]
        public string Name { get; set; }
        [Column("slug", TypeName = "nvarchar(255)")]
        public string Slug { get; set; }

        [Column("description", TypeName = "nvarchar(255)")]
        public string? Description { get; set; } = null;

        [Column("price", TypeName = "DECIMAL(20,7)")]
        public decimal Price { get; set; }

        [Column("inventory_count")]
        public int InventoryCount { get; set; }
        [Column("image")]
        public string? Image { get; set; } = null;
        [Column("gallery")]
        public string? Gallery { get; set; } = null;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; } = null;

        // Navigation properties
        public virtual Category Category { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<Wishlist> Wishlists { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
