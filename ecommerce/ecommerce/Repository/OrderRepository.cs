using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;

namespace ecommerce.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IRepositoryBase<Order> _repositoryBase;
        public OrderRepository(IRepositoryBase<Order> repositoryBase)
        {
            _repositoryBase = repositoryBase;
        }
        public async Task<int> AddOrderAsync(Order order)
        {
            try
            {
                _repositoryBase.Create(order);

                return order.OrderId;
            }
            catch (Exception ex)
            {
                throw new CustomException("Order not added", 500);
            }
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _repositoryBase.FindByIdAsync(id);
            if (order == null)
            {
                throw new CustomException("No Order found", 404);
            }
            try
            {
                _repositoryBase.Delete(order);

            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            var orders = await _repositoryBase.FindAllAsync();
            if (orders == null)
            {
                throw new CustomException("No Order found", 404);
            }
            return orders;
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var order = await _repositoryBase.FindByIdAsync(id);
            if (order == null)
            {
                throw new CustomException("Order not found", 404);
            }
            return order;
        }

        public async Task UpdateOrderAsync(int id, Order orderToUpdate)
        {
            if (orderToUpdate == null)
            {
                throw new CustomException("No Order found", 404);
            }
            try
            {
                _repositoryBase.Update(orderToUpdate);

            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }
    }
}
