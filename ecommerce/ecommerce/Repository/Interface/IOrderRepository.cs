using ecommerce.Models;

namespace ecommerce.Repository.Interface
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task<int> AddOrderAsync(Order order);
        Task DeleteOrderAsync(int id);
        Task UpdateOrderAsync(int id, Order orderToUpdate);
        Task<Order> GetOrderByUserIdAsync(int userId);
    }
}
