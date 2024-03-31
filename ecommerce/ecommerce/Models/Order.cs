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
        [Column("order_status_message", TypeName = "nvarchar(255)")]
        public string? OrderStatusMessage { get; set; } = null;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; } = null;


        // shipping address
        [Column("address")]
        public string? Address { get; set; } = null;
        // phone number
        [Column("phone_number")]
        public string? PhoneNumber { get; set; } = null;
        // Note 
        [Column("note")]
        public string? Note { get; set; } = null;
        // Navigation properties
        public virtual User User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        [Column("total_price", TypeName = "DECIMAL(20,7)")]
        public decimal TotalPrice { get; set; } = 0;
        public Order(OrderStatus OrderStatus)
        {
            this.OrderStatusMessage = nameof(OrderStatus);
            this.OrderStatus = OrderStatus;
        }
        public Order()
        {

        }
    }
}
