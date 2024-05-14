using ecommerce.Context;
using ecommerce.DTO;
using ecommerce.Middleware;
using ecommerce.Models;
using ecommerce.Repository.Interface;
using ecommerce.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly IRepositoryBase<Product> _repositoryBase;
        private readonly EcommerceContext _context;
        public ProductRepository(IRepositoryBase<Product> repositoryBase, EcommerceContext context)
        {
            _repositoryBase = repositoryBase;
            _context = context;
        }

        public void AddProduct(Product product)
        {
            var newProduct = new Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                Slug = product.Slug,
                CreatedAt = product.CreatedAt,
                Image = product.Image,
                Gallery = product.Gallery,
                Popular = product.Popular,
                InventoryCount = product.InventoryCount,
                PopularText = product.PopularText,
                Sale = product.Sale,
                PriceSale = product.PriceSale,
                Gender = product.Gender,
                AgeRange = product.AgeRange,
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
            var products = await _context.Products.Include(u => u.Category).ToListAsync();
            if (products == null)
            {
                throw new CustomException("No Product found", 404);
            }
            return products;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = await _context.Products.Include(u => u.Category).FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                throw new CustomException("Product not found", 404);
            }
            return product;
        }

        public async Task<Product> GetProductBySlugAsync(string slug)
        {
            var product = await _context.Products.Include(u => u.Category).FirstOrDefaultAsync(p => p.Slug == slug);
            if (product == null)
            {
                throw new CustomException("Product not found", 404);
            }
            return product;

        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            var products = await _context.Products.Include(u => u.Category).Where(p => p.CategoryId == categoryId).ToListAsync();
            if (products == null)
            {
                throw new CustomException("No Product found", 404);
            }
            return products;
        }

        public async Task DeleteProducts(List<int> ids)
        {
            var products = await _context.Products.Where(x => ids.Contains(x.ProductId)).ToListAsync();
            _context.Products.RemoveRange(products);
        }


        public async Task<IEnumerable<Product>> SearchProductsAsync(ProductSearchDto searchDTO)
        {
            var products = await _context.Products.Include(u => u.Category).Where(p =>
                (string.IsNullOrEmpty(searchDTO.Name) || p.Name.Contains(searchDTO.Name)) &&
                (searchDTO.MinPrice == 0 || p.Price >= searchDTO.MinPrice) &&
                (searchDTO.MaxPrice == 0 || p.Price <= searchDTO.MaxPrice) &&
                (searchDTO.CategoryId == 0 || p.CategoryId == searchDTO.CategoryId)).ToListAsync();

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
            productExist.CategoryId = product.CategoryId;
            productExist.Price = product.Price;
            productExist.UpdatedAt = DateTime.UtcNow;
            productExist.Slug = product.Slug;
            if (product.Image != null)
            {
                productExist.Image = product.Image;
            }
            if (!string.IsNullOrEmpty(product.Gallery))
            {
                productExist.Gallery = product.Gallery;
            }
            productExist.Popular = product.Popular;
            productExist.InventoryCount = product.InventoryCount;
            productExist.PopularText = product.PopularText;
            productExist.Sale = product.Sale;
            productExist.PriceSale = product.PriceSale;
            productExist.Gender = product.Gender;
            productExist.AgeRange = product.AgeRange;
            _repositoryBase.Update(productExist);

        }

        public async Task<List<Product>> GetProductsByFilterAsync(ProductFilterDto filterDto)
        {
            int itemsToSkip = (filterDto.Page - 1) * filterDto.PageSize;

            var productsQuery = _context.Products
                .Include(p => p.Category)
                .Where(p =>
                    (filterDto.CategoryId == 0 || p.CategoryId == filterDto.CategoryId) &&
                    (filterDto.Popular == 0 || p.Popular == filterDto.Popular) &&
                    (filterDto.Gender == 0 || p.Gender == filterDto.Gender) &&
                    (filterDto.AgeRange == 0 || p.AgeRange == filterDto.AgeRange) &&
                    (filterDto.InventoryCount == 0 || p.InventoryCount >= filterDto.InventoryCount));

            if (!string.IsNullOrEmpty(filterDto.Name))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(filterDto.Name) ||
                p.Description.Contains(filterDto.Name) || p.Category.Name.Contains(filterDto.Name));
            }
         
            if(!string.IsNullOrEmpty(filterDto.MinAndMaxPrice)){
                var prices = filterDto.MinAndMaxPrice.Split("-");
                var minPrice = Convert.ToDecimal(prices[0]);
                if(prices.Length > 1 && !string.IsNullOrEmpty(prices[1])){
                    var maxPrice = Convert.ToDecimal(prices[1]);
                    productsQuery = productsQuery.Where(p => p.PriceSale >= minPrice && p.PriceSale <= maxPrice);
                }
                else {
                    productsQuery = productsQuery.Where(p => p.PriceSale >= minPrice);
                }
            }

            productsQuery = filterDto.SortByDate ? productsQuery.OrderBy(p => p.CreatedAt) : productsQuery.OrderByDescending(p => p.CreatedAt);
            if(filterDto.SortByPrice == Enums.SortByPrice.Ascending){
                productsQuery = productsQuery.OrderBy(p => p.PriceSale);
            }
            if(filterDto.SortByPrice == Enums.SortByPrice.Descending){
                productsQuery = productsQuery.OrderByDescending(p => p.PriceSale);
            }
            var products = await productsQuery
                .Skip(itemsToSkip)
                .Take(filterDto.PageSize)
                .ToListAsync();

            return products;

        }

    }
}
