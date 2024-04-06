using ecommerce.Enums;

namespace ecommerce.DTO
{
    public class UpdateAccountRequest
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
        public bool? IsAdmin { get; set; } = false;
        public AdminRole? Role { get; set; } = null;
        public AccountStatus? Status { get; set; } = null;
    }
}