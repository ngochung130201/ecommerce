using ecommerce.Enums;

namespace ecommerce.DTO
{
    public class ProductFilterDto
    {
        public Popular Popular { get; set; } = 0;

        public Gender Gender { get; set; } = 0;
        public bool SortByDate { get; set; } = false; // sap xep theo ngay cu nhat hoac moi nhat
        public string? MinAndMaxPrice { get; set; } = null; // gia tu va gia den 100-200
        // name
        public string? Name { get; set; } = null;
        // category
        public int CategoryId { get; set; } = 0;
        public int InventoryCount { get; set; } = 0;
        // number of products
        // SortOrder
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public decimal Price { get; set; } = 0;
        public bool SortByPrice { get; set; } = false;

    }
}