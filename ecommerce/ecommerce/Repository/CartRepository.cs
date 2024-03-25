using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;

namespace ecommerce.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly IRepositoryBase<Cart> _repositoryBase;
        public CartRepository(IRepositoryBase<Cart> repositoryBase)
        {
            _repositoryBase = repositoryBase;
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

        public async Task<IEnumerable<Cart>> GetAllCartsAsync()
        {
            return await _repositoryBase.FindAllAsync();
        }

        public Task<Cart> GetCartByIdAsync(int id)
        {
            var cart = _repositoryBase.FindByIdAsync(id);
            return cart;
        }

        public Task<IEnumerable<Cart>> GetCartsByUserIdAsync(int userId)
        {
            var carts = _repositoryBase.FindByConditionAsync(c => c.UserId == userId);
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
