using ecommerce.Enums;

namespace ecommerce.DTO
{
    public class HistoryDto
    {
        public int HistoryId { get; set; }
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public HistoryStatus Status { get; set; }
        public string? StatusMessage { get; set; } = null;
        public string? Message { get; set; } = null;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateAt { get; set; } = null;
        public PaymentDto Payment { get; set; } = new PaymentDto();
        public UserDto User { get; set; } = new UserDto();

    }
}
