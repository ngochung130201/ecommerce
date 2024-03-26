using ecommerce.Models;

namespace ecommerce.Repository.Interface
{
    public interface IProductReviewRepository
    {
        Task<IEnumerable<ProductReview>> GetAllProductReviewsAsync();
        Task<ProductReview> GetProductReviewByIdAsync(int id);
        Task AddProductReviewAsync(ProductReview productReview);
        Task DeleteProductReviewAsync(int id);
        Task UpdateProductReviewAsync(int id, ProductReview productReview);
        Task<IEnumerable<ProductReview>> GetProductReviewsByProductAsync(int productId);
        Task<IEnumerable<ProductReview>> GetProductReviewsByUserAsync(int userId);
    }
}
