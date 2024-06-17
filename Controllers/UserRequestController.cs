
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ma3ak.Models;
using Ma3ak.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ma3ak.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UserRequests/GetAllRequests
        [HttpGet("GetAllRequests")]
        public async Task<ActionResult<IEnumerable<UserRequest>>> GetUserRequests()
        {
            return await _context.UserRequests.ToListAsync();
        }

        // GET: api/UserRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserRequest>> GetUserRequest(int id)
        {
            var userRequest = await _context.UserRequests.FindAsync(id);

            if (userRequest == null || userRequest.IsActive == false)
            {
                return NotFound();
            }

            return Ok(userRequest);
        }

        // POST: api/UserRequests/AddUserRequest
        [HttpPost("AddUserRequest")]
        public async Task<ActionResult<UserRequest>> PostUserRequest([FromForm] UserRequestDto userRequestDto)
        {
            var userRequest = new UserRequest
            {
                UserPhone = userRequestDto.UserPhone,
                ProblemDescription = userRequestDto.ProblemDescription,
                CarName = userRequestDto.CarName,
                CarModel = userRequestDto.CarModel,
                IsActive = true // Assuming new requests are active by default
            };

            _context.UserRequests.Add(userRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserRequest), new { id = userRequest.Id }, userRequest);
        }

        
        // DELETE: api/UserRequests/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserRequest>> DeleteUserRequest(int id)
        {
            var userRequest = await _context.UserRequests.FindAsync(id);
            if (userRequest == null || userRequest.IsActive == false)
            {
                return NotFound();
            }

            userRequest.IsActive = false;
            _context.UserRequests.Update(userRequest);
            await _context.SaveChangesAsync();

            return Ok(userRequest);
        }

        private bool UserRequestExists(int id)
        {
            return _context.UserRequests.Any(e => e.Id == id);
        }
    }
}



