using ecommerce.DTO;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;

namespace ecommerce.Services
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly IProductReviewRepository _productReviewRepository;
        public ProductReviewService(IProductReviewRepository productReviewRepository)
        {
            _productReviewRepository = productReviewRepository;
        }

        public async Task<ApiResponse<int>> AddProductReviewAsync(ProductReviewDto productReview)
        {
            var newProductReview = new ProductReview
            {
                ProductId = productReview.ProductId,
                Rating = productReview.Rating,
                Comment = productReview.Comment
            };
            await _productReviewRepository.AddProductReviewAsync(newProductReview);
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

        public async Task<ApiResponse<IEnumerable<ProductReviewDto>>> GetAllProductReviewsAsync()
        {
            var productReviews = await _productReviewRepository.GetAllProductReviewsAsync();
            if (productReviews == null)
            {
                return new ApiResponse<IEnumerable<ProductReviewDto>>
                {
                    Data = null,
                    Message = "No Product Review found",
                    Status = false
                };
            }
            return new ApiResponse<IEnumerable<ProductReviewDto>>
            {
                Data = productReviews.Select(x => new ProductReviewDto
                {
                    ProductId = x.ProductId,
                    Rating = x.Rating,
                    Comment = x.Comment
                }),
                Message = "Product Reviews found",
                Status = true
            };
        }

        public async Task<ApiResponse<ProductReviewDto>> GetProductReviewByIdAsync(int id)
        {
            var productReview = await _productReviewRepository.GetProductReviewByIdAsync(id);
            if (productReview == null)
            {
                return new ApiResponse<ProductReviewDto>
                {
                    Data = null,
                    Message = "Product Review not found",
                    Status = false
                };
            }
            return new ApiResponse<ProductReviewDto>
            {
                Data = new ProductReviewDto
                {
                    ProductId = productReview.ProductId,
                    Rating = productReview.Rating,
                    Comment = productReview.Comment
                },
                Message = "Product Review found",
                Status = true
            };
        }

        public async Task<ApiResponse<int>> UpdateProductReviewAsync(int id, ProductReviewDto productReview)
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
            var updatedProductReview = new ProductReview
            {
                ProductId = productReview.ProductId,
                Rating = productReview.Rating,
                Comment = productReview.Comment
            };
            try
            {
                await _productReviewRepository.UpdateProductReviewAsync(id, updatedProductReview);
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
