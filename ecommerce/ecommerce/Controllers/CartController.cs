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
        public async Task<IActionResult> GetAllCartsAsync([FromQuery] int userId = 0)
        {
            var carts = await _cartService.GetAllCartsAsync(userId);
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
        public async Task<IActionResult> AddCartAsync(CartDto cart, int productId)
        {
            var result = await _cartService.AddCartAsync(cart, productId);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        /// <summary>
        /// Delete cart from the database by cart id
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
        /// <summary>
        /// Update cart in the database by cart id and cart details and add order, order details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cart"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCartAsync(int id, CartDto cart)
        {
            var result = await _cartService.UpdateCartAsync(id, cart);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

    }
}
