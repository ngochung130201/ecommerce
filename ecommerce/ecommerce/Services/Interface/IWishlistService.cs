using ecommerce.DTO;

namespace ecommerce.Services.Interface
{
    public interface IWishlistService
    {
        Task<ApiResponse<IEnumerable<WishlistDto>>> GetAllWishlistsAsync();
        Task<ApiResponse<WishlistDto>> GetWishlistByIdAsync(int id);
        Task<ApiResponse<int>> AddWishlistAsync(WishlistRequestDto wishlist);
        Task<ApiResponse<int>> DeleteWishlistAsync(int id);
        Task<ApiResponse<int>> UpdateWishlistAsync(int id, WishlistRequestDto wishlist);
        Task<ApiResponse<IEnumerable<WishlistDto>>> GetWishListByUserIdAsync(int userId);
        Task<ApiResponse<IEnumerable<WishlistDto>>> GetWishlistByProductIdAsync(int productId);

    }
}
