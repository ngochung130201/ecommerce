using ecommerce.Context;
using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Repository
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly IRepositoryBase<CartItem> _repositoryBase;
        private readonly EcommerceContext _context;
        
        public CartItemRepository(IRepositoryBase<CartItem> repositoryBase, EcommerceContext context)
        {
            _repositoryBase = repositoryBase;
            _context = context;
        }
        public void AddCartItem(CartItem cartItem)
        {
            try
            {
                _repositoryBase.Create(cartItem);

            }
            catch (Exception ex)
            {
                throw new CustomException("Cart Item not added", 500);
            }
        }

        public void DeleteCartItem(CartItem? cartItem)
        {
            if (cartItem == null)
            {
                throw new CustomException("No Cart Item found", 404);
            }
            try
            {
                _repositoryBase.Delete(cartItem);

            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }

        public void DeleteCartItemsByCartId(IEnumerable<CartItem> cartItems)
        {
            if (cartItems == null)
            {
                throw new CustomException("No Cart Item found", 404);
            }
            try
            {
                _repositoryBase.DeleteRange(cartItems);

            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }

        }

        public async Task<IEnumerable<CartItem>> GetAllCartItemsAsync()
        {
            var cartItems = await _repositoryBase.FindAllAsync();
            if (cartItems == null)
            {
                throw new CustomException("No Cart Item found", 404);
            }
            return cartItems;
        }

        public async Task<CartItem> GetCartItemByIdAsync(int id)
        {
            var cartItem = await _repositoryBase.FindByIdAsync(id);
            if (cartItem == null)
            {
                return null;
            }
            return cartItem;
        }
        public async Task<CartItem> GetCartItemsByCartIdAsync(int cartId, int productId)
        {
            var cartItems = await _context.CartItems.FirstOrDefaultAsync(x => x.CartId == cartId && x.ProductId == productId);
            if (cartItems == null)
            {
                return null;
            }
            return cartItems;
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsByCartsIdAsync(int cartId)
        {
            var cart = await _context.CartItems.Where(x => x.CartId == cartId).ToListAsync();
            return cart; 
        }


        public void UpdateCartItem(CartItem? cartItemToUpdate, CartItem cartItem)
        {
            if (cartItemToUpdate == null)
            {
                throw new CustomException("Cart Item not found", 404);
            }
            try
            {
                cartItemToUpdate.Quantity = cartItem.Quantity;
                _repositoryBase.Update(cartItemToUpdate);

            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }

        public void UpdateCartsItem(IEnumerable<CartItem>? cartItemToUpdate, IEnumerable<CartItem> cartItem)
        {
            if (cartItemToUpdate == null)
            {
                throw new CustomException("Cart Item not found", 404);
            }
            try
            {
                _repositoryBase.UpdateRange(cartItem);
            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }
    }
}
