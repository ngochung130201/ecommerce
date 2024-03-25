using ecommerce.DTO;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;

namespace ecommerce.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemService _cartItemService;
        public CartService(ICartRepository cartRepository,
        ICartItemService cartItemService)
        {
            _cartRepository = cartRepository;
            _cartItemService = cartItemService;
        }

        public async Task<ApiResponse<int>> AddCartAsync(CartDto cart, int productId)
        {
            var cartExist = await _cartRepository.GetCartByIdAsync(cart.CartId);
            if (cartExist != null)
            {
                var cartItems = await _cartItemService.GetCartItemsByCartIdAsync(cart.CartId, productId);
                if (cartItems != null)
                {
                    foreach (var cartItem in cartItems.Data)
                    {
                        cartItem.Quantity += cart.Quantity;
                    }
                    await _cartItemService.UpdateCartItemsAsync(cart.CartId, cartItems.Data.ToList());
                }
                else
                {
                    await _cartItemService.AddCartItemAsync(new CartItemDto
                    {
                        CartId = cart.CartId,
                        ProductId = productId,
                        Quantity = cart.Quantity
                    });

                }
            }
            else
            {
                var newCart = new Models.Cart
                {
                    UserId = cart.UserId,
                    CreatedAt = DateTime.Now,
                };
                _cartRepository.AddCart(newCart);
                await _cartItemService.AddCartItemAsync(new CartItemDto
                {
                    CartId = newCart.CartId,
                    ProductId = productId,
                    Quantity = cart.Quantity
                });
            }
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
            // Remove all cart items associated with the cart
            var cartItems = cart.CartItems;
            await _cartItemService.DeleteCartItemsByCartIdAsync(cartItems);
            return new ApiResponse<int> { Message = "Cart deleted successfully", Status = true };
        }

        public async Task<ApiResponse<IEnumerable<CartDto>>> GetAllCartsAsync(int userId)
        {
            // if userId == 0 then get all carts with role admin
            // else get all carts with userId with role user
            if (userId != 0)
            {
                var carts = await _cartRepository.GetAllCartsAsync();
                if (carts == null)
                {
                    return new ApiResponse<IEnumerable<CartDto>> { Message = "No carts found", Status = false };
                }
                var cartsDto = carts.Select(c => new CartDto
                {
                    CartId = c.CartId,
                    UserId = c.UserId,
                    CreatedAt = c.CreatedAt
                });
                return new ApiResponse<IEnumerable<CartDto>> { Data = cartsDto, Status = true };
            }
            else
            {
                var carts = await GetCartsByUserIdAsync(userId);
                if (carts == null)
                {
                    return new ApiResponse<IEnumerable<CartDto>> { Message = "No carts found", Status = false };
                }
                return carts;
            }

        }

        public async Task<ApiResponse<CartDto>> GetCartByIdAsync(int id)
        {
            var cart = await _cartRepository.GetCartByIdAsync(id);
            if (cart == null)
            {
                return new ApiResponse<CartDto> { Message = "Cart not found", Status = false };
            }
            var cartDto = new CartDto
            {
                CartId = cart.CartId,
                UserId = cart.UserId,
                CreatedAt = cart.CreatedAt
            };
            return new ApiResponse<CartDto> { Data = cartDto, Status = true };
        }

        public async Task<ApiResponse<IEnumerable<CartDto>>> GetCartsByUserIdAsync(int userId)
        {
            var carts = await _cartRepository.GetCartsByUserIdAsync(userId);
            if (carts == null)
            {
                return null;
            }
            var cartsDto = carts.Select(c => new CartDto
            {
                CartId = c.CartId,
                UserId = c.UserId,
                CreatedAt = c.CreatedAt
            });
            return new ApiResponse<IEnumerable<CartDto>> { Data = cartsDto, Status = true };
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
                CreatedAt = cart.CreatedAt
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
