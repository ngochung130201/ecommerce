using ecommerce.DTO;
using ecommerce.Enums;
using ecommerce.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IUploadFilesService _uploadFilesService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductController(IProductService productService, IUploadFilesService uploadFilesService, IHttpContextAccessor httpContextAccessor)
        {
            _productService = productService;
            _uploadFilesService = uploadFilesService;
            _httpContextAccessor = httpContextAccessor;

        }
        [HttpGet]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            var response = await _productService.GetAllProductsAsync();
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductByIdAsync(int id)
        {
            var response = await _productService.GetProductByIdAsync(id);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetProductBySlugAsync(string slug)
        {
            var response = await _productService.GetProductBySlugAsync(slug);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetProductsByCategoryAsync(int categoryId)
        {
            var response = await _productService.GetProductsByCategoryAsync(categoryId);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost]
        public async Task<IActionResult> AddProductAsync([FromForm] ProductRequestDto product)
        {
            // convert type string to decimal, int
            var categoryId = int.TryParse(product.CategoryId, out int catId) ? catId : 0;
            var price = decimal.TryParse(product.Price, out decimal prc) ? prc : 0;
            var inventoryCount = int.TryParse(product.InventoryCount, out int invCount) ? invCount : 0;
            var popular = int.TryParse(product.Popular, out int pop) ? pop : 0;
            var gender = int.TryParse(product.Gender, out int gen) ? gen : 0;
            var ageRange = int.TryParse(product.AgeRange, out int age) ? age : 0;
            if (categoryId == 0 || price == 0 || inventoryCount == 0)
            {
                return BadRequest(new { Status = false, Message = "Invalid input" });
            }
            var productModel = new ProductDto
            {
                Name = product.Name,
                CategoryId = categoryId,
                Description = product.Description,
                Price = price,
                InventoryCount = inventoryCount,
                Image = product.Image,
                Gallery = product.Gallery,
                Popular = (Popular)popular,
                Sale = product.Sale,
                PriceSale = product.PriceSale,
                AgeRange = (AgeRange)ageRange,
                Gender = (Gender)gender

            };

            var response = await _productService.AddProductAsync(productModel, product.Image, product.Gallery);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, [FromForm] ProductUpdateRequestDto product)
        {
            var price = decimal.TryParse(product.Price, out decimal prc) ? prc : 0;
            var inventoryCount = int.TryParse(product.InventoryCount, out int invCount) ? invCount : 0;
            var popular = int.TryParse(product.Popular, out int pop) ? pop : 0;
            var gender = int.TryParse(product.Gender, out int gen) ? gen : 0;
            var ageRange = int.TryParse(product.AgeRange, out int age) ? age : 0;
            if (price == 0 || inventoryCount == 0)
            {
                return BadRequest(new { Status = false, Message = "Invalid input" });
            }
            var productModel = new ProductUpdateDto
            {
                Name = product.Name,
                Description = product.Description,
                Price = price,
                InventoryCount = inventoryCount,
                Image = product.Image,
                Gallery = product.Gallery,
                Popular = (Popular)popular,
                Sale = product.Sale,
                PriceSale = product.PriceSale,
                AgeRange = (AgeRange)ageRange,
                Gender = (Gender)gender
            };
            var response = await _productService.UpdateProductAsync(id, productModel, image: product.Image, gallery: product.Gallery);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var response = await _productService.DeleteProductAsync(id);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        // delete products
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteProductsAsync([FromBody] List<int> ids)
        {
            var response = await _productService.DeleteProductsAsync(ids);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        // filter products
        [HttpPost("filter")]
        public async Task<IActionResult> GetProductsByFilterAsync(ProductFilterDto popularDto)
        {
            var response = await _productService.GetProductsByFilterAsync(popularDto);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        // Get image by filename and folder
        [HttpGet("image/{folder}/{filename}")]
        public IActionResult GetImage(string folder, string filename)
        {
            string ImageUrl = string.Empty;
            string HostUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            string Filepath = _uploadFilesService.GetImageHostPath(filename, folder);
            string Imagepath = Filepath + "\\image.png";
            if (!System.IO.File.Exists(Imagepath))
            {
                ImageUrl = HostUrl + "/none.png";
            }
            else
            {
                ImageUrl = HostUrl + "/" + folder + "/" + filename;
            }
            // return File
            return Ok(new { ImageUrl });

        }
        // pagination with pageSize and pageNumber
        [HttpGet("pagination")]
        public async Task<IActionResult> GetProductsByPaginationAsync(int pageSize, int pageNumber)
        {
            var response = await _productService.GetProductsByPaginationAsync(pageSize, pageNumber);
            if (response.Status)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

    }
}
