using ecommerce.DTO;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;
using ecommerce.UnitOfWork;

namespace ecommerce.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemService _cartItemService;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IAccountRepository<User> _accountRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CartService(ICartRepository cartRepository,
        ICartItemService cartItemService, ICartItemRepository cartItemRepository,
        IAccountRepository<User> accountRepository, IUnitOfWork unitOfWork,
        IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _cartItemService = cartItemService;
            _unitOfWork = unitOfWork;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
            _accountRepository = accountRepository;
        }

        public async Task<ApiResponse<int>> AddCartAsync(CartDto cart)
        {
            // kiểm tra xem user đã có cart chưa
            var cartExistByUserId = await _cartRepository.GetCartsByUserIdAsync(cart.UserId);
            var product = await _productRepository.GetProductByIdAsync(cart.ProductId);
            if (cartExistByUserId != null)
            {
                // nếu có rồi thì không thêm nữa mà update số lượng và giá tiền
                var cartItemExistByProductId = await _cartItemRepository.GetCartItemsByCartIdAsync(cartExistByUserId.CartId, productId: cart.ProductId);
                if (cartItemExistByProductId == null)
                {
                    // add cart item to the cart
                    var newCartItems = new CartItem
                    {
                        ProductId = cart.ProductId,
                        Quantity = cart.Quantity,
                        CartId = cartExistByUserId.CartId,
                    };
                    newCartItems.TotalPrice = product.PriceSale * cart.Quantity;
                    _cartItemRepository.AddCartItem(newCartItems);
                    cartExistByUserId.TotalPrice += newCartItems.TotalPrice;
                }
                else
                {
                    cartItemExistByProductId.Quantity += cart.Quantity;
                    cartItemExistByProductId.TotalPrice = product.Price * cartItemExistByProductId.Quantity;

                }
            }
            else
            {
                // nếu chưa có cart thì tạo mới voi user
                var cartOfUser = await _accountRepository.GetByIdForUser(cart.UserId);
                var cartItemExist = await _cartItemRepository.GetCartItemsByCartIdAsync(cart.CartId, productId: cart.ProductId);
                if (cartItemExist == null)
                {
                    var newCartItems = new CartItem
                    {
                        ProductId = cart.ProductId,
                        Quantity = cart.Quantity,
                        CartId = cart.CartId,
                    };
                    newCartItems.TotalPrice = product.PriceSale * cart.Quantity;
                    _cartItemRepository.AddCartItem(newCartItems);
                    if (cartOfUser.Carts == null)
                    {
                        cartOfUser.Carts = new Cart
                        {
                            UserId = cart.UserId,
                            TotalPrice = newCartItems.TotalPrice,
                            CartItems = new List<CartItem> { newCartItems },
                            CreatedAt = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        cartOfUser.Carts.TotalPrice += newCartItems.TotalPrice;
                        cartOfUser.Carts.CartItems.Add(newCartItems);
                    }
                }
                else
                {
                    cartItemExist.Quantity += cart.Quantity;
                    cartItemExist.TotalPrice = product.PriceSale * cartItemExist.Quantity;
                }
            }
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<int>
            {
                Data = cart.CartId,
                Message = "Cart added successfully",
                Status = true
            };
        }

        public async Task<ApiResponse<int>> DeleteCartAsync(int id)
        {
            var cart = await _cartRepository.GetCartByIdAsync(id);
            if (cart == null)
            {
                return new ApiResponse<int> { Message = "Cart not found", Status = false };
            }
            _cartRepository.DeleteCart(cart);
            // remove total price from the cart
            return new ApiResponse<int> { Message = "Cart deleted successfully", Status = true };
        }

        public async Task<ApiResponse<int>> DeleteCartItemAsync(int cartId, int cartItemId)
        {
            var cartItem = await _cartItemRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null)
            {
                return new ApiResponse<int> { Message = "Cart Item not found", Status = false };
            }
            // remove total price from the cart
            var cart = await _cartRepository.GetCartByIdAsync(cartId);
            cart.TotalPrice -= cartItem.TotalPrice;
            _cartItemRepository.DeleteCartItem(cartItem);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<int> { Message = "Cart Item deleted successfully", Status = true };
        }

        public async Task<ApiResponse<int>> DeleteCartItemsByCartIdAsync(int cartId, List<int> cartItemId)
        {
            var cartItems = await _cartItemRepository.GetCartItemsByCartsIdAsync(cartId);
            if (cartItems == null)
            {
                return new ApiResponse<int> { Message = "Cart Items not found", Status = false };
            }
            // remove total price from the cart
            var cart = await _cartRepository.GetCartByIdAsync(cartId);
            foreach (var item in cartItems)
            {
                cart.TotalPrice -= item.TotalPrice;
            }
            _cartItemRepository.DeleteCartItemsByCartId(cartItems);
            if (cart.CartItems.Count == cartItems.Count())
            {
                _cartRepository.DeleteCart(cart);
            }
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<int> { Message = "Cart Items deleted successfully", Status = true };
        }

        public async Task<ApiResponse<int>> DeleteListCartItemAsync(List<CartItem> cartItems)
        {
            try
            {
                _cartItemRepository.DeleteCartItemsByCartId(cartItems);
                return new ApiResponse<int> { Message = "Cart Items deleted successfully", Status = true };
            }
            catch (System.Exception)
            {
                return new ApiResponse<int> { Message = "Cart Items not found", Status = false };
            }
        }

        public async Task<ApiResponse<IEnumerable<CartAllDto>>> GetAllCartsAsync()
        {
            // if userId == 0 then get all carts with role admin
            // else get all carts with userId with role user

            var carts = await _cartRepository.GetAllCartsAsync();
            if (carts == null)
            {
                return new ApiResponse<IEnumerable<CartAllDto>> { Message = "No carts found", Status = false };
            }
            var cartsDto = carts.Select(c => new CartAllDto
            {
                CartId = c.CartId,
                UserId = c.UserId,
                CartItems = c.CartItems.Select(ci => new CartItemDto
                {
                    CartItemId = ci.CartItemId,
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    TotalPrice = ci.TotalPrice,
                    CartId = ci.CartId,
                    Product = new ProductAllDto {
                        CategoryId = ci.Product.CategoryId,
                        Description = ci.Product.Description,
                        Image = ci.Product.Image,
                        Price = ci.Product.Price,
                        PriceSale = ci.Product.PriceSale,
                        ProductId = ci.Product.ProductId,
                        CreatedAt = ci.Product.CreatedAt,
                        UpdatedAt = ci.Product.UpdatedAt,
                        Name = ci.Product.Name,
                        CategoryName = ci.Product.Category.Name,
                    }

                }).ToList()
            });
            return new ApiResponse<IEnumerable<CartAllDto>> { Data = cartsDto, Status = true };

        }

        public async Task<ApiResponse<CartAllDto>> GetCartByIdAsync(int id)
        {
            var cart = await _cartRepository.GetCartByIdAsync(id);
            if (cart == null)
            {
                return new ApiResponse<CartAllDto> { Message = "Cart not found", Status = false };
            }
            var cartDto = new CartAllDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                CartItems = cart.CartItems.Select(ci => new CartItemDto
                {
                    CartItemId = ci.CartItemId,
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    TotalPrice = ci.TotalPrice,
                    CartId = ci.CartId,
                }).ToList()
            };
            return new ApiResponse<CartAllDto> { Data = cartDto, Status = true };
        }

        public async Task<ApiResponse<CartAllDto>> GetCartsByUserIdAsync(int userId)
        {
            var cart = await _cartRepository.GetCartsByUserIdAsync(userId);
            if (cart == null)
            {
                return null;
            }
            var cartDto = new CartAllDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                CartItems = cart.CartItems.Select(ci => new CartItemDto
                {
                    CartItemId = ci.CartItemId,
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    TotalPrice = ci.TotalPrice,
                    CartId = ci.CartId,
                }).ToList()

            };
            return new ApiResponse<CartAllDto> { Data = cartDto, Status = true };
        }

        public async Task<ApiResponse<int>> UpdateCartAsync(int id, CartDto cart)
        {
            var cartExist = await _cartRepository.GetCartByIdAsync(id);
            if (cartExist == null)
            {
                return new ApiResponse<int> { Message = "Cart not found", Status = false };
            }
            var newCart = new Models.Cart
            {
                UserId = cart.UserId,
            };
            var cartItems = await _cartItemService.GetCartItemByIdAsync(id);
            if (cartItems == null)
            {
                return new ApiResponse<int> { Message = "Cart Items not found", Status = false };
            }
            _cartRepository.UpdateCart(newCart, cartExist);
            return new ApiResponse<int> { Message = "Cart updated successfully", Status = true };
        }
    }
}
