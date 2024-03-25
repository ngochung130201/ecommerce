using ecommerce.Enums;

namespace ecommerce.DTO
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public bool IsAdmin { get; set; } = false;
        public UserDto? User { get; set; } = null;
        public AdminDto? Admin { get; set; } = null;
        public AdminRole? AdminRole { get; set; } = null;
    }
}
