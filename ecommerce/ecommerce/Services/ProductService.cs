﻿using ecommerce.DTO;
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
        private readonly IUploadFilesService _uploadFilesService;
        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork, IUploadFilesService uploadFilesService)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _uploadFilesService = uploadFilesService;
        }
        public async Task<ApiResponse<int>> AddProductAsync(ProductDto product, IFormFile image, List<IFormFile> gallery)
        {
            var imageString = string.Empty;
            var galleryStrings = new List<string>();
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
                if(image == null){
                    return new ApiResponse<int> { Message = "Product image is required", Status = false };
                }
                var imageResponse = await _uploadFilesService.UploadFileAsync(image, "products");
                if (!imageResponse.Status)
                {
                    return new ApiResponse<int> { Message = imageResponse.Message, Status = false };
                }
                List<string> galleryString = new List<string>();
                if (gallery != null && gallery.Count > 0)
                {
                    var galleryRes  = await _uploadFilesService.UploadFilesAsync(gallery, "products");
                    if (!galleryRes.Status)
                    {
                        return new ApiResponse<int> { Message = galleryRes.Message, Status = false };
                    }
                    galleryString = galleryRes.Data ?? new List<string>();
                 
                }
                var newProduct = new Product
                {
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                    InventoryCount = product.InventoryCount,
                    Slug = product.Name.GenerateSlug(),
                    Image = imageResponse.Data,
                    CreatedAt = DateTime.UtcNow,
                };
                if (galleryString.Count > 0)
                {
                    // galleryString add "," to separate each image
                    newProduct.Gallery = string.Join(",", galleryString);
                }
                galleryStrings = galleryString;
                imageString = imageResponse.Data;
                _productRepository.AddProduct(newProduct);
                await _unitOfWork.SaveChangesAsync();
                return new ApiResponse<int> { Message = "Product added successfully", Data = newProduct.ProductId, Status = true };
            }
            catch (Exception ex)
            {
                // remove image and gallery if error
                if (image != null && !string.IsNullOrEmpty(imageString))
                {
                    await _uploadFilesService.RemoveFileAsync(imageString, "products");
                }
                if (gallery != null && gallery.Count > 0)
                {
                    await _uploadFilesService.RemoveFilesAsync(galleryStrings, "products");
                }
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
                // remove image and gallery if error
                if (product != null && !string.IsNullOrEmpty(product.Image))
                {
                    await _uploadFilesService.RemoveFileAsync(product.Image, "products");
                }
                if (product.Gallery != null && string.IsNullOrEmpty(product.Gallery))
                {
                    var galleryStrings = product.Gallery.Split(",").ToList();
                    await _uploadFilesService.RemoveFilesAsync(galleryStrings, "products");
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
                Slug = p.Slug,
                Image = p.Image,
                Gallery = p.Gallery,
            });
            var newProductDtos = new List<ProductAllDto>();
            // get file path
            foreach (var product in productDtos)
            {
                if (!string.IsNullOrEmpty(product.Image))
                {
                    product.Image = _uploadFilesService.GetFilePath(product.Image, "products");
                }
                if (!string.IsNullOrEmpty(product.Gallery))
                {
                    var gallery = product.Gallery.Split(",").ToList();
                    var galleryUrls = gallery.Select(g => _uploadFilesService.GetFilePath(g, "products"));
                    product.Gallery = string.Join(",", galleryUrls);
                }
                newProductDtos.Add(product);
            }
            return new ApiResponse<IEnumerable<ProductAllDto>> { Data = newProductDtos, Status = true };
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
            // get image in forder wwwroot and return url to client 
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
                UpdatedAt = product.UpdatedAt,
            };
            // get file path
            if (!string.IsNullOrEmpty(product.Image))
            {
                productDto.Image = _uploadFilesService.GetFilePath(product.Image, "products");
            }
            if (!string.IsNullOrEmpty(product.Gallery))
            {
                var gallery = product.Gallery.Split(",").ToList();
                var galleryUrls = gallery.Select(g => _uploadFilesService.GetFilePath(g, "products"));
                productDto.Gallery = string.Join(",", galleryUrls);
            }
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
                UpdatedAt = product.UpdatedAt,
            };
            // get file path
            if (!string.IsNullOrEmpty(product.Image))
            {
                productDto.Image = _uploadFilesService.GetFilePath(product.Image, "products");
            }
            if (!string.IsNullOrEmpty(product.Gallery))
            {
                var gallery = product.Gallery.Split(",").ToList();
                var galleryUrls = gallery.Select(g => _uploadFilesService.GetFilePath(g, "products"));
                productDto.Gallery = string.Join(",", galleryUrls);
            }
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
                CategoryId = p.CategoryId,
                InventoryCount = p.InventoryCount,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Slug = p.Slug,
                Image = p.Image,
                Gallery = p.Gallery,
            });
            // get file path
            foreach (var product in productDtos)
            {
                if (!string.IsNullOrEmpty(product.Image))
                {
                    product.Image = _uploadFilesService.GetFilePath(product.Image, "products");
                }
                if (!string.IsNullOrEmpty(product.Gallery))
                {
                    var gallery = product.Gallery.Split(",").ToList();
                    var galleryUrls = gallery.Select(g => _uploadFilesService.GetFilePath(g, "products"));
                    product.Gallery = string.Join(",", galleryUrls);
                }
            }
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
                InventoryCount = p.InventoryCount,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Slug = p.Slug,
                Image = p.Image,
                Gallery = p.Gallery,
            });
            var newProductDtos = new List<ProductAllDto>();
            // get file path
            foreach (var product in productDtos)
            {
                if (!string.IsNullOrEmpty(product.Image))
                {
                    product.Image = _uploadFilesService.GetFilePath(product.Image, "products");
                }
                if (!string.IsNullOrEmpty(product.Gallery))
                {
                    var gallery = product.Gallery.Split(",").ToList();
                    var galleryUrls = gallery.Select(g => _uploadFilesService.GetFilePath(g, "products"));
                    product.Gallery = string.Join(",", galleryUrls);
                }
                newProductDtos.Add(product);
            }
            return new ApiResponse<IEnumerable<ProductAllDto>> { Data = newProductDtos, Status = true };
        }

        public async Task<ApiResponse<int>> UpdateProductAsync(int id, ProductUpdateDto product, IFormFile? image = null, List<IFormFile>? gallery = null)
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
                if (image != null)
                {
                    // remove old image
                    if (!string.IsNullOrEmpty(productItem.Image))
                    {
                        await _uploadFilesService.RemoveFileAsync(productItem.Image, "products");
                    }
                    var imageResponse = await _uploadFilesService.UploadFileAsync(image, "products");
                    if (!imageResponse.Status)
                    {
                        return new ApiResponse<int> { Message = imageResponse.Message, Status = false };
                    }
                    productUpdate.Image = imageResponse.Data;
                }
                if (gallery != null && gallery.Count > 0)
                {
                    // remove old gallery
                    if (!string.IsNullOrEmpty(productItem.Gallery))
                    {
                        var galleryStrings = productItem.Gallery.Split(",").ToList();
                        await _uploadFilesService.RemoveFilesAsync(galleryStrings, "products");
                    }
                    var galleryRes = await _uploadFilesService.UploadFilesAsync(gallery, "products");
                    if (!galleryRes.Status)
                    {
                        return new ApiResponse<int> { Message = galleryRes.Message, Status = false };
                    }
                    productUpdate.Gallery = string.Join(",", galleryRes.Data ?? new List<string>());
                }
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
