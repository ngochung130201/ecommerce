using ecommerce.DTO;
using ecommerce.Models;

namespace ecommerce.Services.Interface
{
    public interface IBlogService
    {
        Task<ApiResponse<bool>> CreateBlogAsync(BlogDto blogDto);
        Task<ApiResponse<bool>> UpdateBlogAsync(int id,BlogDto blog);
        Task<ApiResponse<bool>> DeleteBlogAsync(int id);

        Task<ApiResponse<List<BlogAllDto>>> GetAllBlogsAsync(int pageNumber, int pageSize);
        Task<ApiResponse<BlogAllDto>> GetBlogByIdAsync(int id);
        Task<ApiResponse<BlogAllDto>> GetBlogBySlugAsync(string slug);
        Task<ApiResponse<bool>> DeleteAllBlogsAsync();
        Task<ApiResponse<bool>> DeleteMultipleBlogsAsync(List<int> ids);
        Task<ApiResponse<List<BlogAllDto>>> SearchBlogsAsync(string searchTerm, int pageNumber, int pageSize);

        Task<ApiResponse<bool>> CreateBlogDetailAsync(int blogId, BlogDetailDto detail);
        Task<ApiResponse<bool>> UpdateBlogDetailAsync(int id,BlogDetailDto detail);
       Task<ApiResponse<bool>> DeleteBlogDetailAsync(int id);
        Task<ApiResponse<BlogDetailAllDto>> GetBlogDetailByIdAsync(int id);


        Task<ApiResponse<bool>> CreateBlogCategoryAsync(BlogCategoryDto category);
        Task<ApiResponse<bool>> UpdateBlogCategoryAsync(int id,BlogCategoryDto category);
        Task<ApiResponse<bool>> DeleteBlogCategoryAsync(int id);
        Task<ApiResponse<List<BlogCategoryAllDto>>> GetAllBlogCategoriesAsync(PagingForBlogCategory? paging = null);
        Task<ApiResponse<BlogCategoryAllDto>> GetBlogCategoryByIdAsync(int id);
        Task<ApiResponse<bool>> DeleteMultipleBlogCategoriesAsync(List<int> ids);
        Task<ApiResponse<bool>> DeleteAllBlogCategoriesAsync();
        Task<ApiResponse<BlogDetailAllDto>> GetBlogDetailByBlogIdAsync(int id);
    }
}
