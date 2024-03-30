using ecommerce.Context;
using ecommerce.DTO;
using ecommerce.Enums;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;
using ecommerce.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemService _orderItemService;
        private readonly ICartService  _cartService;
        private readonly EcommerceContext _context;
        private readonly IPaymentService _paymentService;
        private readonly IHistoryService _historyService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRevenueReportService _revenueReportService;
        public OrderService(IOrderRepository orderRepository,
        IPaymentService paymentService, IOrderItemService orderItemService,
        EcommerceContext context,
        ICartService  cartService, IHistoryService historyService,
        IRevenueReportService revenueReportService,
        IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _paymentService = paymentService;
            _orderItemService = orderItemService;
            _cartService = cartService;
            _historyService = historyService;
            _unitOfWork = unitOfWork;
            _revenueReportService = revenueReportService;
            _context = context;
        }
        public async Task<ApiResponse<int>> ProcessOrderAsync(OrderRequestDto order)
        {
            var orderIdByUser = await _orderRepository.GetOrderByUserIdAsync(order.UserId);
            var getCartByIdAsync = await _context.Carts.Where(u => u.UserId == order.UserId).Include(u => u.CartItems).ThenInclude(u => u.Product).FirstOrDefaultAsync();
            if (getCartByIdAsync == null)
            {
                return new ApiResponse<int>
                {
                    Data = 0,
                    Message = "No Cart found",
                    Status = false
                };
            }
            var cartItemsToProcess = getCartByIdAsync.CartItems.ToList();
            var products = cartItemsToProcess.Select(u => u.Product).ToList(); // have db
            var productInput = cartItemsToProcess.Select(u => u.Product).Where(u =>order.OrderProduct.Any(k=>k.ProductId == u.ProductId)).ToList();
            var productIds = cartItemsToProcess.Select(u => u.ProductId).ToList();
            var isDupProduct = productIds.Any(u => order.OrderProduct.Any(k => k.ProductId == u));
            // khac ngay hom nay van tao order moi
            var isToday = getCartByIdAsync.CreatedAt.Date == DateTime.UtcNow.Date;
            // Create a new order if it doesn't exist
            if (!isDupProduct || !isToday || orderIdByUser == null)
            {
                var newOrder = new Order
                {
                    UserId = order.UserId,
                    OrderStatus = OrderStatus.Pending,
                    OrderStatusMessage = nameof(OrderStatus.Pending),
                    CreatedAt = DateTime.UtcNow,
                };
                await _orderRepository.AddOrderAsync(newOrder);
                await _unitOfWork.SaveChangesAsync();
                // add table history
            }
            orderIdByUser = await _orderRepository.GetOrderByUserIdAsync(order.UserId);
            foreach (var cartItem in cartItemsToProcess)
            {
                var productExist = order.OrderProduct.FirstOrDefault(k => k.ProductId == cartItem.ProductId);
                if (productExist == null)
                {
                    continue;
                }
                await _orderItemService.AddOrderItemAsync(new OrderItemDto
                {
                    OrderId = orderIdByUser.OrderId,
                    ProductId = cartItem.ProductId,
                    Quantity = productExist.Quantity,
                    PriceAtTimeOfOrder = cartItem.TotalPrice,
                });
                await _unitOfWork.SaveChangesAsync();
                var orderItem = orderIdByUser.OrderItems.FirstOrDefault(u => u.ProductId == cartItem.ProductId);
                var product = products.FirstOrDefault(u => u.ProductId == cartItem.ProductId);
                orderItem.Quantity += productExist.Quantity;
                orderItem.PriceAtTimeOfOrder = product.PriceSale;
                await _orderItemService.UpdateOrderItemAsync(orderItem.OrderItemId, new OrderItemDto
                {
                    PriceAtTimeOfOrder = orderItem.PriceAtTimeOfOrder,
                });
                // if quantity input == quantity in cart, delete cart item
                if (productExist.Quantity == cartItem.Quantity)
                {
                    await _cartService.DeleteCartItemAsync(cartId: getCartByIdAsync.CartId, cartItemId: cartItem.CartItemId);
                }
                else
                {
                    // update cart item
                    await _cartService.UpdateCartItemQuantityAsync(cartId: getCartByIdAsync.CartId, cartItemId: cartItem.CartItemId, quantity: cartItem.Quantity - productExist.Quantity);
                }
            }
            orderIdByUser.TotalPrice = orderIdByUser.OrderItems.Sum(u => u.PriceAtTimeOfOrder);

            // Delete processed cart items and possibly the cart

            var getCartItemDb = await _context.Carts.Where(u => u.UserId == order.UserId).Include(u => u.CartItems).FirstOrDefaultAsync();
            if(productInput.Count == products.Count)
            {
                await _cartService.DeleteCartAsync(orderIdByUser.User.Carts.CartId);
            }
            // Add payment
            await AddPaymentAsync(payment: new PaymentDto
            {
                OrderId = orderIdByUser.OrderId,
                PaymentMethod = order.PaymentMethod,
                Amount = orderIdByUser.TotalPrice,
                PaymentStatus = PaymentStatus.Initiated,
                CreatedAt = DateTime.UtcNow,

            }, orderId: orderIdByUser.OrderId, orderStatus: order.OrderStatus);
            await _unitOfWork.SaveChangesAsync();
            var history = new HistoryDto
            {
                UserId = order.UserId,
                Message = "Order created_" + orderIdByUser.OrderId,
                Status = HistoryStatus.OrderPending,
                StatusMessage = nameof(HistoryStatus.OrderPending)
            };
            // get payment 
            var paymentId = await _paymentService.GetPaymentByOrderIdAsync(orderIdByUser.OrderId);
            await _historyService.AddHistoryAsync(new History
            {
                CreateAt = DateTime.UtcNow.Date,
                Message = history.Message,
                Status = history.Status,
                StatusMessage = history.StatusMessage,
                PaymentId = paymentId.PaymentId
            });
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<int>
            {
                Data = orderIdByUser.OrderId,
                Message = "Order added",
                Status = true
            };
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
                // remove cart and cart item
                await _cartService.DeleteListCartItemAsync(order.User.Carts.CartItems.ToList());
                await _cartService.DeleteCartAsync(order.UserId);
                await _unitOfWork.SaveChangesAsync();

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
                    CreatedAt = order.CreatedAt,
                    OrderId = order.OrderId,
                    UpdatedAt = order.UpdatedAt,
                    TotalPrice = order.TotalPrice
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
                    CreatedAt = order.CreatedAt,
                    OrderId = order.OrderId,
                    UpdatedAt = order.UpdatedAt,
                    TotalPrice = order.TotalPrice
                },
                Message = "Order found",
                Status = true
            };
        }

        public async Task<ApiResponse<int>> UpdateOrderAsync(OrderUpdate order)
        {
            var orderToPayment = new OrderStatus[]
            {
                OrderStatus.Delivered,
                OrderStatus.Shipped,
                OrderStatus.Processing,
                OrderStatus.Pending
            };
            try
            {
                var orderExist = await _orderRepository.GetOrderByIdAsync(order.OrderId);
                if (orderExist == null)
                {
                    return new ApiResponse<int>
                    {
                        Data = 0,
                        Message = "No Order found",
                        Status = false
                    };
                }
                // if status == delivered, shipped, processing, pending add payment
                orderExist.OrderStatus = order.OrderStatus;
                orderExist.OrderStatusMessage = nameof(order.OrderStatus);
                orderExist.UpdatedAt = DateTime.UtcNow;
                await _orderRepository.UpdateOrderAsync(order.OrderId, orderExist);
                await _unitOfWork.SaveChangesAsync();

                var payment = await _paymentService.GetPaymentByOrderIdAsync(order.OrderId);
                if (orderToPayment.Contains(orderExist.OrderStatus) && payment != null)
                {
                    if (order.OrderStatus == OrderStatus.Pending)
                    {
                        payment.PaymentStatus = PaymentStatus.Processing;
                    }
                    if (order.OrderStatus == OrderStatus.Processing && payment.PaymentStatus == PaymentStatus.Initiated)
                    {
                        payment.PaymentStatus = PaymentStatus.Processing;
                    }
                    if (order.OrderStatus == OrderStatus.Shipped)
                    {
                        payment.PaymentStatus = PaymentStatus.Processing;
                    }
                    if (order.OrderStatus == OrderStatus.Delivered)
                    {
                        payment.PaymentStatus = PaymentStatus.Completed;
                    }
                    if (order.OrderStatus == OrderStatus.Cancelled)
                    {
                        payment.PaymentStatus = PaymentStatus.Failed;
                        await _orderRepository.DeleteOrderAsync(order.OrderId);
                        // remove order detail
                        await _orderItemService.DeleteOrderItemByOrderIdAsync(order.OrderId);
                    }
                    payment.PaymentMethod = order.PaymentMethod;
                    payment.PaymentMethodText = payment.PaymentMethod.ToString();
                    payment.UpdatedAt = DateTime.UtcNow;

                    if (order.OrderStatus == OrderStatus.Delivered)
                    {
                       // add revenue report
                       var revenue = new RevenueReportAddDto
                       {
                            TotalRevenue = payment.Amount
                       };
                       await _revenueReportService.AddRevenueReportAsync(revenue);  
                    }
    
                    // add table history
                    var historyStatus = GetHistoryStatus(order.OrderStatus);
                    await _historyService.AddHistoryAsync(new History
                    {
                        CreateAt = DateTime.UtcNow.Date,
                        Message = historyStatus.ToString() + "_" + order.OrderId,
                        Status = historyStatus,
                        StatusMessage = historyStatus.ToString(),
                        PaymentId = payment.PaymentId
                    });
                }
                await _unitOfWork.SaveChangesAsync();

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
        public OrderStatus GetOrderStatus(OrderUpdate order)
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
        // Get History status for Order Status
        public HistoryStatus GetHistoryStatus(OrderStatus orderStatus)
        {
            switch (orderStatus)
            {
                case OrderStatus.Pending:
                    return HistoryStatus.OrderPending;
                case OrderStatus.Processing:
                    return HistoryStatus.OrderProcessing;
                case OrderStatus.Shipped:
                    return HistoryStatus.OrderShipped;
                case OrderStatus.Delivered:
                    return HistoryStatus.OrderDelivered;
                case OrderStatus.Cancelled:
                    return HistoryStatus.OrderCancelled;
                default:
                    return HistoryStatus.OrderPending;
            }
        }
        // Add Payment
        public async Task AddPaymentAsync(PaymentDto payment, int orderId, OrderStatus orderStatus)
        {
            
            await _paymentService.AddPaymentAsync(payment);
        }

        public async Task<ApiResponse<bool>> DeleteOrderItemsAsync(OrderItemDeleteDto orderItemDeleteDto)
        {
            var result = await _orderItemService.DeleteListOrderItemAsync(orderItemDeleteDto.OrderId, orderItemDeleteDto.OrderItemIds);
            if (result.Status)
            {
                return new ApiResponse<bool>
                {
                    Data = true,
                    Message = "Order items deleted",
                    Status = true
                };
            }
            return new ApiResponse<bool>
            {
                Data = false,
                Message = "Error deleting order items",
                Status = false
            };
        }
     
                     

    }
}