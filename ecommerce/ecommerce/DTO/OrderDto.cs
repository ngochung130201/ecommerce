namespace ecommerce.DTO
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
