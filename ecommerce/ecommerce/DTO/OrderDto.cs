using ecommerce.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce.DTO
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public decimal TotalPrice { get; set; }
        public string? Address { get; set; } = null;
        public string? PhoneNumber { get; set; } = null;
        public string? Note { get; set; } = null;
        public virtual ICollection<OrderItemDto> OrderItems { get; set; }
    }
    public class OrderUpdate
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? Address { get; set; } = null;
        public string? PhoneNumber { get; set; } = null;
        public string? Note { get; set; } = null;
    }
    public class OrderRequestDto
    {
        public int UserId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.DirectPayment;
        public List<OrderProductDto> OrderProduct { get; set; }

        public string? Address { get; set; } = null;
        public string? PhoneNumber { get; set; } = null;
        public string? Note { get; set; } = null;

    }

    public class OrderItemDeleteDto
    {
        public List<int>? OrderItemIds { get; set; } = null;
        public int OrderId { get; set; }
    }
    // Order Product
    public class OrderProductDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
