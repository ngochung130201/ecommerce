using ecommerce.Enums;

namespace ecommerce.DTO
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        // Password is usually not included in a DTO
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public AccountStatus Status { get; set; }
    }
}
