﻿using ecommerce.DTO;

namespace ecommerce.Services.Interface
{
    public interface IProductReviewService
    {
        Task<ApiResponse<IEnumerable<ProductReviewAllDto>>> GetAllProductReviewsAsync();
        Task<ApiResponse<ProductReviewAllDto>> GetProductReviewByIdAsync(int id);
        Task<ApiResponse<int>> AddProductReviewAsync(ProductReviewDto productReview);
        Task<ApiResponse<int>> DeleteProductReviewAsync(int id);
        Task<ApiResponse<int>> UpdateProductReviewAsync(int id, ProductReviewUpdateDto productReview);
        Task<ApiResponse<IEnumerable<ProductReviewAllDto>>> GetProductReviewsByProductAsync(int productId);
        Task<ApiResponse<IEnumerable<ProductReviewAllDto>>> GetProductReviewsByUserAsync(int userId);
        // delete product reviews
        Task<ApiResponse<int>> DeleteProductReviewsAsync(List<int> ids);

    }
}
