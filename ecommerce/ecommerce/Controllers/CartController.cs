using ecommerce.DTO;
using ecommerce.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCartsAsync()
        {
            var carts = await _cartService.GetAllCartsAsync();
            if (carts.Status)
            {
                return Ok(carts);
            }
            return BadRequest(carts);
        }
        /// <summary>
        /// Add cart to the database with the product id and cart item details 
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddCartAsync(CartDto cart)
        {
            var result = await _cartService.AddCartAsync(cart);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        /// <summary>
        /// Delete cart from the database by cart id remove all the cart items
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartAsync(int id)
        {
            var result = await _cartService.DeleteCartAsync(id);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        // remove cart item from the cart
        [HttpDelete("{cartId}/cartItem/{cartItemId}")]
        public async Task<IActionResult> DeleteCartItemAsync(int cartId, int cartItemId)
        {
            var result = await _cartService.DeleteCartItemAsync(cartId, cartItemId);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        // remove list of cart items 
        [HttpDelete("{cartId}/cartItems")]
        public async Task<IActionResult> DeleteCartItemsByCartIdAsync(int cartId, List<int> cartItemsId)
        {
            var result = await _cartService.DeleteCartItemsByCartIdAsync(cartId,cartItemsId);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
