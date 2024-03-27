using ecommerce.Enums;

namespace ecommerce.DTO
{
    public class ProductFilterDto
    {
        public Popular Popular { get; set; } = 0;
        public bool SortByDate { get; set; } = false; // sap xep theo ngay cu nhat hoac moi nhat
        // Sort min or max
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
        // name
        public string? Name { get; set; } = null;
        // category
        public int CategoryId { get; set; } = 0;
        public int InventoryCount { get; set; } = 0;
        // number of products
        // SortOrder
        public bool IsSortPrice { get; set; } = true;
        public string SortPrice { get; set; } = "asc";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

    }
}