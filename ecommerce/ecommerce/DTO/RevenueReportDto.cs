namespace ecommerce.DTO
{
    public class RevenueReportDto
    {
        public int ReportId { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
    }
}
