using ecommerce.DTO;
using ecommerce.Helpers;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;
using ecommerce.UnitOfWork;

namespace ecommerce.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<int>> AddCategoryAsync(CategoryDto category)
        {
            var newCategory = new Category
            {
                Name = category.Name,
                Description = category.Description,
                Slug = StringHelper.GenerateSlug(category.Name),
            };
            _categoryRepository.AddCategory(newCategory);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<int>
            {
                Data = newCategory.CategoryId,
                Message = "Category added",
                Status = true
            };
        }

        public async Task<ApiResponse<int>> DeleteCategoriesAsync(List<int> ids)
        {
            try
            {
                _categoryRepository.DeleteCategories(ids);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<int>
                {
                    Data = 0,
                    Message = "Categories deleted",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Data = 0,
                    Message = ex.Message,
                    Status = false
                };
            }
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
                _categoryRepository.DeleteCategory(category);
                await _unitOfWork.SaveChangesAsync();
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

        public async Task<ApiResponse<IEnumerable<CategoryAllDto>>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            if (categories == null)
            {
                return new ApiResponse<IEnumerable<CategoryAllDto>>
                {
                    Data = null,
                    Message = "No Category found",
                    Status = false
                };
            }
            return new ApiResponse<IEnumerable<CategoryAllDto>>
            {
                Data = categories.Select(x => new CategoryAllDto
                {
                    CategoryId = x.CategoryId,
                    Name = x.Name,
                    Description = x.Description,
                    UpdatedAt = x.UpdatedAt,
                    CreatedAt = x.CreatedAt,
                }),
                Message = "Categories found",
                Status = true
            };
        }

        public async Task<ApiResponse<CategoryAllDto>> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return new ApiResponse<CategoryAllDto>
                {
                    Data = null,
                    Message = "Category not found",
                    Status = false
                };
            }
            return new ApiResponse<CategoryAllDto>
            {
                Data = new CategoryAllDto
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    Description = category.Description,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt

                },
                Message = "Category found",
                Status = true
            };
        }

        public async Task<ApiResponse<CategoryAllDto>> GetCategoryBySlugAsync(string slug)
        {
            var category = await _categoryRepository.GetCategoryBySlugAsync(slug);
            if (category == null)
            {
                return new ApiResponse<CategoryAllDto>
                {
                    Data = null,
                    Message = "Category not found",
                    Status = false
                };
            }
            return new ApiResponse<CategoryAllDto>
            {
                Data = new CategoryAllDto
                {
                    CategoryId = category.CategoryId,
                    Name = category.Name,
                    Description = category.Description,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt
                },
                Message = "Category found",
                Status = true
            };
        }

        public async Task<ApiResponse<int>> UpdateCategoryAsync(int id, CategoryDto category)
        {
            var categoryToUpdate = new Category
            {
                Name = category.Name,
                Description = category.Description,
                UpdatedAt = DateTime.UtcNow,
                Slug = StringHelper.GenerateSlug(category.Name)
            };
            try
            {
                await _categoryRepository.UpdateCategoryAsync(id, categoryToUpdate);
                await _unitOfWork.SaveChangesAsync();
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
