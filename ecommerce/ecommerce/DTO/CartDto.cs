namespace ecommerce.DTO
{
    public class CartDto
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public int Quantity { get; set; }
        public virtual ICollection<CartItemDto> CartItems { get; set; }
    }
}
