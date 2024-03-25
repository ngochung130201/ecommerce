using ecommerce.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce.Models
{
    [Table("history")]
    public class History
    {
        [Key]
        [Column("history_id")]
        public int HistoryId { get; set; }
        [Column("payment_id")]
        public int PaymentId { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("status")]
        public HistoryStatus Status { get; set; }
        public string? StatusMessage { get; set; } = null;
        [Column("message")]
        public string? Message { get; set; } = null;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateAt { get; set; } = null;
        public virtual Payment? Payment { get; set; }
        public virtual User? User { get; set; }
        public History(HistoryStatus Status)
        {
            this.StatusMessage = nameof(Status);
            this.Status = Status;
        }
        public History()
        {

        }
    }
}
