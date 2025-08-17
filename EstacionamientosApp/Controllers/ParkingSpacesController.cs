using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstacionamientosApp.Data;
using EstacionamientosApp.Models;

namespace EstacionamientosApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingSpacesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ParkingSpacesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ParkingSpaces
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParkingSpace>>> GetParkingSpaces(
            [FromQuery] string? search = null,
            [FromQuery] string? zone = null,
            [FromQuery] string? spaceType = null,
            [FromQuery] bool? isAvailable = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var query = _context.ParkingSpaces.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(ps => ps.SpaceNumber.Contains(search) ||
                                         ps.Zone.Contains(search) ||
                                         ps.Description.Contains(search));
            }

            if (!string.IsNullOrEmpty(zone))
            {
                query = query.Where(ps => ps.Zone == zone);
            }

            if (!string.IsNullOrEmpty(spaceType))
            {
                query = query.Where(ps => ps.SpaceType == spaceType);
            }

            if (isAvailable.HasValue)
            {
                query = query.Where(ps => ps.IsAvailable == isAvailable.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(ps => ps.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();
            var parkingSpaces = await query
                .OrderBy(ps => ps.Zone)
                .ThenBy(ps => ps.SpaceNumber)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(ps => ps.ParkingAssignments.Where(pa => pa.IsActive))
                .ToListAsync();

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            return parkingSpaces;
        }

        // GET: api/ParkingSpaces/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ParkingSpace>> GetParkingSpace(int id)
        {
            var parkingSpace = await _context.ParkingSpaces
                .Include(ps => ps.ParkingAssignments)
                    .ThenInclude(pa => pa.Client)
                .Include(ps => ps.ParkingAssignments)
                    .ThenInclude(pa => pa.Car)
                .FirstOrDefaultAsync(ps => ps.Id == id);

            if (parkingSpace == null)
            {
                return NotFound();
            }

            return parkingSpace;
        }

        // GET: api/ParkingSpaces/Available
        [HttpGet("Available")]
        public async Task<ActionResult<IEnumerable<ParkingSpace>>> GetAvailableParkingSpaces(
            [FromQuery] string? spaceType = null)
        {
            var query = _context.ParkingSpaces
                .Where(ps => ps.IsAvailable && ps.IsActive);

            if (!string.IsNullOrEmpty(spaceType))
            {
                query = query.Where(ps => ps.SpaceType == spaceType);
            }

            var availableSpaces = await query
                .OrderBy(ps => ps.Zone)
                .ThenBy(ps => ps.SpaceNumber)
                .ToListAsync();

            return availableSpaces;
        }

        // GET: api/ParkingSpaces/Zones
        [HttpGet("Zones")]
        public async Task<ActionResult<IEnumerable<string>>> GetZones()
        {
            var zones = await _context.ParkingSpaces
                .Where(ps => ps.IsActive && !string.IsNullOrEmpty(ps.Zone))
                .Select(ps => ps.Zone)
                .Distinct()
                .OrderBy(z => z)
                .ToListAsync();

            return zones;
        }

        // GET: api/ParkingSpaces/Types
        [HttpGet("Types")]
        public async Task<ActionResult<IEnumerable<string>>> GetSpaceTypes()
        {
            var types = await _context.ParkingSpaces
                .Where(ps => ps.IsActive)
                .Select(ps => ps.SpaceType)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();

            return types;
        }

        // PUT: api/ParkingSpaces/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutParkingSpace(int id, ParkingSpace parkingSpace)
        {
            if (id != parkingSpace.Id)
            {
                return BadRequest();
            }

            // Check if space number already exists for another parking space
            var existingSpace = await _context.ParkingSpaces
                .Where(ps => ps.Id != id && ps.SpaceNumber == parkingSpace.SpaceNumber)
                .FirstOrDefaultAsync();

            if (existingSpace != null)
            {
                return Conflict("Space number already exists for another parking space.");
            }

            _context.Entry(parkingSpace).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ParkingSpaceExists(id))
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

        // POST: api/ParkingSpaces
        [HttpPost]
        public async Task<ActionResult<ParkingSpace>> PostParkingSpace(ParkingSpace parkingSpace)
        {
            // Check if space number already exists
            var existingSpace = await _context.ParkingSpaces
                .Where(ps => ps.SpaceNumber == parkingSpace.SpaceNumber)
                .FirstOrDefaultAsync();

            if (existingSpace != null)
            {
                return Conflict("Space number already exists.");
            }

            _context.ParkingSpaces.Add(parkingSpace);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetParkingSpace", new { id = parkingSpace.Id }, parkingSpace);
        }

        // DELETE: api/ParkingSpaces/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingSpace(int id)
        {
            var parkingSpace = await _context.ParkingSpaces.FindAsync(id);
            if (parkingSpace == null)
            {
                return NotFound();
            }

            // Check if parking space has active assignments
            var hasActiveAssignments = await _context.ParkingAssignments
                .AnyAsync(pa => pa.ParkingSpaceId == id && pa.IsActive && pa.Status == "Active");

            if (hasActiveAssignments)
            {
                return BadRequest("Cannot delete parking space with active assignments.");
            }

            // Soft delete - mark as inactive instead of removing
            parkingSpace.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ParkingSpaceExists(int id)
        {
            return _context.ParkingSpaces.Any(e => e.Id == id);
        }
    }
}