namespace ecommerce.Enums
{
    public enum HistoryStatus
    {
        PaymentInitiated, // Đã khởi tạo thanh toán
        PaymentCompleted, // Thanh toán thành công
        PaymentFailed,
        OrderPending, // Chờ xác nhận
        OrderProcessing, // Đang xử lý
        OrderShipped, // Đã giao hàng
        OrderDelivered, // Đã nhận hàng
        OrderCancelled // Đã hủy đơn hàng
    }
}
