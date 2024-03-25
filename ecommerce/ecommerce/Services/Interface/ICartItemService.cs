using ecommerce.DTO;
using ecommerce.Models;

namespace ecommerce.Services.Interface
{
    public interface ICartItemService
    {
        Task<ApiResponse<IEnumerable<CartItemDto>>> GetAllCartItemsAsync();
        Task<ApiResponse<CartItemDto>> GetCartItemByIdAsync(int id);
        Task AddCartItemAsync(CartItemDto cartItem);
        Task<ApiResponse<int>> DeleteCartItemAsync(int id);
        Task<ApiResponse<int>> UpdateCartItemAsync(int id, CartItemDto cartItem);
        Task<ApiResponse<int>> UpdateCartItemsAsync(int id, List<CartItemDto> cartItem);
        // Get cart items by cart id
        Task<ApiResponse<IEnumerable<CartItemDto>>> GetCartItemsByCartIdAsync(int cartId, int productId);
        Task<ApiResponse<int>> DeleteCartItemsByCartIdAsync(IEnumerable<CartItem> cartItems);
    }
}
