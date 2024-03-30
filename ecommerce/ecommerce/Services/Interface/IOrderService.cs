using ecommerce.DTO;

namespace ecommerce.Services.Interface
{
    public interface IOrderService
    {
        Task<ApiResponse<IEnumerable<OrderDto>>> GetAllOrdersAsync();
        Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int id);
        Task<ApiResponse<int>> ProcessOrderAsync(OrderRequestDto order);
        Task<ApiResponse<int>> DeleteOrderAsync(int id);
        Task<ApiResponse<int>> UpdateOrderAsync(OrderUpdate order);
        Task<ApiResponse<bool>> DeleteOrderItemsAsync(OrderItemDeleteDto orderItemDeleteDto);
        // Update order status
    }
}
