using ecommerce.DTO;
using ecommerce.Models;
using ecommerce.Repository.Interface;

namespace ecommerce.Services
{
    public class OrderItemService : IOrderItemService
    {
        private readonly IOrderItemRepository _orderItemRepository;
        public OrderItemService(IOrderItemRepository orderItemRepository)
        {
            _orderItemRepository = orderItemRepository;
        }
        public async Task<ApiResponse<int>> AddOrderItemAsync(OrderItemDto orderItem)
        {
            var orderItemEntity = new OrderItem
            {
                OrderId = orderItem.OrderId,
                ProductId = orderItem.ProductId,
                Quantity = orderItem.Quantity
            };
            try
            {
                _orderItemRepository.AddOrderItem(orderItemEntity);
                return new ApiResponse<int>
                {
                    Data = orderItemEntity.OrderId

                };
            }
            catch (Exception e)
            {
                return new ApiResponse<int>
                {
                    Message = "Error adding order item"
                };
            }
        }

        public async Task<ApiResponse<int>> AddOrderItemAsync(List<OrderItemDto> orderItem)
        {
            var orderItemEntities = orderItem.Select(x => new OrderItem
            {
                OrderId = x.OrderId,
                ProductId = x.ProductId,
                Quantity = x.Quantity
            }).ToList();
            try
            {
                _orderItemRepository.AddOrderItems(orderItemEntities);
                return new ApiResponse<int>
                {
                    Data = 0,
                    Message = "Order items added",
                    Status = true
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<int>
                {
                    Message = "Error adding order item"
                };
            }
        }

        public async Task<ApiResponse<int>> DeleteOrderItemAsync(int id)
        {
            var orderItem = await _orderItemRepository.GetOrderItemByIdAsync(id);
            if (orderItem == null)
            {
                return new ApiResponse<int>
                {
                    Message = "Order item not found"
                };
            }
            try
            {
                _orderItemRepository.DeleteOrderItem(orderItem);
                return new ApiResponse<int>
                {
                    Data = id
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<int>
                {
                    Message = "Error deleting order item"
                };
            }
        }

        public async Task<ApiResponse<int>> DeleteOrderItemByOrderIdAsync(int orderId)
        {
            var orderItems = await _orderItemRepository.GetOrderItemsByOrderIdAsync(orderId);
            if (orderItems == null)
            {
                return new ApiResponse<int>
                {
                    Message = "Order items not found"
                };
            }
            try
            {
                _orderItemRepository.DeleteOrderItemByOrderId(orderItems);
                return new ApiResponse<int>
                {
                    Data = orderId
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<int>
                {
                    Message = "Error deleting order items"
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<OrderItemDto>>> GetAllOrderItemsAsync()
        {
            var orderItems = await _orderItemRepository.GetAllOrderItemsAsync();
            if (orderItems == null)
            {
                return new ApiResponse<IEnumerable<OrderItemDto>>
                {
                    Message = "No order items found"
                };
            }
            var orderItemDtos = orderItems.Select(x => new OrderItemDto
            {
                OrderId = x.OrderId,
                ProductId = x.ProductId,
                Quantity = x.Quantity
            });
            return new ApiResponse<IEnumerable<OrderItemDto>>
            {
                Data = orderItemDtos
            };
        }

        public async Task<ApiResponse<OrderItemDto>> GetOrderItemByIdAsync(int id)
        {
            var orderItem = await _orderItemRepository.GetOrderItemByIdAsync(id);
            if (orderItem == null)
            {
                return new ApiResponse<OrderItemDto>
                {
                    Message = "Order item not found"
                };
            }
            var orderItemDto = new OrderItemDto
            {
                OrderId = orderItem.OrderId,
                ProductId = orderItem.ProductId,
                Quantity = orderItem.Quantity
            };
            return new ApiResponse<OrderItemDto>
            {
                Data = orderItemDto
            };
        }

        public async Task<ApiResponse<IEnumerable<OrderItemDto>>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            var orderItems = await _orderItemRepository.GetOrderItemsByOrderIdAsync(orderId);
            if (orderItems == null)
            {
                return new ApiResponse<IEnumerable<OrderItemDto>>
                {
                    Message = "No order items found"
                };
            }
            var orderItemDtos = orderItems.Select(x => new OrderItemDto
            {
                OrderId = x.OrderId,
                ProductId = x.ProductId,
                Quantity = x.Quantity
            });
            return new ApiResponse<IEnumerable<OrderItemDto>>
            {
                Data = orderItemDtos
            };
        }

        public async Task<ApiResponse<int>> UpdateOrderItemAsync(int id, OrderItemDto orderItem)
        {
            var existingOrderItem = await _orderItemRepository.GetOrderItemByIdAsync(id);
            if (existingOrderItem == null)
            {
                return new ApiResponse<int>
                {
                    Message = "Order item not found"
                };
            }
            var orderItemEntity = new OrderItem
            {
                OrderItemId = id,
                OrderId = orderItem.OrderId,
                ProductId = orderItem.ProductId,
                Quantity = orderItem.Quantity
            };
            try
            {
                _orderItemRepository.UpdateOrderItem(orderItemEntity);
                return new ApiResponse<int>
                {
                    Data = id
                };
            }
            catch (Exception e)
            {
                return new ApiResponse<int>
                {
                    Message = "Error updating order item"
                };
            }
        }
    }
}
