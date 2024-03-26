using ecommerce.Models;

namespace ecommerce.Repository.Interface
{
    public interface IWishListRepository
    {
        Task<IEnumerable<Wishlist>> GetAllWishListsAsync();
        Task<Wishlist> GetWishListByIdAsync(int id);
        Task<IEnumerable<Wishlist>> GetWishListsByUserIdAsync(int userId);
        Task AddWishListAsync(Wishlist wishList);
        Task UpdateWishListAsync(int id, Wishlist wishList);
        Task DeleteWishListAsync(int id);
        Task<IEnumerable<Wishlist>> GetWishListByProductIdAsync(int productId);

    }
}
