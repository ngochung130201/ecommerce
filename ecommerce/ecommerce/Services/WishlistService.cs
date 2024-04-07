using ecommerce.DTO;
using ecommerce.Helpers;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;
using ecommerce.UnitOfWork;

namespace ecommerce.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishListRepository _wishlistRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUploadFilesService _uploadFilesService;
        public WishlistService(IWishListRepository wishlistRepository, IUnitOfWork unitOfWork, IUploadFilesService uploadFilesService)
        {
            _wishlistRepository = wishlistRepository;
            _unitOfWork = unitOfWork;
            _uploadFilesService = uploadFilesService;
        }

        public async Task<ApiResponse<int>> AddWishlistAsync(WishlistRequestDto wishlist)
        {
            var newWishlist = new Wishlist
            {
                UserId = wishlist.UserId,
                ProductId = wishlist.ProductId,
            };
            try
            {
                await _wishlistRepository.AddWishListAsync(newWishlist);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<int>
                {
                    Data = 0,
                    Message = "Wishlist added",
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

        public async Task<ApiResponse<int>> DeleteWishlistAsync(int id)
        {
            var wishlist = await _wishlistRepository.GetWishListByIdAsync(id);
            if (wishlist == null)
            {
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "No Wishlist found",
                    Status = false
                };
            }
            try
            {
                await _wishlistRepository.DeleteWishListAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "Wishlist deleted",
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

        public async Task<ApiResponse<IEnumerable<WishlistDto>>> GetAllWishlistsAsync(PagingForWishlist? paging = null)
        {
            var wishlists = await _wishlistRepository.GetAllWishListsAsync(paging);
            if (wishlists == null)
            {
                return new ApiResponse<IEnumerable<WishlistDto>>
                {
                    Data = null,
                    Message = "No Wishlist found",
                    Status = false
                };
            }
            return new ApiResponse<IEnumerable<WishlistDto>>
            {
                Data = wishlists.Select(x => new WishlistDto
                {
                    UserId = x.UserId,
                    ProductId = x.ProductId,
                    WishlistId = x.WishlistId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    Product = new ProductAllDto
                    {
                        Slug = x.Product.Slug,
                        ProductId = x.Product.ProductId,
                        Name = x.Product.Name,
                        CategoryId = x.Product.CategoryId,
                        Image = _uploadFilesService.GetFilePath(x.Product.Image, Contains.ProductImageFolder),
                        Price = x.Product.Price,
                        PriceSale = x.Product.PriceSale,
                        Sale = x.Product.Sale,
                        CategoryName = x.Product.Category.Name,
                        Popular = x.Product.Popular,
                        InventoryCount = x.Product.InventoryCount,
                    },
                    User = new UserDto
                    {
                        UserId = x.User.UserId,
                        Username = x.User.Username,
                        Email = x.User.Email,
                        CreatedAt = x.User.CreatedAt,
                        UpdatedAt = x.User.UpdatedAt,
                    }
                }),
                Message = "Wishlist found",
                Status = true,
                Total = wishlists.Count()
            };

        }

        public async Task<ApiResponse<WishlistDto>> GetWishlistByIdAsync(int id)
        {
            var wishlist = await _wishlistRepository.GetWishListByIdAsync(id);
            if (wishlist == null)
            {
                return new ApiResponse<WishlistDto>
                {
                    Data = null,
                    Message = "Wishlist not found",
                    Status = false
                };
            }
            return new ApiResponse<WishlistDto>
            {
                Data = new WishlistDto
                {
                    UserId = wishlist.UserId,
                    ProductId = wishlist.ProductId,
                    CreatedAt = wishlist.CreatedAt,
                    UpdatedAt = wishlist.UpdatedAt,
                    WishlistId = wishlist.WishlistId,
                    Product = new ProductAllDto
                    {
                        Slug = wishlist.Product.Slug,
                        ProductId = wishlist.Product.ProductId,
                        Name = wishlist.Product.Name,
                        CategoryId = wishlist.Product.CategoryId,
                        Image = _uploadFilesService.GetFilePath(wishlist.Product.Image, Contains.ProductImageFolder),
                        Price = wishlist.Product.Price,
                        PriceSale = wishlist.Product.PriceSale,
                        Sale = wishlist.Product.Sale,
                        CategoryName = wishlist.Product.Category.Name,
                        Popular = wishlist.Product.Popular,
                        InventoryCount = wishlist.Product.InventoryCount,
                    },
                    User = new UserDto
                    {
                        UserId = wishlist.User.UserId,
                        Username = wishlist.User.Username,
                        Email = wishlist.User.Email,
                        CreatedAt = wishlist.User.CreatedAt,
                        UpdatedAt = wishlist.User.UpdatedAt,
                    }
                },
                Message = "Wishlist found",
                Status = true
            };
        }

        public async Task<ApiResponse<IEnumerable<WishlistDto>>> GetWishlistByProductIdAsync(int productId)
        {
            var wishlist = await _wishlistRepository.GetWishListByProductIdAsync(productId);
            if (wishlist == null)
            {
                return new ApiResponse<IEnumerable<WishlistDto>>
                {
                    Data = null,
                    Message = "Wishlist not found",
                    Status = false
                };
            }
            return new ApiResponse<IEnumerable<WishlistDto>>
            {
                Data = wishlist.Select(x => new WishlistDto
                {
                    UserId = x.UserId,
                    ProductId = x.ProductId,
                    UpdatedAt = x.UpdatedAt,
                    CreatedAt = x.CreatedAt,
                    Product = new ProductAllDto
                    {
                        Slug = x.Product.Slug,
                        ProductId = x.Product.ProductId,
                        Name = x.Product.Name,
                        CategoryId = x.Product.CategoryId,
                        Image = _uploadFilesService.GetFilePath(x.Product.Image, Contains.ProductImageFolder),
                        Price = x.Product.Price,
                        PriceSale = x.Product.PriceSale,
                        Sale = x.Product.Sale,
                        CategoryName = x.Product.Category.Name,
                        Popular = x.Product.Popular,
                        InventoryCount = x.Product.InventoryCount,

                    },
                    User = new UserDto
                    {
                        UserId = x.User.UserId,
                        Username = x.User.Username,
                        Email = x.User.Email,
                        CreatedAt = x.User.CreatedAt,
                        UpdatedAt = x.User.UpdatedAt,
                    }

                }),
                Message = "Wishlist found",
                Status = true
            };
        }
        public async Task<ApiResponse<IEnumerable<WishlistDto>>> GetWishListByUserIdAsync(int userId)
        {
            var wishlist = await _wishlistRepository.GetWishListsByUserIdAsync(userId);
            if (wishlist == null)
            {
                return new ApiResponse<IEnumerable<WishlistDto>>
                {
                    Data = null,
                    Message = "Wishlist not found",
                    Status = false
                };
            }
            return new ApiResponse<IEnumerable<WishlistDto>>
            {
                Data = wishlist.Select(x => new WishlistDto
                {
                    UserId = x.UserId,
                    ProductId = x.ProductId,
                    WishlistId = x.WishlistId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    Product = new ProductAllDto
                    {
                        Slug = x.Product.Slug,
                        ProductId = x.Product.ProductId,
                        Name = x.Product.Name,
                        CategoryId = x.Product.CategoryId,
                        Image = _uploadFilesService.GetFilePath(x.Product.Image, Contains.ProductImageFolder),
                        Price = x.Product.Price,
                        PriceSale = x.Product.PriceSale,
                        Sale = x.Product.Sale,
                        CategoryName = x.Product.Category.Name,
                        Popular = x.Product.Popular,
                        InventoryCount = x.Product.InventoryCount,

                    },
                    User = new UserDto
                    {
                        UserId = x.User.UserId,
                        Username = x.User.Username,
                        Email = x.User.Email,
                        CreatedAt = x.User.CreatedAt,
                        UpdatedAt = x.User.UpdatedAt,
                    }

                }),
                Message = "Wishlist found",
                Status = true
            };
        }

        public async Task<ApiResponse<int>> UpdateWishlistAsync(int id, WishlistRequestDto wishlist)
        {
            var newWishlist = new Wishlist
            {
                UserId = wishlist.UserId,
                ProductId = wishlist.ProductId,
                UpdatedAt = DateTime.UtcNow,
            };
            try
            {
                await _wishlistRepository.UpdateWishListAsync(id, newWishlist);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<int>
                {
                    Data = id,
                    Message = "Wishlist updated",
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
