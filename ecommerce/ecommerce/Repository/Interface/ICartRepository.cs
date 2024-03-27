using ecommerce.Models;

namespace ecommerce.Repository.Interface
{
    public interface ICartRepository
    {
        Task<IEnumerable<Cart>> GetAllCartsAsync();
        Task<Cart> GetCartByIdAsync(int id);
        Task<Cart> GetCartsByUserIdAsync(int userId);
        void AddCart(Cart cart);
        void UpdateCart(Cart cart, Cart cartExist);
        void DeleteCart(Cart cart);
    }
}
