using ecommerce.Models;

namespace ecommerce.Repository.Interface
{
    public interface ICartItemRepository
    {
        Task<IEnumerable<CartItem>> GetAllCartItemsAsync();
        Task<CartItem> GetCartItemByIdAsync(int id);
        void AddCartItem(CartItem cartItem);
        void DeleteCartItem(CartItem? cartItem);
        void UpdateCartItem(CartItem? cartItemToUpdate, CartItem cartItem);
        void UpdateCartsItem(IEnumerable<CartItem>? cartItemToUpdate, IEnumerable<CartItem> cartItem);
        // Get cart items by cart id
        Task<IEnumerable<CartItem>> GetCartItemsByCartIdAsync(int cartId, int productId);
        void DeleteCartItemsByCartId(IEnumerable<CartItem> cartItems);
    }
}
