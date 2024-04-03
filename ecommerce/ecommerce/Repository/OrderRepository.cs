using ecommerce.Context;
using ecommerce.DTO;
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

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(PagingForOrder? paging = null)
        {
            // if paging is null, return all orders
            if (paging == null)
            {
                return await _context.Orders.Include(u => u.OrderItems).Include(x => x.User).ToListAsync();
            }
            // if paging is not null, return orders based on paging
            var orders = _context.Orders.Include(u=> u.OrderItems).ThenInclude(i=>i.Product).Include(k=>k.User).AsQueryable();
            if (!string.IsNullOrEmpty(paging.Search))
            {
               orders = orders.Where(u=> u.User.Username.Contains(paging.Search) || u.User.Email.Contains(paging.Search));
            }
            if (paging.MinTotalPrice != 0)
            {
                orders = orders.Where(u => u.TotalPrice >= paging.MinTotalPrice);
            }
            if (paging.MaxTotalPrice != 0)
            {
                orders = orders.Where(u => u.TotalPrice <= paging.MaxTotalPrice);
            }
            if (paging.SortByDate)
            {
                orders = orders.OrderBy(u => u.CreatedAt);
            }
            else
            {
                orders = orders.OrderByDescending(u => u.CreatedAt);
            }
            if(paging.OrderStatus != null)
            {
                orders = orders.Where(u => u.OrderStatus == paging.OrderStatus);
            }
            orders = orders.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);
            if (orders == null)
            {
                throw new CustomException("No Order found", 404);
            }
            return orders;
        }

        public Task<Order> GetCartByUserIdAsync(int userId)
        {
            throw new NotImplementedException();
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
