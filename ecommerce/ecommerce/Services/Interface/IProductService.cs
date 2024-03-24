using ecommerce.DTO;

namespace ecommerce.Services.Interface
{
    public interface IProductService
    {
        Task<ApiResponse<IEnumerable<ProductDto>>> GetAllProductsAsync();
        Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id);
        Task<ApiResponse<IEnumerable<ProductDto>>> GetProductsByCategoryAsync(int categoryId);
        Task<ApiResponse<IEnumerable<ProductDto>>> SearchProductsAsync(ProductSearchDto searchDTO);
        Task<ApiResponse<int>> AddProductAsync(ProductDto product);
        Task<ApiResponse<int>> UpdateProductAsync(int id, ProductDto product);
        Task<ApiResponse<int>> DeleteProductAsync(int id);
    }
}
