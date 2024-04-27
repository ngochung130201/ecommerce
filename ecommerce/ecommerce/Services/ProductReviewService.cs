using ecommerce.Context;
using ecommerce.DTO;
using ecommerce.Helpers;
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
        private readonly IUploadFilesService _uploadFilesService;
        private readonly EcommerceContext _context;
        public ProductReviewService(IProductReviewRepository productReviewRepository, IUnitOfWork unitOfWork, EcommerceContext context, IUploadFilesService uploadFilesService)
        {
            _productReviewRepository = productReviewRepository;
            _unitOfWork = unitOfWork;
            _context = context;
            _uploadFilesService = uploadFilesService;
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
                // remove image
                if (!string.IsNullOrEmpty(productReview.Image))
                {
                    await _uploadFilesService.RemoveFileAsync(productReview.Image, Contains.ProductReviewImageFolder);
                }
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
                var productReviews = await _context.ProductReviews.Where(x => ids.Contains(x.ReviewId)).ToListAsync();
                foreach(var productReview in productReviews)
                {
                    if (!string.IsNullOrEmpty(productReview.Image))
                    {
                        await _uploadFilesService.RemoveFileAsync(productReview.Image, Contains.ProductReviewImageFolder);
                    }
                }
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


        public async Task<ApiResponse<IEnumerable<ProductReviewAllDto>>> GetAllProductReviewsAsync(PagingForProductReview? paging = null)
        {
            if (paging == null)
            {
                var productTotal = await _context.ProductReviews.CountAsync();
                var productReviews = await _context.ProductReviews.Include(u => u.User).Include(u => u.Product).ThenInclude(i => i.Category).ToListAsync();
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
                        User = new UserDto
                        {
                            Email = x.User.Email,
                            Username = x.User.Username,
                            UserId = x.UserId
                        },
                        Product = new ProductAllDto
                        {
                            Name = x.Product.Name,
                            Price = x.Product.Price,
                            PriceSale = x.Product.PriceSale,
                            Image = _uploadFilesService.GetFilePath(x.Product.Image, Contains.ProductImageFolder),
                            ProductId = x.Product.ProductId,
                            CategoryName = x.Product.Category.Name,
                            CategoryId = x.Product.CategoryId,
                            Sale = x.Product.Sale,
                            InventoryCount = x.Product.InventoryCount,
                            Popular = x.Product.Popular,
                            Slug = x.Product.Slug,


                        }
                    }),
                    Message = "Product Reviews found",
                    Status = true,
                    Total = productTotal
                };
            }
            var productReviewsPaging = _context.ProductReviews.Include(u => u.User).Include(u => u.Product).ThenInclude(i => i.Category).AsQueryable();
            if (!string.IsNullOrEmpty(paging.UserName))
            {
                productReviewsPaging = productReviewsPaging.Where(u => u.User.Username.Contains(paging.UserName) || u.User.Email.Contains(paging.UserName));
            }
            if (paging.MinRating > 0 && paging.MaxRating > 0)
            {
                productReviewsPaging = productReviewsPaging.Where(u => u.Rating >= paging.MinRating && u.Rating <= paging.MaxRating);
            }
            else if (paging.Rating > 0)
            {
                productReviewsPaging = productReviewsPaging.Where(u => u.Rating == paging.Rating);
            }
            else if (paging.MinRating > 0)
            {
                productReviewsPaging = productReviewsPaging.Where(u => u.Rating >= paging.MinRating);
            }
            else if (paging.MaxRating > 0)
            {
                productReviewsPaging = productReviewsPaging.Where(u => u.Rating <= paging.MaxRating);
            }
          
            if (paging.SortByDate)
            {
                productReviewsPaging = productReviewsPaging.OrderBy(u => u.CreatedAt);
            }
            else
            {
                productReviewsPaging = productReviewsPaging.OrderByDescending(u => u.CreatedAt);
            }
            productReviewsPaging = productReviewsPaging.Skip((paging.Page - 1) * paging.PageSize).Take(paging.PageSize);
            return new ApiResponse<IEnumerable<ProductReviewAllDto>>
            {
                Data = productReviewsPaging.Select(x => new ProductReviewAllDto
                {
                    ProductId = x.ProductId,
                    Rating = x.Rating,
                    Comment = x.Comment,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    ReviewId = x.ReviewId,
                    UserId = x.UserId,
                    User = new UserDto
                    {
                        Email = x.User.Email,
                        Username = x.User.Username,
                        UserId = x.UserId
                    },
                    Product = new ProductAllDto
                    {
                        Name = x.Product.Name,
                        Price = x.Product.Price,
                        PriceSale = x.Product.PriceSale,
                        Image = _uploadFilesService.GetFilePath(x.Product.Image, Contains.ProductImageFolder),
                        ProductId = x.Product.ProductId,
                        CategoryName = x.Product.Category.Name,
                        CategoryId = x.Product.CategoryId,
                        Sale = x.Product.Sale,
                        InventoryCount = x.Product.InventoryCount,
                        Popular = x.Product.Popular,
                        Slug = x.Product.Slug,
                    }
                }),
                Message = "Product Reviews found",
                Status = true,
                Total =  (int)Math.Ceiling(productReviewsPaging.Count() / (double)paging.PageSize)
            };
        }

        public async Task<ApiResponse<ProductReviewAllDto>> GetProductReviewByIdAsync(int id)
        {
            var productReview = await _context.ProductReviews.Include(u => u.User).Include(u => u.Product).ThenInclude(i => i.Category).FirstOrDefaultAsync(x => x.ReviewId == id);
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
                    User = new UserDto
                    {
                        Email = productReview.User.Email,
                        Username = productReview.User.Username,
                        UserId = productReview.UserId
                    },
                    Product = new ProductAllDto
                    {
                        Name = productReview.Product.Name,
                        Price = productReview.Product.Price,
                        PriceSale = productReview.Product.PriceSale,
                        Image = _uploadFilesService.GetFilePath(productReview.Product.Image, Contains.ProductImageFolder),
                        ProductId = productReview.Product.ProductId,
                        CategoryName = productReview.Product.Category.Name,
                        CategoryId = productReview.Product.CategoryId,
                        Sale = productReview.Product.Sale,
                        InventoryCount = productReview.Product.InventoryCount,
                        Popular = productReview.Product.Popular,
                        Slug = productReview.Product.Slug,
                    }

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
            var productReviews = await _context.ProductReviews.Include(u => u.User).Include(u => u.Product).ThenInclude(i => i.Category).Where(x => x.ProductId == productId).ToListAsync();
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
                    Product = new ProductAllDto
                    {
                        Name = x.Product.Name,
                        Price = x.Product.Price,
                        PriceSale = x.Product.PriceSale,
                        Image = _uploadFilesService.GetFilePath(x.Product.Image, Contains.ProductImageFolder),
                        ProductId = x.Product.ProductId,
                        CategoryName = x.Product.Category.Name,
                        CategoryId = x.Product.CategoryId,
                        Sale = x.Product.Sale,
                        InventoryCount = x.Product.InventoryCount,
                        Popular = x.Product.Popular,
                        Slug = x.Product.Slug,
                    },
                    User = new UserDto
                    {
                        Email = x.User.Email,
                        Username = x.User.Username,
                        UserId = x.User.UserId
                    }
                }),
                Message = "Product Reviews found",
                Status = true
            };

        }

        public async Task<ApiResponse<IEnumerable<ProductReviewAllDto>>> GetProductReviewsByUserAsync(int userId)
        {
            var productReviews = await _context.ProductReviews.Include(u => u.User).Include(u => u.Product).ThenInclude(i => i.Category).Where(x => x.UserId == userId).ToListAsync();
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
                    Product = new ProductAllDto
                    {
                        Name = x.Product.Name,
                        Price = x.Product.Price,
                        PriceSale = x.Product.PriceSale,
                        Image = _uploadFilesService.GetFilePath(x.Product.Image, Contains.ProductImageFolder),
                        ProductId = x.Product.ProductId,
                        CategoryName = x.Product.Category.Name,
                        CategoryId = x.Product.CategoryId,
                        Sale = x.Product.Sale,
                        InventoryCount = x.Product.InventoryCount,
                        Popular = x.Product.Popular,
                        Slug = x.Product.Slug,
                    },
                    User = new UserDto
                    {
                        Email = x.User.Email,
                        Username = x.User.Username,
                        UserId = x.User.UserId
                    }
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
                    UpdatedAt = DateTime.UtcNow,
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
