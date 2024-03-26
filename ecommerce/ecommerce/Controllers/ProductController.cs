using ecommerce.DTO;
using ecommerce.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var response = await _productService.GetAllProductsAsync();
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            var response = await _productService.GetProductByIdAsync(id);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetProductBySlugAsync(string slug)
        {
            var response = await _productService.GetProductBySlugAsync(slug);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategoryAsync(int categoryId)
        {
            var response = await _productService.GetProductsByCategoryAsync(categoryId);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("search")]
        public async Task<IActionResult> SearchProductsAsync(ProductSearchDto searchDTO)
        {
            var response = await _productService.SearchProductsAsync(searchDTO);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost]
        public async Task<IActionResult> AddProductAsync(ProductDto product)
        {
            var response = await _productService.AddProductAsync(product);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, ProductUpdateDto product)
        {
            var response = await _productService.UpdateProductAsync(id, product);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var response = await _productService.DeleteProductAsync(id);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}
