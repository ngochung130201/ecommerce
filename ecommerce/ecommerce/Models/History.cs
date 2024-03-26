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
        [Column("status")]
        public HistoryStatus Status { get; set; }
        [Column("status_message", TypeName = "nvarchar(255)")]
        public string? StatusMessage { get; set; } = null;
        [Column("message", TypeName = "nvarchar(255)")]
        public string? Message { get; set; } = null;
        [Column("created_at")]
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        [Column("updated_at")]
        public DateTime? UpdateAt { get; set; } = null;
        public virtual Payment? Payment { get; set; }
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
