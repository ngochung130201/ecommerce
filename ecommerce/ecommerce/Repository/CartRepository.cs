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
        public async Task AddCartAsync(Cart cart)
        {
            var newCart = new Cart
            {
                UserId = cart.UserId,
                CreatedAt = DateTime.Now,
            };
            _repositoryBase.Create(newCart);
            await _repositoryBase.SaveAsync();
        }

        public async Task DeleteCartAsync(int id)
        {
            var cart = await _repositoryBase.FindByIdAsync(id);
            _repositoryBase.Delete(cart);
        }

        public Task<IEnumerable<Cart>> GetAllCartsAsync()
        {
            return _repositoryBase.FindAllAsync();
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

        public Task UpdateCartAsync(int id, Cart cart, Cart cartExist)
        {
            cartExist.UserId = cart.UserId;
            cartExist.CreatedAt = cart.CreatedAt;
            _repositoryBase.Update(cartExist);
            return _repositoryBase.SaveAsync();
        }
    }
}
