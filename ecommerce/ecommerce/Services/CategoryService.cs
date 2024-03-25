using ecommerce.DTO;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;

namespace ecommerce.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ApiResponse<int>> AddCategoryAsync(CategoryDto category)
        {
            var newCategory = new Category
            {
                Name = category.Name
            };
            await _categoryRepository.AddCategoryAsync(newCategory);
            return new ApiResponse<int>
            {
                Data = 0,
                Message = "Category added",
                Status = true
            };
        }

        public async Task<ApiResponse<int>> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "No Category found",
                    Status = false
                };
            }
            try
            {
                await _categoryRepository.DeleteCategoryAsync(id);
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "Category deleted",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = ex.Message,
                    Status = false
                };
            }
        }

        public async Task<ApiResponse<IEnumerable<CategoryDto>>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            if (categories == null)
            {
                return new ApiResponse<IEnumerable<CategoryDto>>
                {
                    Data = null,
                    Message = "No Category found",
                    Status = false
                };
            }
            return new ApiResponse<IEnumerable<CategoryDto>>
            {
                Data = categories.Select(x => new CategoryDto
                {
                    Name = x.Name
                }),
                Message = "Categories found",
                Status = true
            };
        }

        public async Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return new ApiResponse<CategoryDto>
                {
                    Data = null,
                    Message = "Category not found",
                    Status = false
                };
            }
            return new ApiResponse<CategoryDto>
            {
                Data = new CategoryDto
                {
                    Name = category.Name
                },
                Message = "Category found",
                Status = true
            };
        }

        public async Task<ApiResponse<int>> UpdateCategoryAsync(int id, CategoryDto category)
        {
            var categoryToUpdate = new Category
            {
                Name = category.Name
            };
            try
            {
                await _categoryRepository.UpdateCategoryAsync(id, categoryToUpdate);
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "Category updated",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = ex.Message,
                    Status = false
                };
            }
        }
    }
}
