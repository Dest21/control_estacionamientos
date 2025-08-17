using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstacionamientosApp.Data;
using EstacionamientosApp.Models;

namespace EstacionamientosApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Cars
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Car>>> GetCars(
            [FromQuery] string? search = null,
            [FromQuery] int? clientId = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Cars.Include(c => c.Client).AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.LicensePlate.Contains(search) ||
                                        c.Brand.Contains(search) ||
                                        c.Model.Contains(search) ||
                                        c.Color.Contains(search));
            }

            if (clientId.HasValue)
            {
                query = query.Where(c => c.ClientId == clientId.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();
            var cars = await query
                .OrderBy(c => c.LicensePlate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(c => c.ParkingAssignments)
                .ToListAsync();

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            return cars;
        }

        // GET: api/Cars/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Car>> GetCar(int id)
        {
            var car = await _context.Cars
                .Include(c => c.Client)
                .Include(c => c.ParkingAssignments)
                    .ThenInclude(pa => pa.ParkingSpace)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return car;
        }

        // GET: api/Cars/ByClient/5
        [HttpGet("ByClient/{clientId}")]
        public async Task<ActionResult<IEnumerable<Car>>> GetCarsByClient(int clientId)
        {
            var cars = await _context.Cars
                .Where(c => c.ClientId == clientId && c.IsActive)
                .OrderBy(c => c.LicensePlate)
                .ToListAsync();

            return cars;
        }

        // PUT: api/Cars/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(int id, Car car)
        {
            if (id != car.Id)
            {
                return BadRequest();
            }

            // Check if license plate already exists for another car
            var existingCar = await _context.Cars
                .Where(c => c.Id != id && c.LicensePlate == car.LicensePlate)
                .FirstOrDefaultAsync();

            if (existingCar != null)
            {
                return Conflict("License plate already exists for another car.");
            }

            // Verify client exists
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == car.ClientId);
            if (!clientExists)
            {
                return BadRequest("Client does not exist.");
            }

            _context.Entry(car).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CarExists(id))
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

        // POST: api/Cars
        [HttpPost]
        public async Task<ActionResult<Car>> PostCar(Car car)
        {
            // Check if license plate already exists
            var existingCar = await _context.Cars
                .Where(c => c.LicensePlate == car.LicensePlate)
                .FirstOrDefaultAsync();

            if (existingCar != null)
            {
                return Conflict("License plate already exists.");
            }

            // Verify client exists
            var clientExists = await _context.Clients.AnyAsync(c => c.Id == car.ClientId);
            if (!clientExists)
            {
                return BadRequest("Client does not exist.");
            }

            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCar", new { id = car.Id }, car);
        }

        // DELETE: api/Cars/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            // Check if car has active parking assignments
            var hasActiveAssignments = await _context.ParkingAssignments
                .AnyAsync(pa => pa.CarId == id && pa.IsActive && pa.Status == "Active");

            if (hasActiveAssignments)
            {
                return BadRequest("Cannot delete car with active parking assignments.");
            }

            // Soft delete - mark as inactive instead of removing
            car.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}