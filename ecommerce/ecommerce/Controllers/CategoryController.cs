using ecommerce.DTO;
using ecommerce.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            if (categories.Status)
            {
                return Ok(categories);
            }
            return BadRequest(categories);
        }
        // filter category by paging
        [HttpPost("filter")]
        public async Task<IActionResult> GetAllCategoriesAsync(PagingForBlogCategory? paging = null)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(paging);
            if (categories.Status)
            {
                return Ok(categories);
            }
            return BadRequest(categories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category.Status)
            {
                return Ok(category);
            }
            return BadRequest(category);
        }
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetCategoryBySlugAsync(string slug)
        {
            var category = await _categoryService.GetCategoryBySlugAsync(slug);
            if (category.Status)
            {
                return Ok(category);
            }
            return BadRequest(category);
        }
        [HttpPost]
        public async Task<IActionResult> AddCategoryAsync(CategoryDto category)
        {
            var result = await _categoryService.AddCategoryAsync(category);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryAsync(int id, CategoryDto category)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, category);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
