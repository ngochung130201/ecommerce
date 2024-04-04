using ecommerce.DTO;
using ecommerce.Models;

namespace ecommerce.Repository.Interface
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(PagingForBlogCategory? paging = null);
        Task<Category> GetCategoryByIdAsync(int id);
        Task<Category> GetCategoryBySlugAsync(string slug);
        void AddCategory(Category category);
        void DeleteCategory(Category category);
        Task UpdateCategoryAsync(int id, Category category);
        // delete categories
        void DeleteCategories(List<int> ids);
    }
}
