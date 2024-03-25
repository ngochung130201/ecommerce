using ecommerce.DTO;
using ecommerce.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Route("api/product-review")]
    [ApiController]
    public class ProductReviewController : ControllerBase
    {
        private readonly IProductReviewService _productReviewService;
        public ProductReviewController(IProductReviewService productReviewService)
        {
            _productReviewService = productReviewService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProductReviewsAsync()
        {
            var productReviews = await _productReviewService.GetAllProductReviewsAsync();
            if (productReviews.Status)
            {
                return Ok(productReviews);
            }
            return BadRequest(productReviews);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductReviewByIdAsync(int id)
        {
            var productReview = await _productReviewService.GetProductReviewByIdAsync(id);
            if (productReview.Status)
            {
                return Ok(productReview);
            }
            return BadRequest(productReview);
        }
        [HttpPost]
        public async Task<IActionResult> AddProductReviewAsync(ProductReviewDto productReview)
        {
            var result = await _productReviewService.AddProductReviewAsync(productReview);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductReviewAsync(int id)
        {
            var result = await _productReviewService.DeleteProductReviewAsync(id);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductReviewAsync(int id, ProductReviewDto productReview)
        {
            var result = await _productReviewService.UpdateProductReviewAsync(id, productReview);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
