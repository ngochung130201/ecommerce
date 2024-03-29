using ecommerce.Context;
using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IRepositoryBase<Order> _repositoryBase;
        private readonly EcommerceContext _context;
        public OrderRepository(IRepositoryBase<Order> repositoryBase, EcommerceContext context)
        {
            _repositoryBase = repositoryBase;
            _context = context;
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

        public async Task<Order> GetOrderByUserIdAsync(int userId)
        {
            var order = await _context.Orders.Include(u => u.OrderItems).Include(x => x.User).ThenInclude(x => x.Carts).ThenInclude(u => u.CartItems).ThenInclude(u => u.Product).FirstOrDefaultAsync(x => x.UserId == userId);
            if (order != null)
            {
                return order;
            }
            return null;
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
