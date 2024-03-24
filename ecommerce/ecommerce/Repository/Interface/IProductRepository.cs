using ecommerce.DTO;
using ecommerce.Models;

namespace ecommerce.Repository.Interface
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        // Search for products by name, price, category, etc.
        Task<IEnumerable<Product>> SearchProductsAsync(ProductSearchDto searchDTO);
        // Add a new product
        Task AddProductAsync(Product product);
        // Update a product by id
        Task UpdateProductAsync(int id, Product product, Product productExist);
        // Delete a product by id
        Task DeleteProductAsync(int id);
        
    }
}
