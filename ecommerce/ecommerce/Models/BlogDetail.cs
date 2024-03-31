using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce.Models
{
    [Table("blog_detail")]
    public class BlogDetail
    {
        [Key]
        [Column("blog_detail_id")]
        public int BlogDetailId { get; set; }

        [Column("blog_id")]
        public int BlogId { get; set; }

        [Column("content")]
        public string? Content { get; set; } = null;

        [Column("description")]
        public string? Description { get; set; } = null;
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set;}
    }
}
