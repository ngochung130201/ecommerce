using ecommerce.Context;
using ecommerce.DTO;
using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;

namespace ecommerce.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IRepositoryBase<Category> _repositoryBase;
        private readonly EcommerceContext _context;
        public CategoryRepository(IRepositoryBase<Category> repositoryBase, EcommerceContext context)
        {
            _repositoryBase = repositoryBase;
            _context = context;
        }

        public void AddCategory(Category category)
        {
            var newCategory = new Category
            {
                Name = category.Name,
                Description = category.Description,
                Slug = category.Slug,
                CreatedAt = category.CreatedAt
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

        public async void DeleteCategories(List<int> ids)
        {
            try 
            {
                var categories = await _repositoryBase.FindByConditionAsync(c => ids.Contains(c.CategoryId));
                _repositoryBase.DeleteRange(categories);
            }
            catch (Exception ex)
            {
                throw new CustomException("Categories not deleted", 500);
            }
        }


        public void DeleteCategory(Category category)
        {
            try
            {
                _repositoryBase.Delete(category);
            }
            catch (Exception ex)
            {
                throw new CustomException("Category not deleted", 500);
            }
        }

        public async Task<(IEnumerable<Category>,int)> GetAllCategoriesAsync(PagingForBlogCategory? paging = null)
        {
            var categories = _context.Categories.AsQueryable();
            if (paging != null)
            {
            //    search
                if (!string.IsNullOrEmpty(paging.Name))
                {
                    categories = categories.Where(c => c.Name.Contains(paging.Name));
                }
                // sort
                if (paging.SortByDate)
                {
                    categories = categories.OrderBy(c => c.CreatedAt);
                }
                // pagination
                var total = categories.Count();
                return  (categories.Skip((paging.Page - 1) * paging.PageSize)
                    .Take(paging.PageSize), total);
            }
            if (categories == null)
            {
                throw new CustomException("No Category found", 404);
            }
            return (categories, categories.Count());
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

        public async Task<Category> GetCategoryBySlugAsync(string slug)
        {
            var category = await _repositoryBase.FindByConditionAsync(c => c.Slug == slug);
            if (category == null)
            {
                throw new CustomException("Category not found", 404);
            }
            return category.FirstOrDefault() ?? new Category();
        }

        public async Task UpdateCategoryAsync(int id, Category category)
        {
            var categoryToUpdate = await _repositoryBase.FindByIdAsync(id);
            if (categoryToUpdate == null)
            {
                throw new CustomException("Category not found", 404);
            }
            categoryToUpdate.Name = category.Name;
            categoryToUpdate.Slug = category.Slug;
            categoryToUpdate.Description = category.Description;
            categoryToUpdate.UpdatedAt = category.UpdatedAt;
            _repositoryBase.Update(categoryToUpdate);
        }
    }
}
