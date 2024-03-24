using ecommerce.DTO;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;

namespace ecommerce.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
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
                    CategoryId = product.CategoryId
                };
                await _productRepository.AddProductAsync(newProduct);
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
                await _productRepository.DeleteProductAsync(id);
                return new ApiResponse<int> { Message = "Product deleted successfully", Status = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Message = ex.Message, Status = false };
            }
        }

        public async Task<ApiResponse<IEnumerable<ProductDto>>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            if (products == null)
            {
                return new ApiResponse<IEnumerable<ProductDto>> { Message = "Products not found", Status = false };
            }
            var productDtos = products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId
            });
            return new ApiResponse<IEnumerable<ProductDto>> { Data = productDtos, Status = true };
        }

        public async Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id)
        {
            // validation
            if (id <= 0)
            {
                return new ApiResponse<ProductDto> { Message = "Product id is required", Status = false };
            }
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                return new ApiResponse<ProductDto> { Message = "Product not found", Status = false };
            }
            var productDto = new ProductDto
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId
            };
            return new ApiResponse<ProductDto> { Data = productDto, Status = true };
        }

        public async Task<ApiResponse<IEnumerable<ProductDto>>> GetProductsByCategoryAsync(int categoryId)
        {
            // validation
            if (categoryId <= 0)
            {
                return new ApiResponse<IEnumerable<ProductDto>> { Message = "Category id is required", Status = false };
            }
            var products = await _productRepository.GetProductsByCategoryAsync(categoryId);
            if (products == null)
            {
                return new ApiResponse<IEnumerable<ProductDto>> { Message = "Products not found", Status = false };
            }
            var productDtos = products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId
            });
            return new ApiResponse<IEnumerable<ProductDto>> { Data = productDtos, Status = true };
        }

        public async Task<ApiResponse<IEnumerable<ProductDto>>> SearchProductsAsync(ProductSearchDto searchDTO)
        {
            // validation
            if (searchDTO == null)
            {
                return new ApiResponse<IEnumerable<ProductDto>> { Message = "Search criteria is required", Status = false };
            }
            var products = await _productRepository.SearchProductsAsync(searchDTO);
            if (products == null)
            {
                return new ApiResponse<IEnumerable<ProductDto>> { Message = "Products not found", Status = false };
            }
            var productDtos = products.Select(p => new ProductDto
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId
            });
            return new ApiResponse<IEnumerable<ProductDto>> { Data = productDtos, Status = true };
        }

        public async Task<ApiResponse<int>> UpdateProductAsync(int id, ProductDto product, ProductDto productExist)
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
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId
                };
                await _productRepository.UpdateProductAsync(id, productUpdate, productItem);
                return new ApiResponse<int> { Message = "Product updated successfully", Status = true };
            }
            catch (Exception ex)
            {
                return new ApiResponse<int> { Message = ex.Message, Status = false };
            }
        }
    }
}
