using ecommerce.DTO;
using ecommerce.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllWishlistsAsync()
        {
            var wishlists = await _wishlistService.GetAllWishlistsAsync();
            if (wishlists.Status)
            {
                return Ok(wishlists);
            }
            return BadRequest(wishlists);
        }
        // filter by PagingForWishlist
        [HttpPost("filter")]
        public async Task<IActionResult> GetWishlistsByFilterAsync(PagingForWishlist? paging = null)
        {
            var wishlists = await _wishlistService.GetAllWishlistsAsync(paging);
            if (wishlists.Status)
            {
                return Ok(wishlists);
            }
            return BadRequest(wishlists);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWishlistByIdAsync(int id)
        {
            var wishlist = await _wishlistService.GetWishlistByIdAsync(id);
            if (wishlist.Status)
            {
                return Ok(wishlist);
            }
            return BadRequest(wishlist);
        }
        [HttpPost]
        public async Task<IActionResult> AddWishlistAsync(WishlistRequestDto wishlist)
        {
            var result = await _wishlistService.AddWishlistAsync(wishlist);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWishlistAsync(int id)
        {
            var result = await _wishlistService.DeleteWishlistAsync(id);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        // wishlist by user id
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetWishlistByUserIdAsync(int userId)
        {
            var wishlist = await _wishlistService.GetWishListByUserIdAsync(userId);
            if (wishlist.Status)
            {
                return Ok(wishlist);
            }
            return BadRequest(wishlist);
        }
        // wishlist by product id
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetWishlistByProductIdAsync(int productId)
        {
            var wishlist = await _wishlistService.GetWishlistByProductIdAsync(productId);
            if (wishlist.Status)
            {
                return Ok(wishlist);
            }
            return BadRequest(wishlist);
        }
    }
}
