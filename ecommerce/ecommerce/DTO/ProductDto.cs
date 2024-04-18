using ecommerce.Enums;

namespace ecommerce.DTO
{
    public class ProductDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int InventoryCount { get; set; }
        public Popular? Popular { get; set; } = null;
        public Gender? Gender { get; set; } = null;
        public IFormFile? Image { get; set; } = null;
        public decimal PriceSale { get; set; }
        public AgeRange? AgeRange { get; set; } = null;
        public decimal? Sale { get; set; } = null;
        public List<IFormFile> Gallery { get; set; } = new List<IFormFile>();
    }
     public class ProductRequestDto
    {
        public string CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string InventoryCount { get; set; }
        public string? Popular { get; set; } = null;
        public string? Gender { get; set; } = null;
        public string? AgeRange { get; set; } = null;
        public decimal PriceSale { get; set; }
        public IFormFile? Image { get; set; } = null;
        public decimal? Sale { get; set; } = null;
        public List<IFormFile> Gallery { get; set; } = new List<IFormFile>();
    }
    public class ProductAllDto
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Image { get; set; }
        public string Gallery { get; set; }
        public Popular? Popular { get; set; } = null;
        public Gender? Gender { get; set; } = null;
        public AgeRange? AgeRange { get; set; } = null;
        public string? PopularText { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal PriceSale { get; set; }
        public int InventoryCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public string CategoryName { get; set; }
        public decimal? Sale { get; set; } = null;
    }
    public class ProductUpdateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Popular? Popular { get; set; } = null;
        public Gender? Gender { get; set; } = null;
        public AgeRange? AgeRange { get; set; } = null;
        public int InventoryCount { get; set; }
        public IFormFile? Image { get; set; } = null;
        public decimal PriceSale { get; set; }
        public List<IFormFile> Gallery { get; set; } = new List<IFormFile>();
        public decimal? Sale { get; set; } = null;
    }
    public class ProductUpdateRequestDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public int? Popular { get; set; } = null;
        public int? Gender { get; set; } = null;
        public int? AgeRange { get; set; } = null;
        public string InventoryCount { get; set; }
        public IFormFile? Image { get; set; } = null;
        public decimal? Sale { get; set; } = null;
        public decimal PriceSale { get; set; }
        public List<IFormFile> Gallery { get; set; } = new List<IFormFile>();
    }
}
