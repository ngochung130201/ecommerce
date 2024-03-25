using ecommerce.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce.Models
{
    [Table("order")]
    public class Order
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("order_status")]
        public OrderStatus OrderStatus { get; set; }
        [Column("order_status_message")]
        public string? OrderStatusMessage { get; set; } = null;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; } = null;
        [Column("cart_id")]
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public Order(OrderStatus OrderStatus)
        {
            this.OrderStatusMessage = nameof(OrderStatus);
            this.OrderStatus = OrderStatus;
        }
        public Order()
        {
            TotalPrice = this.OrderItems?.Sum(oi => oi.PriceAtTimeOfOrder * oi.Quantity) ?? 0;
        }
    }
}
