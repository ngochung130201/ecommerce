namespace ecommerce.Enums
{
    public enum HistoryStatus
    {
        PaymentInitiated, // Đã khởi tạo thanh toán
        PaymentCompleted, // Thanh toán thành công
        PaymentFailed, // Thanh toán thất bại
        OrderCancelled // Đã hủy đơn hàng
    }
}
