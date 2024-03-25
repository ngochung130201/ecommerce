namespace ecommerce.Enums
{
    public enum OrderStatus
    {
        Pending, // Chờ xác nhận
        Processing, // Đang xử lý
        Shipped, // Đã giao hàng
        Delivered, // Đã nhận hàng
        Cancelled // Đã hủy
    }
}
