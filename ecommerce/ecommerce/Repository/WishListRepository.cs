using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;

namespace ecommerce.Repository
{
    public class WishListRepository : IWishListRepository
    {
        private readonly IRepositoryBase<Wishlist> _repositoryBase;
        public WishListRepository(IRepositoryBase<Wishlist> repositoryBase)
        {
            _repositoryBase = repositoryBase;
        }
        public async Task AddWishListAsync(Wishlist wishList)
        {
            _repositoryBase.Create(wishList);
        }

        public async Task DeleteWishListAsync(int id)
        {
            var wishList = await _repositoryBase.FindByIdAsync(id);
            if (wishList == null)
            {
                throw new CustomException("No WishList found", 404);
            }
            try
            {
                _repositoryBase.Delete(wishList);
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }

        public async Task<IEnumerable<Wishlist>> GetAllWishListsAsync()
        {
            var wishLists = await _repositoryBase.FindAllAsync();
            if (wishLists == null)
            {
                throw new CustomException("No WishList found", 404);
            }
            return wishLists;
        }

        public async Task<Wishlist> GetWishListByIdAsync(int id)
        {
            var wishList = await _repositoryBase.FindByIdAsync(id);
            if (wishList == null)
            {
                throw new CustomException("WishList not found", 404);
            }
            return wishList;
        }

        public async Task<IEnumerable<Wishlist>> GetWishListsByUserIdAsync(int userId)
        {
            var wishLists = await _repositoryBase.FindByConditionAsync(x => x.UserId == userId);
            if (wishLists == null)
            {
                throw new CustomException("No WishList found", 404);
            }
            return wishLists;
        }

        public async Task UpdateWishListAsync(int id, Wishlist wishList)
        {
            var wishListToUpdate = await _repositoryBase.FindByIdAsync(id);
            if (wishListToUpdate == null)
            {
                throw new CustomException("WishList not found", 404);
            }
            wishListToUpdate.ProductId = wishList.ProductId;
            wishListToUpdate.UserId = wishList.UserId;
            _repositoryBase.Update(wishListToUpdate);

        }
    }
}
