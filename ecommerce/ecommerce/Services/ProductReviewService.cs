using ecommerce.Context;
using ecommerce.DTO;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;
using ecommerce.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly EcommerceContext _context;
        public ProductReviewService(IProductReviewRepository productReviewRepository, IUnitOfWork unitOfWork, EcommerceContext context)
        {
            _productReviewRepository = productReviewRepository;
            _unitOfWork = unitOfWork;
            _context = context;
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


        public async Task<ApiResponse<IEnumerable<ProductReviewAllDto>>> GetAllProductReviewsAsync(int page, int pageSize)
        {
            // page size
            var productReviews = await _context.ProductReviews.Include(u => u.User).Skip((page - 1) * pageSize).Take(pageSize).OrderByDescending(u => u.CreatedAt).ToListAsync();
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
                    UserId = x.UserId,
                    UserName = x.User.Username
                }),
                Message = "Product Reviews found",
                Status = true
            };
        }

        public async Task<ApiResponse<ProductReviewAllDto>> GetProductReviewByIdAsync(int id)
        {
            var productReview = await _context.ProductReviews.Include(u => u.User).FirstOrDefaultAsync(x => x.ReviewId == id);
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
                    UserId = productReview.UserId,
                    UserName = productReview.User.Username

                },
                Message = "Product Review found",
                Status = true
            };
        }

        public async Task<ApiResponse<object>> GetProductReviewCountByRatingAsync()
        {
            var productReviews = await _context.ProductReviews.GroupBy(x => x.Rating).Select(x => new ProductReviewCountDto
            {
                Rating = x.Key,
                Count = x.Count(),
            }).ToListAsync();

            // Phần trăm số lượng review theo rating
            var objectReviews = productReviews.Select(x => new
            {
                Rating = x.Rating,
                Count = x.Count,
            });

            // phần trăm số lượng review theo rating dưới dạng số nguyên 4.5 -> 5
            var totalCount = objectReviews.Select(x => x.Count).Sum();
            var totalRating = productReviews.Sum(x => x.Rating);
            // Làm tròn lên
            var percent = Math.Ceiling((double)totalRating / totalCount);
            var objectResult = new
            {
                ObjectReviews = objectReviews,
                percent = percent,
                totalCount = totalCount

            };

            return new ApiResponse<object>
            {
                Data = objectResult,
                Message = "Product Review count by rating",
                Status = true
            };

        }

        public async Task<ApiResponse<IEnumerable<ProductReviewAllDto>>> GetProductReviewsByProductAsync(int productId)
        {
            var productReviews = await _context.ProductReviews.Include(u => u.User).Where(x => x.ProductId == productId).ToListAsync();
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
                    UserId = x.UserId,
                    UserName = x.User.Username
                }),
                Message = "Product Reviews found",
                Status = true
            };

        }

        public async Task<ApiResponse<IEnumerable<ProductReviewAllDto>>> GetProductReviewsByUserAsync(int userId)
        {
            var productReviews = await _context.ProductReviews.Include(u => u.User).Where(x => x.UserId == userId).ToListAsync();
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
                    UserId = x.UserId,
                    UserName = x.User.Username
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
