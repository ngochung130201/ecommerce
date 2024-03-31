using ecommerce.DTO;
using ecommerce.Models;
using ecommerce.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Route("api/blog")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogsController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        // GET: api/Blog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> GetAllBlogs(int pageNumber = 1, int pageSize = 10)
        {
            var blogs = await _blogService.GetAllBlogsAsync(pageNumber, pageSize);
            return Ok(blogs);
        }

        // GET: api/Blog/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Blog>> GetBlogById(int id)
        {
            var blog = await _blogService.GetBlogByIdAsync(id);

            if (blog == null || blog.Status == false)
            {
                return NotFound();
            }
            return Ok(blog);
        }

        // POST: api/Blog
        [HttpPost]
        public async Task<IActionResult> CreateBlog(BlogDto blogDto)
        {
            var result = await _blogService.CreateBlogAsync(blogDto);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // PUT: api/Blog/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlog(int id, BlogDto blog)
        {

           var result = await _blogService.UpdateBlogAsync(id,blog);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // DELETE: api/Blog/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await _blogService.GetBlogByIdAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

           var result = await _blogService.DeleteBlogAsync(id);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        // search blog and pagination
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Blog>>> SearchBlog(string keyword, int pageNumber = 1, int pageSize = 10)
        {
            var blogs = await _blogService.SearchBlogsAsync(keyword, pageNumber, pageSize);
            if (blogs == null || blogs.Status == false)
            {
                return NotFound();
            }
            return Ok(blogs);
        }
        // page number and page size
        [HttpGet("page")]
        public async Task<ActionResult<IEnumerable<Blog>>> GetBlogsByPage(int pageNumber = 1, int pageSize = 10)
        {
            var blogs = await _blogService.GetAllBlogsAsync(pageNumber, pageSize);
            if (blogs == null || blogs.Status == false)
            {
                return NotFound();
            }
            return Ok(blogs);
        }
        // delete all blogs
        [HttpDelete]
        public async Task<IActionResult> DeleteAllBlogs()
        {
            var result = await _blogService.DeleteAllBlogsAsync();
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        // Delete multiple blogs
        [HttpDelete("delete-multiple")]
        public async Task<IActionResult> DeleteMultipleBlogs(List<int> ids)
        {
            var result = await _blogService.DeleteMultipleBlogsAsync(ids);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
