namespace ecommerce.DTO
{
    public class CartDto
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
    }
    public class CartAllDto
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public UserDto User { get; set; } = new UserDto();
        public List<CartItemDto> CartItems { get; set; }
    }

}
