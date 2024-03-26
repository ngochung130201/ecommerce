using ecommerce.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce.Models
{
    [Table("payment")]
    public class Payment
    {
        [Key]
        [Column("payment_id")]
        public int PaymentId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("amount", TypeName = "DECIMAL(20,7)")]
        public decimal Amount { get; set; }

        [Column("payment_status")]
        public PaymentStatus PaymentStatus { get; set; }
        [Column("payment_status_text", TypeName = "nvarchar(255)")]
        public string? PaymentStatusText { get; set; } = null;

        [Column("payment_method")]
        public PaymentMethod PaymentMethod { get; set; }
        [Column("payment_method_text", TypeName = "nvarchar(255)")]
        public string? PaymentMethodText { get; set; } = null;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; } = null;

        // Navigation property
        public virtual Order Order { get; set; }
        public Payment(PaymentStatus PaymentStatus, PaymentMethod PaymentMethod)
        {
            this.PaymentStatusText = nameof(PaymentStatus);
            this.PaymentStatus = PaymentStatus;
            this.PaymentMethodText = nameof(PaymentMethod);
            this.PaymentMethod = PaymentMethod;
        }
        public Payment()
        {

        }
    }
}
