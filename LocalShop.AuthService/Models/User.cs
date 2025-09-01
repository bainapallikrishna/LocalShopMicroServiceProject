using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalShop.AuthService.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    public class UserRole
    {
        [Key]
        public int UserRoleId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }
}
