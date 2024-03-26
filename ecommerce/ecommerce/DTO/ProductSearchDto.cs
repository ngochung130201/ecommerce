namespace ecommerce.DTO
{
    public class ProductSearchDto
    {
        public string Name { get; set; }
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = 0;
        public int CategoryId { get; set; } = 0;
    }
}
