namespace ecommerce.DTO
{
    public class OrderItemDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtTimeOfOrder { get; set; }
        public virtual ProductAllDto Product { get; set; }
    }
}
