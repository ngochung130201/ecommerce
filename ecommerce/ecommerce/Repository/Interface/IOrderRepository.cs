using ecommerce.DTO;
using ecommerce.Models;

namespace ecommerce.Repository.Interface
{
    public interface IOrderRepository
    {
        Task<(IEnumerable<Order>,int)> GetAllOrdersAsync(PagingForOrder? paging = null);
        Task<Order> GetOrderByIdAsync(int id);
        Task<int> AddOrderAsync(Order order);
        Task DeleteOrderAsync(int id);
        Task UpdateOrderAsync(int id, Order orderToUpdate);
        Task<Order> GetOrderByUserIdAsync(int userId);
        // Get Cart by userId
        Task<Order> GetCartByUserIdAsync(int userId);
    }
}
