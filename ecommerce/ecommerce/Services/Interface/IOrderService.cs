using ecommerce.DTO;
using ecommerce.Enums;

namespace ecommerce.Services.Interface
{
    public interface IOrderService
    {
        Task<ApiResponse<IEnumerable<OrderDto>>> GetAllOrdersAsync();
        Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int id);
        Task<ApiResponse<int>> AddOrderAsync(OrderDto order);
        Task<ApiResponse<int>> DeleteOrderAsync(int id);
        Task<ApiResponse<int>> UpdateOrderAsync(int id, OrderDto order, PaymentMethod paymentMethod);
    }
}
