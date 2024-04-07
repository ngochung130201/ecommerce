namespace ecommerce.DTO
{
    public class BlogDto
    {

        public string? Title { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        // public IFormFile? Image { get; set; } = null;
        public List<int> CategoryIds { get; set; } = new List<int>();
        public BlogDetailDto Details { get; set; } = new BlogDetailDto();
    }
    public class BlogAllDto
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string Image { get; set; } = null;
        public List<int> CategoryIds { get; set; } = new List<int>();
        public List<string> Categories { get; set; } = new List<string>();
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
          public BlogDetailDto Details { get; set; } = new BlogDetailDto();
    }
}
