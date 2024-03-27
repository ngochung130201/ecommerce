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
        // Get cart items by cart id
        Task<ApiResponse<IEnumerable<CartItemDto>>> GetCartItemsByCartsIdAsync(int cartId);
        Task<ApiResponse<int>> DeleteCartItemsByCartIdAsync(IEnumerable<CartItem> cartItems);
    }
}
