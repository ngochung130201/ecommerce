using ecommerce.Context;
using ecommerce.DTO;
using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly IRepositoryBase<Cart> _repositoryBase;
        private readonly EcommerceContext _context;
        public CartRepository(IRepositoryBase<Cart> repositoryBase, EcommerceContext context)
        {
            _repositoryBase = repositoryBase;
            _context = context;
        }
        public void AddCart(Cart cart)
        {
            var newCart = new Cart
            {
                UserId = cart.UserId,
                CreatedAt = DateTime.Now,
            };
            _repositoryBase.Create(newCart);

        }

        public void DeleteCart(Cart? cart)
        {
            if (cart == null)
            {
                throw new CustomException("No Cart found", 404);
            }
            _repositoryBase.Delete(cart);
        }

        public async Task<(IEnumerable<Cart>, int)> GetAllCartsAsync(PagingForCart? paging)
        {
            if (paging == null)
            {
                var cartsDb = await _context.Carts.Include(i => i.User).Include(u => u.CartItems).ThenInclude(u => u.Product).ThenInclude(u => u.Category).
               ToListAsync();
               return (cartsDb, cartsDb.Count);
            }
            var carts = await _context.Carts.Include(i => i.User).Include(u => u.CartItems).ThenInclude(u => u.Product).ThenInclude(u => u.Category)
                .Where(u => (paging.MinTotalPrice == 0 || u.TotalPrice >= paging.MinTotalPrice)
                        && (paging.MaxTotalPrice == 0 || u.TotalPrice <= paging.MaxTotalPrice) &&
                            (string.IsNullOrEmpty(paging.UserName) || u.User.Username.Contains(paging.UserName) || u.User.Email.Contains(paging.UserName))
                        )
                .OrderByDescending(x => x.CreatedAt).ToListAsync();
            if (paging.SortByDate)
            {
                carts = carts.OrderBy(u => u.CreatedAt).ToList();
            }
            var count = carts.Count();
            return (carts.Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize), count);
        }

        public async Task<Cart> GetCartByIdAsync(int id)
        {
            var cart = await _context.Carts.Include(u => u.User).Include(u => u.CartItems).ThenInclude(u => u.Product).ThenInclude(u => u.Category).FirstOrDefaultAsync(c => c.CartId == id);
            return cart;
        }

        public async Task<Cart> GetCartsByUserIdAsync(int userId)
        {
            var carts = await _context.Carts.Include(u => u.User).Include(u => u.CartItems).ThenInclude(k => k.Product).ThenInclude(u => u.Category).FirstOrDefaultAsync(c => c.UserId == userId);
            return carts;
        }


        public void UpdateCart(Cart cart, Cart cartExist)
        {
            cartExist.UserId = cart.UserId;
            cartExist.UpdatedAt = cart.UpdatedAt;
            _repositoryBase.Update(cartExist);
        }
    }
}
