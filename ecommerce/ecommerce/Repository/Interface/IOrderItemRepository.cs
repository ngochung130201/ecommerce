using ecommerce.Models;

namespace ecommerce.Repository.Interface
{
    public interface IOrderItemRepository
    {
        Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync();
        Task<OrderItem> GetOrderItemByIdAsync(int id);
        Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);
        void AddOrderItem(OrderItem orderItem);
        void AddOrderItems(List<OrderItem> orderItem);
        void UpdateOrderItem(OrderItem existingOrderItem);
        void DeleteOrderItem(OrderItem? orderItem);
        void DeleteOrderItemByOrderId(IEnumerable<OrderItem>? orderItems);
        Task<IEnumerable<OrderItem>> GetListOrderItemsByOrderIdAsync(int orderId, List<int> orderItemIds);
    }
}
