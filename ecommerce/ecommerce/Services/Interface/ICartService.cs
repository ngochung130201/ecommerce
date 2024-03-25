using ecommerce.DTO;
using ecommerce.Repository.Interface;

namespace ecommerce.Services.Interface
{
    public interface ICartService
    {
        Task<ApiResponse<IEnumerable<CartDto>>> GetAllCartsAsync(int userId);
        Task<ApiResponse<CartDto>> GetCartByIdAsync(int id);
        Task<ApiResponse<IEnumerable<CartDto>>> GetCartsByUserIdAsync(int userId);
        Task<ApiResponse<int>> AddCartAsync(CartDto cart, int productId);
        Task<ApiResponse<int>> UpdateCartAsync(int id, CartDto cart);
        Task<ApiResponse<int>> DeleteCartAsync(int id);
    }
}
