using Microsoft.EntityFrameworkCore;
using EstacionamientosApp.Models;

namespace EstacionamientosApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<ParkingSpace> ParkingSpaces { get; set; }
        public DbSet<ParkingAssignment> ParkingAssignments { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Client configuration
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasIndex(e => e.DocumentNumber).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.RegistrationDate).HasDefaultValueSql("GETDATE()");
            });

            // Car configuration
            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasIndex(e => e.LicensePlate).IsUnique();
                entity.Property(e => e.RegistrationDate).HasDefaultValueSql("GETDATE()");
                
                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Cars)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ParkingSpace configuration
            modelBuilder.Entity<ParkingSpace>(entity =>
            {
                entity.HasIndex(e => e.SpaceNumber).IsUnique();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
            });

            // ParkingAssignment configuration
            modelBuilder.Entity<ParkingAssignment>(entity =>
            {
                entity.Property(e => e.AssignedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ParkingAssignments)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.ParkingAssignments)
                    .HasForeignKey(d => d.CarId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.ParkingSpace)
                    .WithMany(p => p.ParkingAssignments)
                    .HasForeignKey(d => d.ParkingSpaceId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Ensure a parking space can only have one active assignment at a time
                entity.HasIndex(e => new { e.ParkingSpaceId, e.Status })
                    .HasFilter("[Status] = 'Active' AND [IsActive] = 1")
                    .IsUnique();
            });

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}