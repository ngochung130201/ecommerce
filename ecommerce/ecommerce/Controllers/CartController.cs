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
        private readonly ICartItemService _cartItemService;
        public CartController(ICartService cartService, ICartItemService cartItemService)
        {
            _cartService = cartService;
            _cartItemService = cartItemService;
            
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
        // filter cart by paging
        [HttpPost("filter")]
        public async Task<IActionResult> GetAllCartsAsync(PagingForCart? paging = null)
        {
            var carts = await _cartService.GetAllCartsAsync(paging);
            if (carts.Status)
            {
                return Ok(carts);
            }
            return BadRequest(carts);
        }
        // get cart by user id
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetCartsByUserIdAsync(int userId)
        {
            var cart = await _cartService.GetCartsByUserIdAsync(userId);
            if (cart.Status)
            {
                return Ok(cart);
            }
            return BadRequest(cart);
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
            var result = await _cartService.DeleteCartItemsByCartIdAsync(cartId, cartItemsId);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        //// update cart item by cart item id
        //[HttpPut("{cartId}/cartItem/{cartItemId}")]
        //public async Task<IActionResult> UpdateCartItemAsync(CartItemDto cartItemDto)
        //{
        //    // var result = await _cartItemService.UpdateCartItemAsync(cartItemDto);
        //    // if (result.Status)
        //    // {
        //    //     return Ok(result);
        //    // }
        //    // return BadRequest(result);
        //    return Ok();
        //}
    }
}
