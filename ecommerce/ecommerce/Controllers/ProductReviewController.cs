﻿using ecommerce.DTO;
using ecommerce.Services.Interface;
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
        // filter by PagingForProductReview
        [HttpPost("filter")]
        public async Task<IActionResult> GetProductReviewsByFilterAsync(PagingForProductReview? paging = null)
        {
            var productReviews = await _productReviewService.GetAllProductReviewsAsync(paging);
            if (productReviews.Status)
            {
                return Ok(productReviews);
            }
            return BadRequest(productReviews);
        }
        [HttpPost]
        public async Task<IActionResult> AddProductReviewAsync([FromBody]  ProductReviewDto productReview)
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
        public async Task<IActionResult> UpdateProductReviewAsync(int id, [FromForm] ProductReviewUpdateDto productReview)
        {
            var result = await _productReviewService.UpdateProductReviewAsync(id, productReview);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        // GET: api/product-review/product/1
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetProductReviewsByProductAsync(int productId)
        {
            var productReviews = await _productReviewService.GetProductReviewsByProductAsync(productId);
            if (productReviews.Status)
            {
                return Ok(productReviews);
            }
            return BadRequest(productReviews);
        }
        // GET: api/product-review/user/1
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetProductReviewsByUserAsync(int userId)
        {
            var productReviews = await _productReviewService.GetProductReviewsByUserAsync(userId);
            if (productReviews.Status)
            {
                return Ok(productReviews);
            }
            return BadRequest(productReviews);
        }
        // DELETE: api/product-review/delete
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteProductReviewsAsync(List<int> ids)
        {
            var result = await _productReviewService.DeleteProductReviewsAsync(ids);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        // GET: api/product-review/count
        [HttpGet("count")]
        public async Task<IActionResult> GetProductReviewCountByRatingAsync()
        {
            var result = await _productReviewService.GetProductReviewCountByRatingAsync();
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
