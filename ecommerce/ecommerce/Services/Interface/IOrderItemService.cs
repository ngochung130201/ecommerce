using ecommerce.DTO;

namespace ecommerce.Services
{
    public interface IOrderItemService
    {
        Task<ApiResponse<IEnumerable<OrderItemDto>>> GetAllOrderItemsAsync();
        Task<ApiResponse<OrderItemDto>> GetOrderItemByIdAsync(int id);
        Task<ApiResponse<IEnumerable<OrderItemDto>>> GetOrderItemsByOrderIdAsync(int orderId);
        Task<ApiResponse<int>> AddOrderItemAsync(OrderItemDto orderItem);
        Task<ApiResponse<int>> AddOrderItemAsync(List<OrderItemDto> orderItem);
        Task<ApiResponse<int>> UpdateOrderItemAsync(int id, OrderItemDto orderItem);
        Task<ApiResponse<int>> DeleteOrderItemAsync(int id);
        Task<ApiResponse<int>> DeleteOrderItemByOrderIdAsync(int orderId);
    }
}
