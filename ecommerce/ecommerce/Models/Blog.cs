using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce.Models
{
    [Table("blog")]
    public class Blog
    {
        [Key]
        [Column("blog_id")]
        public int BlogId { get; set; }

        [Column("title")]
        public string? Title { get; set; }
        [Column("slug")]
        public string? Slug { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [Column("created_by")]
        public string? CreatedBy { get; set; }
        [Column("updated_by")]
        public string? UpdatedBy { get; set; }
        [Column("image")]
        public string? Image { get; set; } = null;
        public List<BlogCategory> Categories { get; set; } = new List<BlogCategory>();
        public virtual BlogDetail Details { get; set; } = new BlogDetail();
    }
}
