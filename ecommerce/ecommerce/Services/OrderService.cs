using ecommerce.DTO;
using ecommerce.Enums;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;

namespace ecommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemService _orderItemService;
        private readonly ICartService _cartItemService;
        private readonly IPaymentService _paymentService;
        private readonly IHistoryService _historyService;
        public OrderService(IOrderRepository orderRepository,
        IPaymentService paymentService, IOrderItemService orderItemService,
        ICartService cartItemService, IHistoryService historyService)
        {
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            _orderItemService = orderItemService;
            _cartItemService = cartItemService;
            _historyService = historyService;
        }
        public async Task<ApiResponse<int>> AddOrderAsync(OrderDto order)
        {
            // find cart by user id
            var cart = await _cartItemService.GetCartsByUserIdAsync(order.UserId);

            // add order item
            var newOrder = new Order
            {
                OrderStatus = order.OrderStatus,
                UserId = order.UserId,
                CreatedAt = DateTime.UtcNow,
                OrderStatusMessage = nameof(order.OrderStatus)
            };
            try
            {
                var orderId = await _orderRepository.AddOrderAsync(newOrder);
                // add order to order table
                foreach (var item in order.OrderItems)
                {
                    item.OrderId = orderId;
                }
                await _orderItemService.AddOrderItemAsync(order.OrderItems.ToList());
                return new ApiResponse<int>
                {
                    Data = 0,
                    Message = "Order added",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Data = 0,
                    Message = ex.Message,
                    Status = false
                };
            }

        }

        public async Task<ApiResponse<int>> DeleteOrderAsync(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "No Order found",
                    Status = false
                };
            }
            try
            {
                // delete order item
                await _orderItemService.DeleteOrderItemByOrderIdAsync(id);
                await _orderRepository.DeleteOrderAsync(id);
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "Order deleted",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = ex.Message,
                    Status = false
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<OrderDto>>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            if (orders == null)
            {
                return new ApiResponse<IEnumerable<OrderDto>>
                {
                    Data = null,
                    Message = "No Order found",
                    Status = false
                };
            }
            return new ApiResponse<IEnumerable<OrderDto>>
            {
                Data = orders.Select(order => new OrderDto
                {
                    OrderStatus = order.OrderStatus,
                    UserId = order.UserId,
                    CreatedAt = order.CreatedAt
                }),
                Message = "Orders retrieved",
                Status = true
            };
        }

        public async Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                return new ApiResponse<OrderDto>
                {
                    Data = null,
                    Message = "Order not found",
                    Status = false
                };
            }
            return new ApiResponse<OrderDto>
            {
                Data = new OrderDto
                {
                    OrderStatus = order.OrderStatus,
                    UserId = order.UserId,
                    CreatedAt = order.CreatedAt
                },
                Message = "Order found",
                Status = true
            };
        }

        public async Task<ApiResponse<int>> UpdateOrderAsync(int id, OrderDto order, PaymentMethod paymentMethod)
        {
            var orderToPayment = new OrderStatus[]
            {
                OrderStatus.Delivered,
                OrderStatus.Shipped,
                OrderStatus.Cancelled
            };
            try
            {
                var orderExist = await _orderRepository.GetOrderByIdAsync(id);
                if (orderExist == null)
                {
                    return new ApiResponse<int>
                    {
                        Data = 0,
                        Message = "No Order found",
                        Status = false
                    };
                }
                var orderStatus = GetOrderStatus(order);
                orderExist.OrderStatus = orderStatus;
                orderExist.UpdatedAt = order.UpdatedAt;
                orderExist.OrderStatusMessage = nameof(orderStatus);
                orderExist.UpdatedAt = DateTime.UtcNow;
                if (!orderToPayment.Contains(orderStatus))
                {
                    await _orderRepository.UpdateOrderAsync(id, orderExist);
                }
                else
                {
                    if (orderExist.OrderStatus == OrderStatus.Cancelled)
                    {
                        // add table history
                        var history = new HistoryDto
                        {
                            UserId = order.UserId,
                            Message = "Order Cancelled",
                            PaymentId = 0,
                            Status = HistoryStatus.OrderCancelled,
                            StatusMessage = nameof(HistoryStatus.OrderCancelled)
                        };
                        await _historyService.AddHistoryAsync(history);

                    }
                    else
                    {
                        await AddPaymentAsync(payment: new PaymentDto
                        {
                            OrderId = id,
                            PaymentMethod = paymentMethod,
                            Amount = orderExist.TotalPrice
                        }, orderId: id, orderStatus: orderStatus);
                    }

                }

                return new ApiResponse<int>
                {
                    Data = 0,
                    Message = "Order updated",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Data = 0,
                    Message = ex.Message,
                    Status = false
                };
            }
        }
        // Get Order status update
        public OrderStatus GetOrderStatus(OrderDto order)
        {
            switch (order.OrderStatus)
            {
                case OrderStatus.Pending:
                    order.OrderStatus = OrderStatus.Processing;
                    break;
                case OrderStatus.Processing:
                    order.OrderStatus = OrderStatus.Shipped;
                    break;
                case OrderStatus.Shipped:
                    order.OrderStatus = OrderStatus.Delivered;
                    break;
                case OrderStatus.Delivered:
                    order.OrderStatus = OrderStatus.Cancelled;
                    break;
            }
            return order.OrderStatus;
        }
        // Add Payment
        public async Task AddPaymentAsync(PaymentDto payment, int orderId, OrderStatus orderStatus)
        {
            await _paymentService.AddPaymentAsync(payment);
            await _orderRepository.UpdateOrderAsync(orderId, new Order
            {
                OrderStatus = orderStatus,
                UpdatedAt = DateTime.UtcNow,
                OrderStatusMessage = nameof(orderStatus),
            });
        }
    }
}