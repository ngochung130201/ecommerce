using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce.Models
{
    [Table("blog_category")]
    public class BlogCategory
    {
        [Key]
        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("name")]
        public string? Name { get; set; }
        [Column("slug")]
        public string? Slug { get; set; }
        [Column("description")]
        public string? Description { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [Column("created_by")]
        public string? CreatedBy { get; set; }
        [Column("updated_by")]
        public string? UpdatedBy { get; set; }
        public List<Blog> Blogs { get; set; }
        public BlogCategory()
        {
            Blogs = new List<Blog>();
        }

    }
}
