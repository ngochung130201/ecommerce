using ecommerce.DTO;
using ecommerce.Models;

namespace ecommerce.Repository.Interface
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> GetProductBySlugAsync(string slug);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        // Search for products by name, price, category, etc.
        Task<IEnumerable<Product>> SearchProductsAsync(ProductSearchDto searchDTO);
        // Add a new product
        void AddProduct(Product product);
        // Update a product by id
        void UpdateProduct(Product product, Product productExist);
        // Delete a product by id
        void DeleteProduct(Product? product);

    }
}
