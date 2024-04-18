using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce.Models
{
    [Table("product_review")]
    public class ProductReview
    {
        [Key]
        [Column("review_id")]
        public int ReviewId { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("rating")]
        public int Rating { get; set; }

        [Column("comment", TypeName = "nvarchar(255)")]
        public string Comment { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; } = null;
        [Column("image")]
        public string? Image { get; set; } = null;

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
    }
}
