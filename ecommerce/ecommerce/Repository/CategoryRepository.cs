using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;

namespace ecommerce.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IRepositoryBase<Category> _repositoryBase;
        public CategoryRepository(IRepositoryBase<Category> repositoryBase)
        {
            _repositoryBase = repositoryBase;
        }

        public async Task AddCategoryAsync(Category category)
        {
            var newCategory = new Category
            {
                Name = category.Name
            };
            try
            {
                _repositoryBase.Create(newCategory);
            }
            catch (Exception ex)
            {
                throw new CustomException("Category not added", 500);
            }
        }

        public async Task DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _repositoryBase.FindByIdAsync(id);
                if (category == null)
                {
                    throw new CustomException("No Category found", 404);
                }
                _repositoryBase.Delete(category);
                throw new CustomException("Category deleted", 200, isSuccess: true);
            }
            catch (System.Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var categories = await _repositoryBase.FindAllAsync();
            if (categories == null)
            {
                throw new CustomException("No Category found", 404);
            }
            return categories;
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            var category = await _repositoryBase.FindByIdAsync(id);
            if (category == null)
            {
                throw new CustomException("Category not found", 404);
            }
            return category;
        }

        public async Task UpdateCategoryAsync(int id, Category category)
        {
            var categoryToUpdate = await _repositoryBase.FindByIdAsync(id);
            if (categoryToUpdate == null)
            {
                throw new CustomException("Category not found", 404);
            }
            categoryToUpdate.Name = category.Name;
            categoryToUpdate.Description = category.Description;
            categoryToUpdate.UpdatedAt = category.UpdatedAt;
            _repositoryBase.Update(categoryToUpdate);
        }
    }
}
