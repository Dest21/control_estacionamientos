using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstacionamientosApp.Models
{
    public class ParkingAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public int CarId { get; set; }

        [Required]
        public int ParkingSpaceId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Expired, Cancelled, Suspended

        [StringLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; } = null!;

        [ForeignKey("CarId")]
        public virtual Car Car { get; set; } = null!;

        [ForeignKey("ParkingSpaceId")]
        public virtual ParkingSpace ParkingSpace { get; set; } = null!;

        [NotMapped]
        public string DisplayInfo => $"{Client?.FullName} - {Car?.DisplayName} - {ParkingSpace?.DisplayName}";

        [NotMapped]
        public bool IsCurrentlyActive => IsActive && Status == "Active" && 
            (StartDate == null || StartDate <= DateTime.Now) && 
            (EndDate == null || EndDate >= DateTime.Now);
    }
}