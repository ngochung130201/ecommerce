using ecommerce.DTO;

namespace ecommerce.Services.Interface
{
    public interface IOrderService
    {
        Task<ApiResponse<IEnumerable<OrderDto>>> GetAllOrdersAsync(PagingForOrder? paging = null);
        Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int id);
        Task<ApiResponse<int>> ProcessOrderAsync(OrderRequestDto order);
        Task<ApiResponse<int>> DeleteOrderAsync(int id);
        Task<ApiResponse<int>> UpdateOrderAsync(OrderUpdate order);
        Task<ApiResponse<bool>> DeleteOrderItemsAsync(OrderItemDeleteDto orderItemDeleteDto);
        Task<ApiResponse<IEnumerable<OrderDto>>> GetOrdersByUserIdAsync(int userId, PagingForOrder? paging = null);
        // Update order status
    }
}
