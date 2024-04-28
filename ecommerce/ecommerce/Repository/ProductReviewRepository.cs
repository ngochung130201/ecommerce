using ecommerce.Context;
using ecommerce.DTO;
using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using Microsoft.EntityFrameworkCore;

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


        public async Task<(IEnumerable<ProductReview>,int)> GetAllProductReviewsAsync(PagingForProductReview? paging = null)
        {
            var productReviews = _context.ProductReviews.Include(u=>u.User).Include(k=>k.Product).AsQueryable();
            if (paging == null)
            {
                var productReviewsDb = await productReviews.ToListAsync();
                return (productReviewsDb, productReviewsDb.Count);}
            if (!string.IsNullOrEmpty(paging.UserName))
            {
                productReviews = productReviews.Where(p => p.User.Email.Contains(paging.UserName) || p.User.Username.Contains(paging.UserName));
            }
            if (!string.IsNullOrEmpty(paging.ProductName))
            {
                productReviews = productReviews.Where(p => p.Product.Name.Contains(paging.ProductName));
            }
            if (paging.MinRating != 0)
            {
                productReviews = productReviews.Where(p => p.Rating >= paging.Rating);
            }
            if (paging.MaxRating != 0)
            {
                productReviews = productReviews.Where(p => p.Rating <= paging.Rating);
            }
            if (paging.Rating != 0)
            {
                productReviews = productReviews.Where(p => p.Rating == paging.Rating);
            }

            if (paging.SortByDate)
            {
                productReviews = productReviews.OrderBy(p => p.CreatedAt);
            }else{
                productReviews = productReviews.OrderByDescending(p => p.CreatedAt);
            }
            var total = productReviews.Count();
            return (productReviews.Skip((paging.Page - 1) * paging.PageSize)
                .Take(paging.PageSize), total);
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
