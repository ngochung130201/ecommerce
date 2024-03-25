using ecommerce.DTO;

namespace ecommerce.Services.Interface
{
    public interface IProductReviewService
    {
        Task<ApiResponse<IEnumerable<ProductReviewDto>>> GetAllProductReviewsAsync();
        Task<ApiResponse<ProductReviewDto>> GetProductReviewByIdAsync(int id);
        Task<ApiResponse<int>> AddProductReviewAsync(ProductReviewDto productReview);
        Task<ApiResponse<int>> DeleteProductReviewAsync(int id);
        Task<ApiResponse<int>> UpdateProductReviewAsync(int id, ProductReviewDto productReview);
    }
}
