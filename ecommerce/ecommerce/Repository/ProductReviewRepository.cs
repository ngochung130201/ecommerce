using ecommerce.Context;
using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;

namespace ecommerce.Repository
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly IRepositoryBase<ProductReview> _repositoryBase;
        private readonly EcommerceContext _context;
        public ProductReviewRepository(IRepositoryBase<ProductReview> repositoryBase, EcommerceContext context)
        {
            _repositoryBase = repositoryBase;
            _context = context;
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

        public async Task DeleteProductReviewsAsync(List<int> ids)
        {
            var productReviews = await _repositoryBase.FindByConditionAsync(p => ids.Contains(p.ReviewId));
            _context.ProductReviews.RemoveRange(productReviews);
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

        public async Task<IEnumerable<ProductReview>> GetProductReviewsByProductAsync(int productId)
        {
            var productReviews = await _repositoryBase.FindByConditionAsync(p => p.ProductId == productId);
            return productReviews;
        }

        public Task<IEnumerable<ProductReview>> GetProductReviewsByUserAsync(int userId)
        {
            var productReviews = _repositoryBase.FindByConditionAsync(p => p.UserId == userId);
            return productReviews;
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
            productReviewToUpdate.UpdatedAt = productReview.UpdatedAt;
        }
    }
}
