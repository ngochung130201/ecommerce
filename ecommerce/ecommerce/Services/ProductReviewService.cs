using ecommerce.DTO;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;
using ecommerce.UnitOfWork;

namespace ecommerce.Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductReviewService(IProductReviewRepository productReviewRepository, IUnitOfWork unitOfWork)
        {
            _productReviewRepository = productReviewRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<int>> AddProductReviewAsync(ProductReviewDto productReview)
        {
            var newProductReview = new ProductReview
            {
                Rating = productReview.Rating,
                Comment = productReview.Comment,
                CreatedAt = DateTime.UtcNow,
                ProductId = productReview.ProductId,
                UserId = productReview.UserId,
            };
            await _productReviewRepository.AddProductReviewAsync(newProductReview);
            await _unitOfWork.SaveChangesAsync();
            return new ApiResponse<int>
            {
                Data = 0,
                Message = "Product Review added",
                Status = true
            };
        }

        public async Task<ApiResponse<int>> DeleteProductReviewAsync(int id)
        {
            var productReview = await _productReviewRepository.GetProductReviewByIdAsync(id);
            if (productReview == null)
            {
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "No Product Review found",
                    Status = false
                };
            }
            try
            {
                await _productReviewRepository.DeleteProductReviewAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "Product Review deleted",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = ex.Message,
                    Status = false
                };
            }
        }

        public async Task<ApiResponse<int>> DeleteProductReviewsAsync(List<int> ids)
        {
            try
            {
                var productReviews = _productReviewRepository.DeleteProductReviewsAsync(ids);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<int>
                {
                    Data = 0,
                    Message = "Product Reviews deleted",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Data = 0,
                    Message = ex.Message,
                    Status = false
                };
            }
        }


        public async Task<ApiResponse<IEnumerable<ProductReviewAllDto>>> GetAllProductReviewsAsync()
        {
            var productReviews = await _productReviewRepository.GetAllProductReviewsAsync();
            if (productReviews == null)
            {
                return new ApiResponse<IEnumerable<ProductReviewAllDto>>
                {
                    Data = null,
                    Message = "No Product Review found",
                    Status = false
                };
            }
            return new ApiResponse<IEnumerable<ProductReviewAllDto>>
            {
                Data = productReviews.Select(x => new ProductReviewAllDto
                {
                    ProductId = x.ProductId,
                    Rating = x.Rating,
                    Comment = x.Comment,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    ReviewId = x.ReviewId,
                    UserId = x.UserId
                }),
                Message = "Product Reviews found",
                Status = true
            };
        }

        public async Task<ApiResponse<ProductReviewAllDto>> GetProductReviewByIdAsync(int id)
        {
            var productReview = await _productReviewRepository.GetProductReviewByIdAsync(id);
            if (productReview == null)
            {
                return new ApiResponse<ProductReviewAllDto>
                {
                    Data = null,
                    Message = "Product Review not found",
                    Status = false
                };
            }
            return new ApiResponse<ProductReviewAllDto>
            {
                Data = new ProductReviewAllDto
                {
                    ProductId = productReview.ProductId,
                    Rating = productReview.Rating,
                    Comment = productReview.Comment,
                    CreatedAt = productReview.CreatedAt,
                    UpdatedAt = productReview.UpdatedAt,
                    ReviewId = productReview.ReviewId,
                    UserId = productReview.UserId

                },
                Message = "Product Review found",
                Status = true
            };
        }

        public async Task<ApiResponse<IEnumerable<ProductReviewAllDto>>> GetProductReviewsByProductAsync(int productId)
        {
            var productReviews = await _productReviewRepository.GetProductReviewsByProductAsync(productId);
            if (productReviews == null)
            {
                return new ApiResponse<IEnumerable<ProductReviewAllDto>>
                {
                    Data = null,
                    Message = "No Product Review found",
                    Status = false
                };
            }
            return new ApiResponse<IEnumerable<ProductReviewAllDto>>
            {
                Data = productReviews.Select(x => new ProductReviewAllDto
                {
                    ProductId = x.ProductId,
                    Rating = x.Rating,
                    Comment = x.Comment,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    ReviewId = x.ReviewId,
                    UserId = x.UserId
                }),
                Message = "Product Reviews found",
                Status = true
            };

        }

        public async Task<ApiResponse<IEnumerable<ProductReviewAllDto>>> GetProductReviewsByUserAsync(int userId)
        {
            var productReviews = await _productReviewRepository.GetProductReviewsByUserAsync(userId);
            if (productReviews == null)
            {
                return new ApiResponse<IEnumerable<ProductReviewAllDto>>
                {
                    Data = null,
                    Message = "No Product Review found",
                    Status = false
                };
            }
            return new ApiResponse<IEnumerable<ProductReviewAllDto>>
            {
                Data = productReviews.Select(x => new ProductReviewAllDto
                {
                    ProductId = x.ProductId,
                    Rating = x.Rating,
                    Comment = x.Comment,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    ReviewId = x.ReviewId,
                    UserId = x.UserId
                }),
                Message = "Product Reviews found",
                Status = true
            };

        }


        public async Task<ApiResponse<int>> UpdateProductReviewAsync(int id, ProductReviewUpdateDto productReview)
        {
            var productReviewToUpdate = await _productReviewRepository.GetProductReviewByIdAsync(id);
            if (productReviewToUpdate == null)
            {
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "Product Review not found",
                    Status = false
                };
            }
            try
            {
                var updatedProductReview = new ProductReview
                {
                    Rating = productReview.Rating,
                    Comment = productReview.Comment,
                    UpdatedAt = DateTime.UtcNow
                };
                await _productReviewRepository.UpdateProductReviewAsync(id, updatedProductReview);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "Product Review updated",
                    Status = true
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = ex.Message,
                    Status = false
                };
            }
        }
    }
}
