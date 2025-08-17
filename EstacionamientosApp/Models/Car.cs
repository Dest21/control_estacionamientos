using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstacionamientosApp.Models
{
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string LicensePlate { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Color { get; set; }

        public int Year { get; set; }

        [Required]
        public int ClientId { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; } = null!;

        public virtual ICollection<ParkingAssignment> ParkingAssignments { get; set; } = new List<ParkingAssignment>();

        [NotMapped]
        public string DisplayName => $"{Brand} {Model} ({LicensePlate})";
    }
}