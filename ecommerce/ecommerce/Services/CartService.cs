using ecommerce.DTO;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;

namespace ecommerce.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<ApiResponse<int>> AddCartAsync(CartDto cart)
        {
            var newCart = new Models.Cart
            {
                UserId = cart.UserId,
                CreatedAt = DateTime.Now,
            };
            await _cartRepository.AddCartAsync(newCart);
            return  new ApiResponse<int> {
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
            await _cartRepository.DeleteCartAsync(id);
            return new ApiResponse<int> { Message = "Cart deleted successfully", Status = true };
        }

        public async Task<ApiResponse<IEnumerable<CartDto>>> GetAllCartsAsync()
        {
            var carts = await _cartRepository.GetAllCartsAsync();
            if(carts == null)
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
            await _cartRepository.UpdateCartAsync(id, newCart,cartExist);
            return new ApiResponse<int> { Message = "Cart updated successfully", Status = true };
        }
    }
}
