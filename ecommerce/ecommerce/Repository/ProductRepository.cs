using ecommerce.Context;
using ecommerce.DTO;
using ecommerce.Models;
using ecommerce.Repository.Base;
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

        public Task AddProductAsync(Product product)
        {
            var newProduct = new Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId
            };
            _repositoryBase.Create(newProduct);
            return _repositoryBase.SaveAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _repositoryBase.FindByIdAsync(id);
            _repositoryBase.Delete(product);
            await _repositoryBase.SaveAsync();
        }

        public Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return _repositoryBase.FindAllAsync();
        }

        public Task<Product> GetProductByIdAsync(int id)
        {
            return _repositoryBase.FindByIdAsync(id);
        }

        public Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return _repositoryBase.FindByConditionAsync(p => p.CategoryId == categoryId);
        }

        public Task<IEnumerable<Product>> SearchProductsAsync(ProductSearchDto searchDTO)
        {
            var products = _repositoryBase.FindByConditionAsync(p =>
                (string.IsNullOrEmpty(searchDTO.Name) || p.Name.Contains(searchDTO.Name)) &&
                (!searchDTO.MinPrice.HasValue || p.Price >= searchDTO.MinPrice) &&
                (!searchDTO.MaxPrice.HasValue || p.Price <= searchDTO.MaxPrice) &&
                (!searchDTO.CategoryId.HasValue || p.CategoryId == searchDTO.CategoryId)
            );
            return products;
        }

        public async Task UpdateProductAsync(int id, Product product, Product productExist)
        {
            productExist.Name = product.Name;
            productExist.Description = product.Description;
            productExist.Price = product.Price;
            productExist.CategoryId = product.CategoryId;
            _repositoryBase.Update(productExist);
            await _repositoryBase.SaveAsync();
        }
    }
}
