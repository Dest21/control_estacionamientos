using EstacionamientosApp.Models;
using EstacionamientosApp.Services;
using Microsoft.EntityFrameworkCore;

namespace EstacionamientosApp.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context, AuthService authService)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Check if data already exists
            if (await context.Clients.AnyAsync())
            {
                return; // Database has been seeded
            }

            // Seed Default Admin User
            if (!await context.Users.AnyAsync())
            {
                var adminUser = new User
                {
                    Username = "admin",
                    PasswordHash = authService.HashPassword("admin123"),
                    FullName = "Administrador del Sistema",
                    Email = "admin@estacionamientos.com",
                    Role = "Admin",
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }

            // Seed Clients
            var clients = new List<Client>
            {
                new Client
                {
                    FirstName = "Juan",
                    LastName = "Pérez",
                    DocumentNumber = "12345678",
                    DocumentType = "DNI",
                    Phone = "987654321",
                    Email = "juan.perez@email.com",
                    Address = "Av. Lima 123, Lima",
                    RegistrationDate = DateTime.Now.AddDays(-30)
                },
                new Client
                {
                    FirstName = "María",
                    LastName = "García",
                    DocumentNumber = "87654321",
                    DocumentType = "DNI",
                    Phone = "912345678",
                    Email = "maria.garcia@email.com",
                    Address = "Jr. Cusco 456, Lima",
                    RegistrationDate = DateTime.Now.AddDays(-25)
                },
                new Client
                {
                    FirstName = "Carlos",
                    LastName = "López",
                    DocumentNumber = "11223344",
                    DocumentType = "DNI",
                    Phone = "998877665",
                    Email = "carlos.lopez@email.com",
                    Address = "Av. Arequipa 789, Lima",
                    RegistrationDate = DateTime.Now.AddDays(-20)
                },
                new Client
                {
                    FirstName = "Ana",
                    LastName = "Rodríguez",
                    DocumentNumber = "44332211",
                    DocumentType = "DNI",
                    Phone = "955443322",
                    Email = "ana.rodriguez@email.com",
                    Address = "Calle Los Olivos 321, Lima",
                    RegistrationDate = DateTime.Now.AddDays(-15)
                }
            };

            await context.Clients.AddRangeAsync(clients);
            await context.SaveChangesAsync();

            // Seed Parking Spaces
            var parkingSpaces = new List<ParkingSpace>();
            
            // Zone A - Regular spaces
            for (int i = 1; i <= 20; i++)
            {
                parkingSpaces.Add(new ParkingSpace
                {
                    SpaceNumber = $"A{i:D2}",
                    Zone = "Zona A",
                    SpaceType = "Regular",
                    Description = $"Espacio regular en Zona A - {i:D2}"
                });
            }

            // Zone B - Compact spaces
            for (int i = 1; i <= 15; i++)
            {
                parkingSpaces.Add(new ParkingSpace
                {
                    SpaceNumber = $"B{i:D2}",
                    Zone = "Zona B",
                    SpaceType = "Compact",
                    Description = $"Espacio compacto en Zona B - {i:D2}"
                });
            }

            // Zone C - VIP spaces
            for (int i = 1; i <= 5; i++)
            {
                parkingSpaces.Add(new ParkingSpace
                {
                    SpaceNumber = $"C{i:D2}",
                    Zone = "Zona C",
                    SpaceType = "VIP",
                    Description = $"Espacio VIP en Zona C - {i:D2}"
                });
            }

            // Disabled spaces
            for (int i = 1; i <= 3; i++)
            {
                parkingSpaces.Add(new ParkingSpace
                {
                    SpaceNumber = $"D{i:D2}",
                    Zone = "Zona Especial",
                    SpaceType = "Disabled",
                    Description = $"Espacio para personas con discapacidad - {i:D2}"
                });
            }

            await context.ParkingSpaces.AddRangeAsync(parkingSpaces);
            await context.SaveChangesAsync();

            // Seed Cars
            var cars = new List<Car>
            {
                new Car
                {
                    LicensePlate = "ABC-123",
                    Brand = "Toyota",
                    Model = "Corolla",
                    Color = "Blanco",
                    Year = 2020,
                    ClientId = clients[0].Id
                },
                new Car
                {
                    LicensePlate = "DEF-456",
                    Brand = "Honda",
                    Model = "Civic",
                    Color = "Azul",
                    Year = 2019,
                    ClientId = clients[0].Id
                },
                new Car
                {
                    LicensePlate = "GHI-789",
                    Brand = "Nissan",
                    Model = "Sentra",
                    Color = "Rojo",
                    Year = 2021,
                    ClientId = clients[1].Id
                },
                new Car
                {
                    LicensePlate = "JKL-012",
                    Brand = "Hyundai",
                    Model = "Elantra",
                    Color = "Negro",
                    Year = 2022,
                    ClientId = clients[2].Id
                },
                new Car
                {
                    LicensePlate = "MNO-345",
                    Brand = "Kia",
                    Model = "Rio",
                    Color = "Gris",
                    Year = 2020,
                    ClientId = clients[3].Id
                }
            };

            await context.Cars.AddRangeAsync(cars);
            await context.SaveChangesAsync();

            // Seed Parking Assignments
            var assignments = new List<ParkingAssignment>
            {
                new ParkingAssignment
                {
                    ClientId = clients[0].Id,
                    CarId = cars[0].Id,
                    ParkingSpaceId = parkingSpaces[0].Id,
                    StartDate = DateTime.Now.AddDays(-10),
                    EndDate = DateTime.Now.AddDays(20),
                    Status = "Active",
                    Notes = "Asignación mensual"
                },
                new ParkingAssignment
                {
                    ClientId = clients[1].Id,
                    CarId = cars[2].Id,
                    ParkingSpaceId = parkingSpaces[1].Id,
                    StartDate = DateTime.Now.AddDays(-5),
                    EndDate = DateTime.Now.AddDays(25),
                    Status = "Active",
                    Notes = "Cliente VIP"
                },
                new ParkingAssignment
                {
                    ClientId = clients[2].Id,
                    CarId = cars[3].Id,
                    ParkingSpaceId = parkingSpaces[20].Id, // Compact space
                    StartDate = DateTime.Now.AddDays(-3),
                    EndDate = DateTime.Now.AddDays(27),
                    Status = "Active",
                    Notes = "Espacio compacto"
                }
            };

            await context.ParkingAssignments.AddRangeAsync(assignments);
            await context.SaveChangesAsync();

            // Update parking space availability
            var assignedSpaceIds = assignments.Select(a => a.ParkingSpaceId).ToList();
            var assignedSpaces = await context.ParkingSpaces
                .Where(ps => assignedSpaceIds.Contains(ps.Id))
                .ToListAsync();

            foreach (var space in assignedSpaces)
            {
                space.IsAvailable = false;
            }

            await context.SaveChangesAsync();
        }
    }
}