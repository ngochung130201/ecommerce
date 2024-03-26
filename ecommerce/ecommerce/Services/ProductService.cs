using ecommerce.DTO;
using ecommerce.Helpers;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;
using ecommerce.UnitOfWork;

namespace ecommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<int>> AddProductAsync(ProductDto product)
        {
            try
            {
                // validation
                if (string.IsNullOrEmpty(product.Name))
                {
                    return new ApiResponse<int> { Message = "Product name is required", Status = false };
                }
                if (product.Price <= 0)
                {
                    return new ApiResponse<int> { Message = "Product price must be greater than 0", Status = false };
                }
                if (product.CategoryId <= 0)
                {
                    return new ApiResponse<int> { Message = "Category is required", Status = false };
                }

                var newProduct = new Product
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                    InventoryCount = product.InventoryCount,
                    Slug = product.Name.GenerateSlug()
                };
                _productRepository.AddProduct(newProduct);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<int> { Message = "Product added successfully", Data = newProduct.ProductId, Status = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Message = ex.Message, Status = false };
            }

        }

        public async Task<ApiResponse<int>> DeleteProductAsync(int id)
        {
            try
            {
                // validation
                if (id <= 0)
                {
                    return new ApiResponse<int> { Message = "Product id is required", Status = false };
                }
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product == null)
                {
                    return new ApiResponse<int> { Message = "Product not found", Status = false };
                }
                _productRepository.DeleteProduct(product);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<int> { Message = "Product deleted successfully", Status = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Message = ex.Message, Status = false, Data = id };
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductAllDto>>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            if (products == null)
            {
                return new ApiResponse<IEnumerable<ProductAllDto>> { Message = "Products not found", Status = false };
            }
            var productDtos = products.Select(p => new ProductAllDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                InventoryCount = p.InventoryCount,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Slug = p.Slug
            });
            return new ApiResponse<IEnumerable<ProductAllDto>> { Data = productDtos, Status = true };
        }

        public async Task<ApiResponse<ProductAllDto>> GetProductByIdAsync(int id)
        {
            // validation
            if (id <= 0)
            {
                return new ApiResponse<ProductAllDto> { Message = "Product id is required", Status = false };
            }
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return new ApiResponse<ProductAllDto> { Message = "Product not found", Status = false };
            }
            var productDto = new ProductAllDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Slug = product.Slug,
                CategoryId = product.CategoryId,
                InventoryCount = product.InventoryCount,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
            return new ApiResponse<ProductAllDto> { Data = productDto, Status = true };
        }

        public async Task<ApiResponse<ProductAllDto>> GetProductBySlugAsync(string slug)
        {
            var product = await _productRepository.GetProductBySlugAsync(slug);
            if (product == null)
            {
                return new ApiResponse<ProductAllDto> { Message = "Product not found", Status = false };
            }
            var productDto = new ProductAllDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Slug = product.Slug,
                CategoryId = product.CategoryId,
                InventoryCount = product.InventoryCount,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
            return new ApiResponse<ProductAllDto> { Data = productDto, Status = true };
        }

        public async Task<ApiResponse<IEnumerable<ProductAllDto>>> GetProductsByCategoryAsync(int categoryId)
        {
            // validation
            if (categoryId <= 0)
            {
                return new ApiResponse<IEnumerable<ProductAllDto>> { Message = "Category id is required", Status = false };
            }
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            if (products == null)
            {
                return new ApiResponse<IEnumerable<ProductAllDto>> { Message = "Products not found", Status = false };
            }
            var productDtos = products.Select(p => new ProductAllDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId
            });
            return new ApiResponse<IEnumerable<ProductAllDto>> { Data = productDtos, Status = true };
        }

        public async Task<ApiResponse<IEnumerable<ProductAllDto>>> SearchProductsAsync(ProductSearchDto searchDTO)
        {
            // validation
            if (searchDTO == null)
            {
                return new ApiResponse<IEnumerable<ProductAllDto>> { Message = "Search criteria is required", Status = false };
            }
            var products = await _productRepository.SearchProductsAsync(searchDTO);
            if (products == null)
            {
                return new ApiResponse<IEnumerable<ProductAllDto>> { Message = "Products not found", Status = false };
            }
            var productDtos = products.Select(p => new ProductAllDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                InventoryCount = p.InventoryCount
            });
            return new ApiResponse<IEnumerable<ProductAllDto>> { Data = productDtos, Status = true };
        }

        public async Task<ApiResponse<int>> UpdateProductAsync(int id, ProductUpdateDto product)
        {
            // validation
            if (id <= 0)
            {
                return new ApiResponse<int> { Message = "Product id is required", Status = false };
            }
            var productItem = await _productRepository.GetProductByIdAsync(id);
            if (productItem == null)
            {
                return new ApiResponse<int> { Message = "Product not found", Status = false };
            }
            try
            {
                var productUpdate = new Product
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    InventoryCount = product.InventoryCount,
                    Slug = product.Name.GenerateSlug(),
                    UpdatedAt = DateTime.UtcNow,
                };
                _productRepository.UpdateProduct(productUpdate, productItem);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<int> { Message = "Product updated successfully", Status = true, Data = id };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Message = ex.Message, Status = false };
            }
        }
    }
}
