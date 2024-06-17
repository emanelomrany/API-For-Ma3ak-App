
using Ma3ak.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Threading.Tasks;

namespace Ma3ak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceCentersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private List<string> _allowedExtentions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;

        public MaintenanceCentersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllMaintenanceCenter")]
        public async Task<IActionResult> GetAllAsync()
        {
            var maintenanceCenters = await _context.MaintenanceCenters.Where(u => u.isDeleted == false).ToListAsync();

            if (maintenanceCenters == null || maintenanceCenters.Count == 0)
                return NotFound(new { StatusCode = 404, Message = "No Maintenance Center found!" });

            return Ok(new { StatusCode = 200, Message = "Here are all the MaintenanceCenters in the Database", MaintenanceCenter = maintenanceCenters });
        }

        [HttpGet("{id},\"GetMaintenanceCenterByID\"")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var maintenanceCenter = await _context.MaintenanceCenters
                .Where(u => u.CenterId == id && u.isDeleted == false)
                .SingleOrDefaultAsync();

            if (maintenanceCenter == null)
                return NotFound(new { StatusCode = 404, Message = $"No Maintenance Center was found with ID: {id}" });

            return Ok(new { StatusCode = 200, Message = $"The Maintenance Center with ID: {maintenanceCenter.CenterId} already exists", MaintenanceCenter = maintenanceCenter });
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetMaintenanceCenterCount()
        {
            var maintenanceCentersCount = await _context.MaintenanceCenters.Where(u => u.isDeleted == false).CountAsync();
            return Ok(new { StatusCode = 200, Message = $"The Maintenance Center Count: {maintenanceCentersCount}" });
        }

        [HttpGet("GetCreateMaintenanceCenterByEmail")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new { StatusCode = 400, Message = "Email parameter is required." });

            var maintenanceCenter = await _context.MaintenanceCenters.FirstOrDefaultAsync(u => u.Email == email && u.isDeleted == false);

            if (maintenanceCenter == null)
                return NotFound(new { StatusCode = 404, Message = $"User with email '{email}' was not found." });

            return Ok(new { StatusCode = 200, Message = $"The user with This Email '{email}' has already been existed.", MaintenanceCenter = maintenanceCenter });
        }

        [HttpGet("GetMaintenanceCenterByCenterName")]
        public async Task<IActionResult> GetMaintenanceCenterByCenterName([FromQuery] string centerName)
        {
            if (string.IsNullOrEmpty(centerName))
                return BadRequest(new { StatusCode = 400, Message = "CenterName parameter is required." });

            var maintenanceCenter = await _context.MaintenanceCenters.FirstOrDefaultAsync(u => u.CenterName == centerName && u.isDeleted == false);

            if (maintenanceCenter == null)
                return NotFound(new { StatusCode = 404, Message = $"Maintenance center with name '{centerName}' was not found." });

            return Ok(new { StatusCode = 200, Message = $"The maintenance center with name '{centerName}' has already existed.", MaintenanceCenter = maintenanceCenter });
        }

        



        [HttpPost("AddCenter")]
        public async Task<IActionResult> PostAsync([FromForm] CreateMaintenanceCenterDtocs dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest(new { StatusCode = 400, Message = "Email is required!" });
            }

            var existingMaintenanceCenter = await _context.MaintenanceCenters.FirstOrDefaultAsync(u => u.Email == dto.Email && u.isDeleted == false);
            if (existingMaintenanceCenter != null)
            {
                return BadRequest(new { StatusCode = 400, Message = $"The email '{dto.Email}' is already in use. Please use a different email." });
            }

            var maintenanceCenter = new MaintenanceCenter
            {
                CenterName = dto.CenterName,
                CenterLocation = dto.CenterLocation,
                CenterRate = dto.CenterRate,
                PhoneNumber = dto.PhoneNumber,
                WorkingHours = dto.WorkingHours,
                Email = dto.Email,
                Password = dto.Password,
                CenterNationalID = dto.CenterNationalID,
                Latitude = dto.Latitude, // تأكد من تعيين هذه القيم
                Longitude = dto.Longitude, // تأكد من تعيين هذه القيم
                isDeleted = false
            };

            if (dto.CentersPoster != null)
            {
                var fileName = $"{Guid.NewGuid()}_{dto.CentersPoster.FileName}";
                var filePath = Path.Combine("wwwroot/images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.CentersPoster.CopyToAsync(fileStream);
                }
                maintenanceCenter.CentersPoster = $"/images/{fileName}";
            }
            else
            {
                maintenanceCenter.CentersPoster = null;
            }

            try
            {
                await _context.AddAsync(maintenanceCenter);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Message = "MaintenanceCenter added successfully", MaintenanceCenter = maintenanceCenter });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to add MaintenanceCenter", Error = "The MaintenanceCenter Email is already in use. Try using another Email " });
            }
        }


        [HttpPut("{id},\"UpdateCenterByID\"")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] CreateMaintenanceCenterDtocs dto)
        {
            if (string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest(new { StatusCode = 400, Message = "Email is required!" });
            }

            var maintenanceCenter = await _context.MaintenanceCenters.SingleOrDefaultAsync(u => u.CenterId == id);
            if (maintenanceCenter == null)
                return NotFound(new { StatusCode = 404, Message = $"No Maintenance Center Id was found with ID : {id}" });

            maintenanceCenter.CentersPoster = null;

            if (dto.CentersPoster != null)
            {
                var fileName = $"{Guid.NewGuid()}_{dto.CentersPoster.FileName}";
                var filePath = Path.Combine("wwwroot/images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.CentersPoster.CopyToAsync(fileStream);
                }
                maintenanceCenter.CentersPoster = $"/images/{fileName}";
            }

            maintenanceCenter.CenterName = dto.CenterName;
            maintenanceCenter.CenterLocation = dto.CenterLocation;
            maintenanceCenter.CenterRate = dto.CenterRate;
            maintenanceCenter.PhoneNumber = dto.PhoneNumber;
            maintenanceCenter.WorkingHours = dto.WorkingHours;
            maintenanceCenter.Email = dto.Email;
            maintenanceCenter.Password = dto.Password;
            maintenanceCenter.CenterNationalID = dto.CenterNationalID;
            maintenanceCenter.isDeleted = false;

            _context.SaveChanges();
            return Ok(new { StatusCode = 200, Message = $"The maintenanceCenter has already been updated", MaintenanceCenter = maintenanceCenter });
        }

        [HttpPut("UpdateByCenterName")]
        public async Task<IActionResult> UpdateByCenterName([FromForm] CreateMaintenanceCenterDtocs dto)
        {
            if (string.IsNullOrEmpty(dto.CenterName) || string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest(new { StatusCode = 400, Message = "CenterName and Email are required!" });
            }

            var maintenanceCenter = await _context.MaintenanceCenters.FirstOrDefaultAsync(u => u.CenterName == dto.CenterName && u.Email == dto.Email && u.isDeleted == false);
            if (maintenanceCenter == null)
                return NotFound(new { StatusCode = 404, Message = $"No Maintenance Center was found with Name: '{dto.CenterName}' and Email: '{dto.Email}'" });

            maintenanceCenter.CentersPoster = null;

            if (dto.CentersPoster != null)
            {
                var fileName = $"{Guid.NewGuid()}_{dto.CentersPoster.FileName}";
                var filePath = Path.Combine("wwwroot/images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.CentersPoster.CopyToAsync(fileStream);
                }
                maintenanceCenter.CentersPoster = $"/images/{fileName}";
            }

            maintenanceCenter.CenterLocation = dto.CenterLocation;
            maintenanceCenter.CenterRate = dto.CenterRate;
            maintenanceCenter.PhoneNumber = dto.PhoneNumber;
            maintenanceCenter.WorkingHours = dto.WorkingHours;
            maintenanceCenter.Password = dto.Password;
            maintenanceCenter.CenterNationalID = dto.CenterNationalID;
            maintenanceCenter.isDeleted = false;

            _context.SaveChanges();
            return Ok(new { StatusCode = 200, Message = $"The maintenanceCenter with the CenterName '{maintenanceCenter.CenterName}' has been updated.", MaintenanceCenter = maintenanceCenter });
        }

        [HttpDelete("{id},\"DeleteMaintenanceCenterByID\"")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var maintenanceCenter = await _context.MaintenanceCenters.SingleOrDefaultAsync(u => u.CenterId == id);
            if (maintenanceCenter == null)
                return NotFound(new { StatusCode = 404, Message = $"No Maintenance Center was found with ID : {id}" });

            maintenanceCenter.isDeleted = true;

            _context.SaveChanges();
            return Ok(new { StatusCode = 200, Message = $"The maintenanceCenter with ID: {maintenanceCenter.CenterId} has been deleted successfully" });
        }

        [HttpDelete("{centerName},\"DeleteMaintenanceCenterByCenterName\"")]
        public async Task<IActionResult> DeleteByCenterNameAsync(string centerName)
        {
            var maintenanceCenter = await _context.MaintenanceCenters.FirstOrDefaultAsync(u => u.CenterName == centerName);
            if (maintenanceCenter == null)
                return NotFound(new { StatusCode = 404, Message = $"No Maintenance Center was found with Name : {centerName}" });

            maintenanceCenter.isDeleted = true;

            _context.SaveChanges();
            return Ok(new { StatusCode = 200, Message = $"The maintenanceCenter with Name: {maintenanceCenter.CenterName} has been deleted successfully" });
        }
    }
}
