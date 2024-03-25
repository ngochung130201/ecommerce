using ecommerce.DTO;
using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;

namespace ecommerce.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IRepositoryBase<Product> _repositoryBase;
        public ProductRepository(IRepositoryBase<Product> repositoryBase)
        {
            _repositoryBase = repositoryBase;
        }

        public void AddProduct(Product product)
        {
            var newProduct = new Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId
            };
            _repositoryBase.Create(newProduct);
        }

        public void DeleteProduct(Product? product)
        {
            if (product == null)
            {
                throw new CustomException("No Product found", 404);
            }
            _repositoryBase.Delete(product);

        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var products = await _repositoryBase.FindAllAsync();
            if (products == null)
            {
                throw new CustomException("No Product found", 404);
            }
            return products;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = await _repositoryBase.FindByIdAsync(id);
            if (product == null)
            {
                throw new CustomException("Product not found", 404);
            }
            return product;
        }

        public Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = _repositoryBase.FindByConditionAsync(p => p.CategoryId == categoryId);
            if (products == null)
            {
                throw new CustomException("No Product found", 404);
            }
            return products;
        }

        public Task<IEnumerable<Product>> SearchProductsAsync(ProductSearchDto searchDTO)
        {
            var products = _repositoryBase.FindByConditionAsync(p =>
                (string.IsNullOrEmpty(searchDTO.Name) || p.Name.Contains(searchDTO.Name)) &&
                (!searchDTO.MinPrice.HasValue || p.Price >= searchDTO.MinPrice) &&
                (!searchDTO.MaxPrice.HasValue || p.Price <= searchDTO.MaxPrice) &&
                (!searchDTO.CategoryId.HasValue || p.CategoryId == searchDTO.CategoryId)
            );
            if (products == null)
            {
                throw new CustomException("No Product found", 404);
            }
            return products;
        }

        public void UpdateProduct(Product product, Product productExist)
        {
            productExist.Name = product.Name;
            productExist.Description = product.Description;
            productExist.Price = product.Price;
            productExist.CategoryId = product.CategoryId;
            productExist.UpdatedAt = DateTime.UtcNow;
            _repositoryBase.Update(productExist);

        }
    }
}
