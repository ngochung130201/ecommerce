﻿using ecommerce.Context;
using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Repository
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly IRepositoryBase<OrderItem> _repositoryBase;
        private readonly EcommerceContext _context;
        public OrderItemRepository(IRepositoryBase<OrderItem> repositoryBase, EcommerceContext context)
        {
            _repositoryBase = repositoryBase;
            _context = context;
        }

        public void AddOrderItem(OrderItem orderItem)
        {
            try
            {
                _repositoryBase.Create(orderItem);
            }
            catch (Exception e)
            {
                throw new CustomException("Order item not added", 500);
            }
        }

        public void AddOrderItems(List<OrderItem> orderItem)
        {
            try
            {
                _repositoryBase.CreateRange(orderItem);
            }
            catch (Exception e)
            {
                throw new CustomException("Order item not added", 500);
            }
        }

        public void DeleteOrderItem(OrderItem? orderItem)
        {
            try
            {
                if (orderItem == null)
                {
                    throw new CustomException("No Order Item found", 404);
                }
                _repositoryBase.Delete(orderItem);
            }
            catch (Exception e)
            {
                throw new CustomException(e.Message, 500);
            }
        }

        public void DeleteOrderItemByOrderId(IEnumerable<OrderItem>? orderItems)
        {
            if (orderItems == null)
            {
                throw new CustomException("No Order Item found", 404);
            }
            try
            {
                _repositoryBase.DeleteRange(orderItems);
            }
            catch (Exception e)
            {
                throw new CustomException(e.Message, 500);
            }
        }

        public async Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync()
        {
            var orderItems = await _repositoryBase.FindAllAsync();
            return orderItems;
        }

        public async Task<OrderItem> GetOrderItemByIdAsync(int id)
        {
            var orderItem = await _repositoryBase.FindByIdAsync(id);
            if (orderItem == null)
            {
                throw new CustomException("Order Item not found", 404);
            }
            return orderItem;
        }

        public async Task<IEnumerable<OrderItem>> GetListOrderItemsByOrderIdAsync(int orderId, List<int> orderItemIds)
        {
            var orderItems = await _context.OrderItems.Where(x => x.OrderId == orderId && orderItemIds.Contains(x.OrderItemId)).ToListAsync();
            if (orderItems.Any())
            {
                return orderItems;
            }
            return null;
        }


        public Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            var orderItems = _repositoryBase.FindByConditionAsync(x => x.OrderId == orderId);
            if (orderItems == null)
            {
                throw new CustomException("No Order Item found", 404);
            }
            return orderItems;
        }

        public void UpdateOrderItem(OrderItem existingOrderItem)
        {
            if (existingOrderItem == null)
            {
                throw new CustomException("Order Item not found", 404);
            }
            _repositoryBase.Update(existingOrderItem);
        }
    }
}
