using ecommerce.Enums;

namespace ecommerce.DTO
{
    public class ForgotPasswordOtp
    {
        public string Email { get; set; } = string.Empty;
        public string Template { get; set; } = string.Empty;
    }
}