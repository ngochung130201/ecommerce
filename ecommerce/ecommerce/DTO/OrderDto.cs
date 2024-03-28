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
    public class OrderUpdate
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
    public class OrderRequestDto
    {
        public int UserId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.DirectPayment;
        public List<int> ProductIds { get; set; }
    }

    public class OrderItemDeleteDto
    {
        public List<int>? OrderItemIds { get; set; } = null;
        public int OrderId { get; set; }
    }
}
