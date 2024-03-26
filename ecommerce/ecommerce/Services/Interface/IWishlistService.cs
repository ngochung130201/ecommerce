using ecommerce.DTO;

namespace ecommerce.Services.Interface
{
    public interface IWishlistService
    {
        Task<ApiResponse<IEnumerable<WishlistDto>>> GetAllWishlistsAsync();
        Task<ApiResponse<WishlistDto>> GetWishlistByIdAsync(int id);
        Task<ApiResponse<int>> AddWishlistAsync(WishlistDto wishlist);
        Task<ApiResponse<int>> DeleteWishlistAsync(int id);
        Task<ApiResponse<int>> UpdateWishlistAsync(int id, WishlistDto wishlist);
        Task<ApiResponse<IEnumerable<WishlistDto>>> GetWishListByUserIdAsync(int userId);
        Task<ApiResponse<IEnumerable<WishlistDto>>> GetWishlistByProductIdAsync(int productId);

    }
}
