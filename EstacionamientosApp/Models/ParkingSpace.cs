using System.ComponentModel.DataAnnotations;

namespace EstacionamientosApp.Models
{
    public class ParkingSpace
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string SpaceNumber { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Zone { get; set; }

        [StringLength(20)]
        public string SpaceType { get; set; } = "Regular"; // Regular, Compact, Disabled, VIP

        public bool IsAvailable { get; set; } = true;

        public bool IsActive { get; set; } = true;

        [StringLength(200)]
        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<ParkingAssignment> ParkingAssignments { get; set; } = new List<ParkingAssignment>();

        public string DisplayName => $"Espacio {SpaceNumber}" + (Zone != null ? $" - {Zone}" : "");
    }
}