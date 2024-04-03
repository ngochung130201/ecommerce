using ecommerce.DTO;
using ecommerce.Models;
using ecommerce.Repository.Interface;

namespace ecommerce.Services.Interface
{
    public interface ICartService
    {
        Task<ApiResponse<IEnumerable<CartAllDto>>> GetAllCartsAsync(PagingForCart? paging = null);
        Task<ApiResponse<CartAllDto>> GetCartByIdAsync(int id);
        Task<ApiResponse<CartAllDto>> GetCartsByUserIdAsync(int userId);
        Task<ApiResponse<int>> AddCartAsync(CartDto cart);
        Task<ApiResponse<int>> UpdateCartAsync(int id, CartDto cart);
        Task<ApiResponse<int>> DeleteCartAsync(int id);
        Task<ApiResponse<int>> DeleteCartItemAsync(int cartId, int cartItemId);
        Task<ApiResponse<int>> DeleteListCartItemAsync(List<CartItem> cartItems);
        Task<ApiResponse<int>> DeleteCartItemsByCartIdAsync(int cartId, List<int> cartItemId);
        // Update cart item quantity
        Task UpdateCartItemQuantityAsync(int cartId, int cartItemId, int quantity);
    }
}
