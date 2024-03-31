using ecommerce.DTO;
using ecommerce.Models;
using ecommerce.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Route("api/blog-category")]
    [ApiController]
    public class BlogCategoryController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogCategoryController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        // GET: api/BlogCategory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogCategory>>> GetAllBlogCategories()
        {
          var categories = await _blogService.GetAllBlogCategoriesAsync();
          if(categories.Status)
          {
            return Ok(categories);
          }
            return BadRequest(categories);
        }

        // GET: api/BlogCategory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogCategory>> GetBlogCategoryById(int id)
        {
            var category = await _blogService.GetBlogCategoryByIdAsync(id);

            if (category == null || category.Status == false)
            {
                return NotFound();
            }
            return Ok(category);
        }

        // POST: api/BlogCategory
        [HttpPost]
        public async Task<IActionResult> CreateBlogCategory(BlogCategoryDto category)
        {
           var result = await _blogService.CreateBlogCategoryAsync(category);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // PUT: api/BlogCategory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlogCategory(int id, BlogCategoryDto category)
        {

          var result = await _blogService.UpdateBlogCategoryAsync(id,category);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // DELETE: api/BlogCategory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogCategory(int id)
        {
            var category = await _blogService.GetBlogCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var result = await _blogService.DeleteBlogCategoryAsync(id);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        // delete blog category all
        [HttpDelete]
        public async Task<IActionResult> DeleteAllBlogCategory()
        {
            var result = await _blogService.DeleteAllBlogCategoriesAsync();
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        // delete multiple blog categories
        [HttpDelete("delete-multiple")]
        public async Task<IActionResult> DeleteMultipleBlogCategories(List<int> ids)
        {
            var result = await _blogService.DeleteMultipleBlogCategoriesAsync(ids);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
