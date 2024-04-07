using ecommerce.Enums;
using ecommerce.Models;

namespace ecommerce.DTO
{
    public class AdminDto
    {
        public int AdminId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        // Password is usually not included in a DTO
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = null;
        public AdminRole AdminRole { get; set; }
        public AccountStatus Status { get; set; }
    }
}
