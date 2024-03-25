using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ecommerce.Enums;

namespace ecommerce.Models
{
    [Table("admin")]
    public class Admin
    {
        [Key]
        [Column("admin_id")]
        public int AdminId { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("email")]
        [EmailAddress]
        public string Email { get; set; }

        [Column("password_hash")]
        public byte[] PasswordHash { get; set; }
        [Column("password_salt")]
        public byte[] PasswordSalt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; } = null;
        // role
        [Column("role")]
        public AdminRole Role { get; set; }
        // role text
        public string? RoleText { get; set; } = null;
        public Admin(AdminRole Role)
        {
            this.RoleText = nameof(Role);
            this.Role = Role;
        }
        public Admin()
        {
            
        }
    }
}
