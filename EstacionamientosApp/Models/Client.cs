using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstacionamientosApp.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string DocumentType { get; set; } = "DNI";

        [StringLength(15)]
        public string? Phone { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Address { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Car> Cars { get; set; } = new List<Car>();
        public virtual ICollection<ParkingAssignment> ParkingAssignments { get; set; } = new List<ParkingAssignment>();

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}