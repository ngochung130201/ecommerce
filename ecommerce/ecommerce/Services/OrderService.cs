﻿using ecommerce.Context;
using ecommerce.DTO;
using ecommerce.Enums;
using ecommerce.Helpers;
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
        private readonly ICartService _cartService;
        private readonly EcommerceContext _context;
        private readonly IPaymentService _paymentService;
        private readonly IHistoryService _historyService;
        private readonly IUploadFilesService _uploadFilesService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRevenueReportService _revenueReportService;
        public OrderService(IOrderRepository orderRepository,
        IPaymentService paymentService, IOrderItemService orderItemService,
        EcommerceContext context,
        ICartService cartService, IHistoryService historyService,
        IRevenueReportService revenueReportService,
        IUploadFilesService uploadFilesService,
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
            _uploadFilesService = uploadFilesService;
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
            var orderStatusNew = new OrderStatus[]
            {
                OrderStatus.Delivered,
                OrderStatus.Shipped,
                OrderStatus.Cancelled
            };
            var cartItemsToProcess = getCartByIdAsync.CartItems.ToList();

            var products = cartItemsToProcess.Select(u => u.Product).ToList(); // have db
            var productInput = cartItemsToProcess.Select(u => u.Product).Where(u => order.OrderProduct.Any(k => k.ProductId == u.ProductId)).ToList();
            var productIds = cartItemsToProcess.Select(u => u.ProductId).ToList();
            var isDupProduct = productIds.Any(u => order.OrderProduct.Any(k => k.ProductId == u));
            // khac ngay hom nay van tao order moi
            var isToday = getCartByIdAsync.CreatedAt == DateTime.UtcNow;
            // Create a new order if it doesn't exist
            if (!isDupProduct || !isToday || orderIdByUser == null || orderStatusNew.Contains(orderIdByUser.OrderStatus))
            {
                var newOrder = new Order
                {
                    UserId = order.UserId,
                    OrderStatus = OrderStatus.Pending,
                    OrderStatusMessage = nameof(OrderStatus.Pending),
                    CreatedAt = DateTime.UtcNow,
                    Address = order.Address,
                    Note = order.Note,
                    PhoneNumber = order.PhoneNumber,
                };
                await _orderRepository.AddOrderAsync(newOrder);
                await _unitOfWork.SaveChangesAsync();
                // add table history
            }
            orderIdByUser = await _orderRepository.GetOrderByUserIdAsync(order.UserId);
            if (!cartItemsToProcess.Any())
            {
                cartItemsToProcess = orderIdByUser.User.Carts.CartItems.ToList();
            }
            foreach (var cartItem in cartItemsToProcess)
            {
                var productExist = order.OrderProduct.FirstOrDefault(k => k.ProductId == cartItem.ProductId);
                int quantity = 0;
                if (!order.OrderProduct.Any(x=>x.Quantity == 0))
                {
                    quantity = productExist.Quantity;
                }
                else
                {
                    quantity = cartItem.Quantity;
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
                orderItem.Quantity += quantity;
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
            if (productInput.Count == products.Count)
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
            var order = await _context.Orders.Include(u => u.User).ThenInclude(x=>x.Carts).ThenInclude(k=>k.CartItems).Include(u => u.OrderItems).FirstOrDefaultAsync(u => u.OrderId == id);
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
                if (order.User.Carts != null)
                {
                    if(order.User.Carts.CartItems.Any())
                    {
                       await _cartService.DeleteListCartItemAsync(order.User.Carts.CartItems.ToList());
                    }
                    await _cartService.DeleteCartAsync(order.UserId);
                }
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

        public async Task<ApiResponse<IEnumerable<OrderDto>>> GetAllOrdersAsync(PagingForOrder? paging = null)
        {
            var orders = await _orderRepository.GetAllOrdersAsync(paging);
            var total = await _context.Orders.CountAsync();
            if (orders.Item1 == null)
            {
                return new ApiResponse<IEnumerable<OrderDto>>
                {
                    Data = null,
                    Message = "No Order found",
                    Status = false
                };
            }
            var result =new ApiResponse<IEnumerable<OrderDto>>
            {
                Data = orders.Item1.Select(order => new OrderDto
                {
                    OrderStatus = order.OrderStatus,
                    UserId = order.UserId,
                    CreatedAt = order.CreatedAt,
                    OrderId = order.OrderId,
                    UpdatedAt = order.UpdatedAt,
                    TotalPrice = order.TotalPrice,
                    Address = order.Address,
                    Note = order.Note,
                    User = new UserDto
                    {
                        UserId = order.User.UserId,
                        Username = order.User.Username,
                        Email = order.User.Email,
                        CreatedAt = order.User.CreatedAt,
                        UpdatedAt = order.User.UpdatedAt,
                    },
                    PhoneNumber = order.PhoneNumber,
                    OrderItems = order.OrderItems.Select(u => new OrderItemDto
                    {
                        OrderId = u.OrderId,
                        ProductId = u.ProductId,
                        Quantity = u.Quantity,
                        PriceAtTimeOfOrder = u.PriceAtTimeOfOrder,
                        Product = new ProductAllDto
                        {
                            Price = u.Product.Price,
                            PriceSale = u.Product.PriceSale,
                            Name = u.Product.Name,
                            ProductId = u.Product.ProductId,
                            Image = _uploadFilesService.GetFilePath(u.Product.Image, Contains.ProductImageFolder),
                            CategoryName = u.Product.Category.Name,
                            CategoryId = u.Product.Category.CategoryId,
                            Popular = u.Product.Popular,
                            Slug = u.Product.Slug,
                            InventoryCount = u.Product.InventoryCount,
                            Sale = u.Product.Sale,

                        }
                    }).ToList()
                }),
                Message = "Orders retrieved",
                Status = true,
                Total = orders.Item2,
            };
            if (paging != null)
            {
                var (page, pageSize, TotalPage) = Helpers.Paging.GetPaging(paging.Page, paging.PageSize, total);
                result.Page = page;
                result.PageSize = pageSize;
                result.TotalPage = TotalPage;
            }
            return result;
        }

        public async Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int id)
        {
            var order = await _context.Orders.Include(u => u.User).Include(u => u.OrderItems).ThenInclude(u => u.Product).ThenInclude(u => u.Category).FirstOrDefaultAsync(u => u.OrderId == id);
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
                    TotalPrice = order.TotalPrice,
                    Address = order.Address,
                    Note = order.Note,
                    PhoneNumber = order.PhoneNumber,
                    User = new UserDto
                    {
                        UserId = order.User.UserId,
                        Username = order.User.Username,
                        Email = order.User.Email,
                        CreatedAt = order.User.CreatedAt,
                        UpdatedAt = order.User.UpdatedAt,
                    },
                    OrderItems = order.OrderItems.Select(u => new OrderItemDto
                    {
                        OrderId = u.OrderId,
                        ProductId = u.ProductId,
                        Quantity = u.Quantity,
                        PriceAtTimeOfOrder = u.PriceAtTimeOfOrder,
                        Product = new ProductAllDto
                        {
                            Price = u.Product.Price,
                            PriceSale = u.Product.PriceSale,
                            Name = u.Product.Name,
                            ProductId = u.Product.ProductId,
                            Image = _uploadFilesService.GetFilePath(u.Product.Image, Contains.ProductImageFolder),
                            CategoryName = u.Product.Category.Name,
                            Slug = u.Product.Slug,
                            InventoryCount = u.Product.InventoryCount,
                            Sale = u.Product.Sale,
                        }
                    }).ToList()
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
                var orderExist = await _context.Orders.Include(u => u.User).Include(u => u.OrderItems).ThenInclude(u => u.Product).ThenInclude(u => u.Category).FirstOrDefaultAsync(u => u.OrderId == order.OrderId);

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
                orderExist.Address = order.Address;
                orderExist.Note = order.Note;
                orderExist.PhoneNumber = order.PhoneNumber;
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
                    // update product InventoryCount
                    if(order.OrderStatus == OrderStatus.Delivered)
                    {
                        var orderItem = orderExist.OrderItems;
                        var getProductsId = orderItem.Select(x => x.ProductId);
                        var products = await _context.Products.Where(x => getProductsId.Contains(x.ProductId)).ToListAsync();

                        foreach (var item in products)
                        {
                            item.InventoryCount = item.InventoryCount - orderItem.Where(x => x.ProductId == item.ProductId).Sum(u => u.Quantity);

                        }

                    }
                    if (order.OrderStatus == OrderStatus.Delivered)
                    {
                        // add revenue report
                        var revenue = new RevenueReportAddDto
                        {
                            TotalRevenue = payment.Amount,
                            Date = DateTime.Now,

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

        public async Task<ApiResponse<IEnumerable<OrderDto>>> GetOrdersByUserIdAsync(int userId, PagingForOrder? paging = null)
        {
            var orders = await _context.Orders.Include(x => x.User).Include(u => u.OrderItems).ThenInclude(x => x.Product).ThenInclude(i => i.Category).Where(x => x.UserId == userId).ToListAsync();
            if (paging == null)
            {
                return new ApiResponse<IEnumerable<OrderDto>>
                {
                    Data = orders.Select(order => new OrderDto
                    {
                        OrderStatus = order.OrderStatus,
                        UserId = order.UserId,
                        CreatedAt = order.CreatedAt,
                        OrderId = order.OrderId,
                        UpdatedAt = order.UpdatedAt,
                        TotalPrice = order.TotalPrice,
                        Address = order.Address,
                        Note = order.Note,
                        PhoneNumber = order.PhoneNumber,
                        User = new UserDto
                        {
                            UserId = order.User.UserId,
                            Username = order.User.Username,
                            Email = order.User.Email,
                            CreatedAt = order.User.CreatedAt,
                            UpdatedAt = order.User.UpdatedAt,
                        },
                        OrderItems = order.OrderItems.Select(u => new OrderItemDto
                        {
                            OrderId = u.OrderId,
                            ProductId = u.ProductId,
                            Quantity = u.Quantity,
                            PriceAtTimeOfOrder = u.PriceAtTimeOfOrder,
                            Product = new ProductAllDto
                            {
                                Price = u.Product.Price,
                                PriceSale = u.Product.PriceSale,
                                Name = u.Product.Name,
                                ProductId = u.Product.ProductId,
                                Image = _uploadFilesService.GetFilePath(u.Product.Image, Contains.ProductImageFolder),
                                CategoryName = u.Product.Category.Name,
                                Slug = u.Product.Slug,
                                InventoryCount = u.Product.InventoryCount,
                                Sale = u.Product.Sale,
                            }
                        }).ToList()
                    }),
                    Message = "Orders retrieved",
                    Status = true
                };
            }
            if (!string.IsNullOrEmpty(paging.UserName))
            {
                orders = orders.Where(x => x.User.Username.Contains(paging.UserName) || x.User.Email.Contains(paging.UserName)).ToList();
            }
            if (paging.MinTotalPrice > 0)
            {
                orders = orders.Where(x => x.TotalPrice >= paging.MinTotalPrice).ToList();
            }
            if (paging.MaxTotalPrice > 0)
            {
                orders = orders.Where(x => x.TotalPrice <= paging.MaxTotalPrice).ToList();
            }
            if (paging.SortByDate)
            {
                orders = orders.OrderBy(x => x.CreatedAt).ToList();
            }
            else
            {
                orders = orders.OrderByDescending(x => x.CreatedAt).ToList();
            }
            if (paging.OrderStatus != null)
            {
                orders = orders.Where(x => x.OrderStatus == paging.OrderStatus).ToList();
            }
            if (paging.Page > 0 && paging.PageSize > 0)
            {
                orders = orders.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize).ToList();
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
                    TotalPrice = order.TotalPrice,
                    Address = order.Address,
                    Note = order.Note,
                    PhoneNumber = order.PhoneNumber,
                    User = new UserDto
                    {
                        UserId = order.User.UserId,
                        Username = order.User.Username,
                        Email = order.User.Email,
                        CreatedAt = order.User.CreatedAt,
                        UpdatedAt = order.User.UpdatedAt,
                    },
                    OrderItems = order.OrderItems.Select(u => new OrderItemDto
                    {
                        OrderId = u.OrderId,
                        ProductId = u.ProductId,
                        Quantity = u.Quantity,
                        PriceAtTimeOfOrder = u.PriceAtTimeOfOrder,
                        Product = new ProductAllDto
                        {
                            Price = u.Product.Price,
                            PriceSale = u.Product.PriceSale,
                            Name = u.Product.Name,
                            ProductId = u.Product.ProductId,
                            Image = _uploadFilesService.GetFilePath(u.Product.Image, Contains.ProductImageFolder),
                            CategoryName = u.Product.Category.Name,
                            Slug = u.Product.Slug,
                            InventoryCount = u.Product.InventoryCount,
                            Sale = u.Product.Sale,
                        }
                    }).ToList()
                }),
                Message = "Orders retrieved",
                Status = true
            };

        }
    }
}