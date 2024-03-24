using ecommerce.Models;

namespace ecommerce.Repository.Interface
{
    public interface ICartRepository
    {
        Task<IEnumerable<Cart>> GetAllCartsAsync();
        Task<Cart> GetCartByIdAsync(int id);
        Task<IEnumerable<Cart>> GetCartsByUserIdAsync(int userId);
        Task AddCartAsync(Cart cart);
        Task UpdateCartAsync(int id, Cart cart, Cart cartExist);
        Task DeleteCartAsync(int id);
    }
}
