using ecommerce.Enums;

namespace ecommerce.DTO
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public virtual ICollection<OrderItemDto> OrderItems { get; set; }
    }
}
