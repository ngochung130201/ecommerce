using ecommerce.DTO;

namespace ecommerce.Services.Interface
{
    public interface ICategoryService
    {
        Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllCategoriesAsync();
        Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(int id);
        Task<ApiResponse<int>> AddCategoryAsync(CategoryDto category);
        Task<ApiResponse<int>> DeleteCategoryAsync(int id);
        Task<ApiResponse<int>> UpdateCategoryAsync(int id, CategoryDto category);
    }
}
