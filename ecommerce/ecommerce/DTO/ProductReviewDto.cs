namespace ecommerce.DTO
{
    public class ProductReviewDto
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
    public class ProductReviewAllDto
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
    }
    public class ProductReviewUpdateDto
    {
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
