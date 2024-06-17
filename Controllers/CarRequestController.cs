using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ma3ak.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ma3ak.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CarRequest
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarRequest>>> GetCarRequests()
        {
            return await _context.CarRequests.ToListAsync();
        }

        // GET: api/CarRequest/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CarRequest>> GetCarRequest(int id)
        {
            var carRequest = await _context.CarRequests.FindAsync(id);

            if (carRequest == null)
            {
                return NotFound();
            }

            return Ok(carRequest);
        }

        // POST: api/CarRequest
        [HttpPost]
        public async Task<ActionResult<CarRequest>> PostCarRequest([FromForm] CarRequestDto carRequestDto)
        {
            var carRequest = new CarRequest
            {
                ToWhere = carRequestDto.ToWhere,
                When = carRequestDto.When,
                Number_Of_Passengers = carRequestDto.Number_Of_Passengers,
                Comments = carRequestDto.Comments
            };

            _context.CarRequests.Add(carRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCarRequest), new { id = carRequest.Id }, new { message = "Your order has been registered and the car will arrive to you soon" });
        }

        // PUT: api/CarRequest/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCarRequest(int id, [FromForm] CarRequestDto carRequestDto)
        {
            var carRequest = await _context.CarRequests.FindAsync(id);
            if (carRequest == null)
            {
                return NotFound();
            }

            carRequest.ToWhere = carRequestDto.ToWhere;
            carRequest.When = carRequestDto.When;
            carRequest.Number_Of_Passengers = carRequestDto.Number_Of_Passengers;
            carRequest.Comments = carRequestDto.Comments;

            _context.Entry(carRequest).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/CarRequest/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CarRequest>> DeleteCarRequest(int id)
        {
            var carRequest = await _context.CarRequests.FindAsync(id);
            if (carRequest == null)
            {
                return NotFound();
            }

            _context.CarRequests.Remove(carRequest);
            await _context.SaveChangesAsync();

            return Ok(carRequest);
        }
    }
}
