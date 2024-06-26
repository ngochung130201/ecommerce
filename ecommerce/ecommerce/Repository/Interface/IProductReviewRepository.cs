﻿using ecommerce.DTO;
using ecommerce.Models;

namespace ecommerce.Repository.Interface
{
    public interface IProductReviewRepository
    {
        Task<(IEnumerable<ProductReview>,int)>  GetAllProductReviewsAsync(PagingForProductReview? paging = null);
        Task<ProductReview> GetProductReviewByIdAsync(int id);
        Task AddProductReviewAsync(ProductReview productReview);
        Task DeleteProductReviewAsync(int id);
        Task UpdateProductReviewAsync(int id, ProductReview productReview);
        Task<IEnumerable<ProductReview>> GetProductReviewsByProductAsync(int productId);
        Task<IEnumerable<ProductReview>> GetProductReviewsByUserAsync(int userId);
        // delete product reviews
        Task DeleteProductReviewsAsync(List<int> ids);
    }
}
