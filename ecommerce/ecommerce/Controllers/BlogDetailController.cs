using ecommerce.DTO;
using ecommerce.Models;
using ecommerce.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Route("api/blog-detail")]
    [ApiController]
    public class BlogDetailController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogDetailController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        // POST: api/BlogDetail
        [HttpPost("{blogId}")]
        public async Task<IActionResult> CreateBlogDetail(int blogId, BlogDetailDto detail)
        {
           var result = await _blogService.CreateBlogDetailAsync(blogId, detail);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // PUT: api/BlogDetail/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlogDetail(int id, BlogDetailDto detail)
        {

          var result = await _blogService.UpdateBlogDetailAsync(id,detail);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        // DELETE: api/BlogDetail/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogDetail(int id)
        {
            var detail = await _blogService.GetBlogDetailByIdAsync(id);
            if (detail == null)
            {
                return NotFound();
            }

            var result = await _blogService.DeleteBlogDetailAsync(id);
            if (result.Status)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        
    }
}
