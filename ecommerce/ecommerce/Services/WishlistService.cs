using ecommerce.DTO;
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
        public WishlistService(IWishListRepository wishlistRepository, IUnitOfWork unitOfWork)
        {
            _wishlistRepository = wishlistRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<int>> AddWishlistAsync(WishlistDto wishlist)
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

        public async Task<ApiResponse<IEnumerable<WishlistDto>>> GetAllWishlistsAsync()
        {
            var wishlists = await _wishlistRepository.GetAllWishListsAsync();
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
                    ProductId = x.ProductId
                }),
                Message = "Wishlist found",
                Status = true
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
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    WishlistId = wishlist.WishlistId
                },
                Message = "Wishlist found",
                Status = true
            };
        }

        public async Task<ApiResponse<IEnumerable<WishlistDto>>> GetWishlistByProductIdAsync(int productId)
        {
            var  wishlist = await _wishlistRepository.GetWishListByProductIdAsync(productId);
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
                }),
                Message = "Wishlist found",
                Status = true
            };
        }

        public async Task<ApiResponse<int>> UpdateWishlistAsync(int id, WishlistDto wishlist)
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
