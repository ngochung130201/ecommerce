namespace ecommerce.DTO
{
    public class CategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
    }
    public class CategoryAllDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public string Slug { get; set; }
    }
}
