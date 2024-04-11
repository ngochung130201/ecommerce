using ecommerce.DTO;

namespace ecommerce.Services.Interface
{
    public interface IProductService
    {
        Task<ApiResponse<IEnumerable<ProductAllDto>>> GetAllProductsAsync();
        Task<ApiResponse<ProductAllDto>> GetProductByIdAsync(int id);
        Task<ApiResponse<ProductAllDto>> GetProductBySlugAsync(string slug);
        Task<ApiResponse<IEnumerable<ProductAllDto>>> GetProductsByCategoryAsync(int categoryId);
        Task<ApiResponse<IEnumerable<ProductAllDto>>> SearchProductsAsync(ProductSearchDto searchDTO);
        Task<ApiResponse<int>> AddProductAsync(ProductDto product, IFormFile image, List<IFormFile> gallery);
        Task<ApiResponse<int>> UpdateProductAsync(int id, ProductUpdateDto product, IFormFile? image = null, List<IFormFile>? gallery = null);
        Task<ApiResponse<int>> DeleteProductAsync(int id);
        // delete products
        Task<ApiResponse<int>> DeleteProductsAsync(List<int> ids);
        Task<ApiResponse<List<ProductAllDto>>> GetProductsByFilterAsync(ProductFilterDto popularDto);
        Task<ApiResponse<List<ProductAllDto>>> GetProductsByPaginationAsync(int pageSize, int pageNumber);
    }
}
