using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstacionamientosApp.Data;
using EstacionamientosApp.Models;

namespace EstacionamientosApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Clients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients(
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.Clients.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.FirstName.Contains(search) ||
                                        c.LastName.Contains(search) ||
                                        c.Email.Contains(search) ||
                                        c.DocumentNumber.Contains(search));
            }

            if (isActive.HasValue)
            {
                query = query.Where(c => c.IsActive == isActive.Value);
            }

            var totalCount = await query.CountAsync();
            var clients = await query
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(c => c.Cars)
                .Include(c => c.ParkingAssignments)
                .ToListAsync();

            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            return clients;
        }

        // GET: api/Clients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            var client = await _context.Clients
                .Include(c => c.Cars)
                .Include(c => c.ParkingAssignments)
                    .ThenInclude(pa => pa.ParkingSpace)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            return client;
        }

        // PUT: api/Clients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutClient(int id, Client client)
        {
            if (id != client.Id)
            {
                return BadRequest();
            }

            // Check if email or document number already exists for another client
            var existingClient = await _context.Clients
                .Where(c => c.Id != id && (c.Email == client.Email || c.DocumentNumber == client.DocumentNumber))
                .FirstOrDefaultAsync();

            if (existingClient != null)
            {
                return Conflict("Email or document number already exists for another client.");
            }

            _context.Entry(client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
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

        // POST: api/Clients
        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(Client client)
        {
            // Check if email or document number already exists
            var existingClient = await _context.Clients
                .Where(c => c.Email == client.Email || c.DocumentNumber == client.DocumentNumber)
                .FirstOrDefaultAsync();

            if (existingClient != null)
            {
                return Conflict("Email or document number already exists.");
            }

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetClient", new { id = client.Id }, client);
        }

        // DELETE: api/Clients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            // Check if client has active parking assignments
            var hasActiveAssignments = await _context.ParkingAssignments
                .AnyAsync(pa => pa.ClientId == id && pa.IsActive && pa.Status == "Active");

            if (hasActiveAssignments)
            {
                return BadRequest("Cannot delete client with active parking assignments.");
            }

            // Soft delete - mark as inactive instead of removing
            client.IsActive = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}