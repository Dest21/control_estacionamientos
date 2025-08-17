using System.ComponentModel.DataAnnotations;

namespace EstacionamientosApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = "";

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = "";

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = "";

        [StringLength(20)]
        public string Role { get; set; } = "Admin";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginDate { get; set; }
    }
}