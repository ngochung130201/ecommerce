using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce.Models
{
    [Table("category")]
    public class Category
    {
        [Key]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("name", TypeName = "nvarchar(255)")]
        public string Name { get; set; }
        [Column("slug", TypeName = "nvarchar(255)")]
        public string Slug { get; set; }

        [Column("description", TypeName = "nvarchar(255)")]
        public string Description { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; } = null;

        // Navigation properties
        public virtual ICollection<Product> Products { get; set; }
    }
}
