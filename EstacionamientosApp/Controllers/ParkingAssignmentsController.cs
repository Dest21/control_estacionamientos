using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstacionamientosApp.Data;
using EstacionamientosApp.Models;

namespace EstacionamientosApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingAssignmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ParkingAssignmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ParkingAssignments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParkingAssignment>>> GetParkingAssignments(
            [FromQuery] string? search = null,
            [FromQuery] int? clientId = null,
            [FromQuery] int? carId = null,
            [FromQuery] int? parkingSpaceId = null,
            [FromQuery] string? status = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.ParkingAssignments
                .Include(pa => pa.Client)
                .Include(pa => pa.Car)
                .Include(pa => pa.ParkingSpace)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(pa => pa.Client.FirstName.Contains(search) ||
                                         pa.Client.LastName.Contains(search) ||
                                         pa.Car.LicensePlate.Contains(search) ||
                                         pa.ParkingSpace.SpaceNumber.Contains(search));
            }

            if (clientId.HasValue)
            {
                query = query.Where(pa => pa.ClientId == clientId.Value);
            }

            if (carId.HasValue)
            {
                query = query.Where(pa => pa.CarId == carId.Value);
            }

            if (parkingSpaceId.HasValue)
            {
                query = query.Where(pa => pa.ParkingSpaceId == parkingSpaceId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(pa => pa.Status == status);
            }

            if (isActive.HasValue)
            {
                query = query.Where(pa => pa.IsActive == isActive.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(pa => pa.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(pa => pa.EndDate <= endDate.Value);
            }

            var totalCount = await query.CountAsync();
            var assignments = await query
                .OrderByDescending(pa => pa.AssignedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            return assignments;
        }

        // GET: api/ParkingAssignments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ParkingAssignment>> GetParkingAssignment(int id)
        {
            var parkingAssignment = await _context.ParkingAssignments
                .Include(pa => pa.Client)
                .Include(pa => pa.Car)
                .Include(pa => pa.ParkingSpace)
                .FirstOrDefaultAsync(pa => pa.Id == id);

            if (parkingAssignment == null)
            {
                return NotFound();
            }

            return parkingAssignment;
        }

        // GET: api/ParkingAssignments/Active
        [HttpGet("Active")]
        public async Task<ActionResult<IEnumerable<ParkingAssignment>>> GetActiveAssignments()
        {
            var activeAssignments = await _context.ParkingAssignments
                .Include(pa => pa.Client)
                .Include(pa => pa.Car)
                .Include(pa => pa.ParkingSpace)
                .Where(pa => pa.IsActive && pa.Status == "Active")
                .OrderBy(pa => pa.ParkingSpace.Zone)
                .ThenBy(pa => pa.ParkingSpace.SpaceNumber)
                .ToListAsync();

            return activeAssignments;
        }

        // GET: api/ParkingAssignments/ByClient/5
        [HttpGet("ByClient/{clientId}")]
        public async Task<ActionResult<IEnumerable<ParkingAssignment>>> GetAssignmentsByClient(int clientId)
        {
            var assignments = await _context.ParkingAssignments
                .Include(pa => pa.Car)
                .Include(pa => pa.ParkingSpace)
                .Where(pa => pa.ClientId == clientId)
                .OrderByDescending(pa => pa.AssignedDate)
                .ToListAsync();

            return assignments;
        }

        // PUT: api/ParkingAssignments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParkingAssignment(int id, ParkingAssignment parkingAssignment)
        {
            if (id != parkingAssignment.Id)
            {
                return BadRequest();
            }

            // Validate that client, car, and parking space exist
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == parkingAssignment.ClientId);
            var carExists = await _context.Cars.AnyAsync(c => c.Id == parkingAssignment.CarId);
            var spaceExists = await _context.ParkingSpaces.AnyAsync(ps => ps.Id == parkingAssignment.ParkingSpaceId);

            if (!clientExists || !carExists || !spaceExists)
            {
                return BadRequest("Client, car, or parking space does not exist.");
            }

            // Check if the car belongs to the client
            var carBelongsToClient = await _context.Cars
                .AnyAsync(c => c.Id == parkingAssignment.CarId && c.ClientId == parkingAssignment.ClientId);

            if (!carBelongsToClient)
            {
                return BadRequest("The selected car does not belong to the specified client.");
            }

            // If changing to active status, check if parking space is available
            if (parkingAssignment.Status == "Active" && parkingAssignment.IsActive)
            {
                var spaceOccupied = await _context.ParkingAssignments
                    .AnyAsync(pa => pa.Id != id && 
                                   pa.ParkingSpaceId == parkingAssignment.ParkingSpaceId && 
                                   pa.IsActive && 
                                   pa.Status == "Active");

                if (spaceOccupied)
                {
                    return BadRequest("Parking space is already occupied by another active assignment.");
                }
            }

            parkingAssignment.ModifiedDate = DateTime.Now;
            _context.Entry(parkingAssignment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Update parking space availability
                await UpdateParkingSpaceAvailability(parkingAssignment.ParkingSpaceId);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParkingAssignmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ParkingAssignments
        [HttpPost]
        public async Task<ActionResult<ParkingAssignment>> PostParkingAssignment(ParkingAssignment parkingAssignment)
        {
            // Validate that client, car, and parking space exist
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == parkingAssignment.ClientId);
            var carExists = await _context.Cars.AnyAsync(c => c.Id == parkingAssignment.CarId);
            var spaceExists = await _context.ParkingSpaces.AnyAsync(ps => ps.Id == parkingAssignment.ParkingSpaceId);

            if (!clientExists || !carExists || !spaceExists)
            {
                return BadRequest("Client, car, or parking space does not exist.");
            }

            // Check if the car belongs to the client
            var carBelongsToClient = await _context.Cars
                .AnyAsync(c => c.Id == parkingAssignment.CarId && c.ClientId == parkingAssignment.ClientId);

            if (!carBelongsToClient)
            {
                return BadRequest("The selected car does not belong to the specified client.");
            }

            // Check if parking space is available for active assignments
            if (parkingAssignment.Status == "Active" && parkingAssignment.IsActive)
            {
                var spaceOccupied = await _context.ParkingAssignments
                    .AnyAsync(pa => pa.ParkingSpaceId == parkingAssignment.ParkingSpaceId && 
                                   pa.IsActive && 
                                   pa.Status == "Active");

                if (spaceOccupied)
                {
                    return BadRequest("Parking space is already occupied by another active assignment.");
                }
            }

            _context.ParkingAssignments.Add(parkingAssignment);
            await _context.SaveChangesAsync();

            // Update parking space availability
            await UpdateParkingSpaceAvailability(parkingAssignment.ParkingSpaceId);

            return CreatedAtAction("GetParkingAssignment", new { id = parkingAssignment.Id }, parkingAssignment);
        }

        // DELETE: api/ParkingAssignments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingAssignment(int id)
        {
            var parkingAssignment = await _context.ParkingAssignments.FindAsync(id);
            if (parkingAssignment == null)
            {
                return NotFound();
            }

            var parkingSpaceId = parkingAssignment.ParkingSpaceId;

            // Soft delete - mark as inactive and cancelled
            parkingAssignment.IsActive = false;
            parkingAssignment.Status = "Cancelled";
            parkingAssignment.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            // Update parking space availability
            await UpdateParkingSpaceAvailability(parkingSpaceId);

            return NoContent();
        }

        // POST: api/ParkingAssignments/5/Revoke
        [HttpPost("{id}/Revoke")]
        public async Task<IActionResult> RevokeAssignment(int id, [FromBody] string? reason = null)
        {
            var assignment = await _context.ParkingAssignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }

            assignment.Status = "Cancelled";
            assignment.IsActive = false;
            assignment.ModifiedDate = DateTime.Now;
            if (!string.IsNullOrEmpty(reason))
            {
                assignment.Notes = (assignment.Notes ?? "") + $"\nRevoked: {reason}";
            }

            await _context.SaveChangesAsync();

            // Update parking space availability
            await UpdateParkingSpaceAvailability(assignment.ParkingSpaceId);

            return NoContent();
        }

        private async Task UpdateParkingSpaceAvailability(int parkingSpaceId)
        {
            var parkingSpace = await _context.ParkingSpaces.FindAsync(parkingSpaceId);
            if (parkingSpace != null)
            {
                var hasActiveAssignment = await _context.ParkingAssignments
                    .AnyAsync(pa => pa.ParkingSpaceId == parkingSpaceId && 
                                   pa.IsActive && 
                                   pa.Status == "Active");

                parkingSpace.IsAvailable = !hasActiveAssignment;
                await _context.SaveChangesAsync();
            }
        }

        private bool ParkingAssignmentExists(int id)
        {
            return _context.ParkingAssignments.Any(e => e.Id == id);
        }
    }
}