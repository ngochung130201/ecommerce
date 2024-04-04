namespace ecommerce.DTO
{
    public class WishlistDto
    {
        public int WishlistId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public UserDto User { get; set; } = new UserDto();
        public ProductAllDto Product { get; set; } = new ProductAllDto();
    }
    public class WishlistRequestDto
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
    }
}
