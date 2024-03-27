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
        public IFormFile Image { get; set; } 
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
        public string? PopularText { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int InventoryCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
    }
    public class ProductUpdateDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Popular? Popular { get; set; } = null;
        public int InventoryCount { get; set; }
        public IFormFile Image { get; set; } 
        public List<IFormFile> Gallery { get; set; } = new List<IFormFile>();
    }
}
