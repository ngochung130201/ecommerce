using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;

namespace ecommerce.Repository
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly IRepositoryBase<ProductReview> _repositoryBase;
        public ProductReviewRepository(IRepositoryBase<ProductReview> repositoryBase)
        {
            _repositoryBase = repositoryBase;
        }
        public async Task AddProductReviewAsync(ProductReview productReview)
        {
            try
            {
                _repositoryBase.Create(productReview);

            }
            catch (Exception ex)
            {
                throw new CustomException("Product Review not added", 500);
            }
        }

        public async Task DeleteProductReviewAsync(int id)
        {
            var productReview = await _repositoryBase.FindByIdAsync(id);
            if (productReview == null)
            {
                throw new CustomException("No Product Review found", 404);
            }
            try
            {
                _repositoryBase.Delete(productReview);

            }
            catch (Exception ex)
            {
                throw new CustomException(ex.Message, 500);
            }
        }

        public async Task<IEnumerable<ProductReview>> GetAllProductReviewsAsync()
        {
            var productReviews = await _repositoryBase.FindAllAsync();
            if (productReviews == null)
            {
                throw new CustomException("No Product Review found", 404);
            }
            return productReviews;
        }

        public async Task<ProductReview> GetProductReviewByIdAsync(int id)
        {
            var productReview = await _repositoryBase.FindByIdAsync(id);
            if (productReview == null)
            {
                throw new CustomException("Product Review not found", 404);
            }
            return productReview;
        }

        public async Task UpdateProductReviewAsync(int id, ProductReview productReview)
        {
            var productReviewToUpdate = await _repositoryBase.FindByIdAsync(id);
            if (productReviewToUpdate == null)
            {
                throw new CustomException("Product Review not found", 404);
            }
            productReviewToUpdate.Rating = productReview.Rating;
            productReviewToUpdate.Comment = productReview.Comment;
            productReview.UpdatedAt = DateTime.UtcNow;
        }
    }
}
