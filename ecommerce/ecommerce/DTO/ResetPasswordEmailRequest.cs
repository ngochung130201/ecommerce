using ecommerce.Enums;

namespace ecommerce.DTO
{
    public class ResetPasswordEmailRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}