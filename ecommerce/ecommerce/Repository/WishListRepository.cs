using ecommerce.Context;
using ecommerce.DTO;
using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Repository
{
    public class WishListRepository : IWishListRepository
    {
        private readonly IRepositoryBase<Wishlist> _repositoryBase;
        private readonly EcommerceContext _context;
        public WishListRepository(IRepositoryBase<Wishlist> repositoryBase, EcommerceContext context)
        {
            _repositoryBase = repositoryBase;
            _context = context;
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

        public async Task<(IEnumerable<Wishlist>,int)> GetAllWishListsAsync(PagingForWishlist? paging = null)
        {
            var wishLists = _context.Wishlists.Include(k => k.User).Include(u => u.Product).ThenInclude(k => k.Category).AsQueryable();
            if (paging == null)
            {
                var wishListsDb = await wishLists.ToListAsync();
                return (wishListsDb, wishListsDb.Count);
            }
            if (!string.IsNullOrEmpty(paging.UserName))
            {
                wishLists = wishLists.Where(x => x.User.Username.Contains(paging.UserName) || x.User.Email.Contains(paging.UserName));
            }
            if (!string.IsNullOrEmpty(paging.ProductName))
            {
                wishLists = wishLists.Where(x => x.Product.Name.Contains(paging.ProductName));
            }
            if (paging.SortByDate)
            {
                wishLists = wishLists.OrderBy(x => x.CreatedAt);
            }
            else
            {
                wishLists = wishLists.OrderByDescending(x => x.CreatedAt);
            }
            if (wishLists == null)
            {
                throw new CustomException("No WishList found", 404);
            }
            var count = wishLists.Count();
            return (wishLists.Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize), count);
        }

        public async Task<Wishlist> GetWishListByIdAsync(int id)
        {
            var wishList = await _context.Wishlists.Include(k => k.User).Include(u => u.Product).ThenInclude(u => u.Category).FirstOrDefaultAsync(x => x.WishlistId == id);
            if (wishList == null)
            {
                throw new CustomException("WishList not found", 404);
            }
            return wishList;
        }

        public async Task<IEnumerable<Wishlist>> GetWishListByProductIdAsync(int productId)
        {
            var wishList = await _repositoryBase.FindByConditionAsync(x => x.ProductId == productId);
            if (wishList == null)
            {
                throw new CustomException("No WishList found", 404);
            }
            return wishList;
        }

        public async Task<IEnumerable<Wishlist>> GetWishListsByUserIdAsync(int userId)
        {
            var wishLists = await _context.Wishlists.Include(k => k.User).Include(u => u.Product).ThenInclude(k => k.Category).Where(x => x.UserId == userId).ToListAsync();
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
            wishListToUpdate.UpdatedAt = wishList.UpdatedAt;
            _repositoryBase.Update(wishListToUpdate);

        }
    }
}
