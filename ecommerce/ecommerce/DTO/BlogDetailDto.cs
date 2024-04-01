namespace ecommerce.DTO
{
    public class BlogDetailDto
    {
        public string? Content { get; set; } = null;
        public string? Description { get; set; } = null;
    }
    public class BlogDetailAllDto
    {
        public int BlogDetailId { get; set; }
        public int BlogId { get; set; }
        public string? Content { get; set; } = null;
        public string? Description { get; set; } = null;
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}