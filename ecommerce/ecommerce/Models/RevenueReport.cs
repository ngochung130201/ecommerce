using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ecommerce.Models
{
    public class RevenueReport
    {
        [Key]
        [Column("report_id")]
        public int ReportId { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("total_revenue")]
        public decimal TotalRevenue { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
