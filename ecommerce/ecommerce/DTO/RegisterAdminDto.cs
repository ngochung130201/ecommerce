using ecommerce.Enums;

namespace ecommerce.DTO
{
    public class RegisterAdminDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public AdminRole Role { get; set; }
        public string EmailForSupperAdmin { get; set; }
    }
}
