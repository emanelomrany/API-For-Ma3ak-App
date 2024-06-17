using Ma3ak.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ma3ak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApppicturesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly List<string> _allowedExtensions = new List<string> { ".jpg", ".png" };
        private readonly long _maxAllowedPosterSize = 1048576;

        public ApppicturesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("AllPictures")]
        public async Task<IActionResult> GetAllAsync()
        {
            var apppictures = await _context.Apppictures.Where(u => u.isDeleted == false).ToListAsync();

            if (apppictures == null || apppictures.Count == 0)
                return NotFound(new { StatusCode = 404, Message = "No pictures found!" });

            return Ok(new { StatusCode = 200, Message = "Here are all the pictures in the Database", Apppictures = apppictures });
        }

        [HttpGet("GetPictureById/{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var apppictures = await _context.Apppictures
                .Where(u => u.ApppictureId == id && u.isDeleted == false)
                .SingleOrDefaultAsync();

            if (apppictures == null)
                return NotFound(new { StatusCode = 404, Message = $"No picture was found with ID: {id}" });

            return Ok(new { StatusCode = 200, Message = $"The Apppicture with ID: {apppictures.ApppictureId} already exists", Apppicture = apppictures });
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetByNameAsync(string name)
        {
            var apppictures = await _context.Apppictures
                .Where(u => u.ApppictureName == name && u.isDeleted == false)
                .SingleOrDefaultAsync();

            if (apppictures == null)
                return NotFound(new { StatusCode = 404, Message = $"No picture was found with name: {name}" });

            return Ok(new { StatusCode = 200, Message = $"The Apppicture with name: {apppictures.ApppictureName} already exists", Apppicture = apppictures });
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetPictureCount()
        {
            var apppictureCount = await _context.Apppictures.Where(u => u.isDeleted == false).CountAsync();
            return Ok(new { StatusCode = 200, Message = $"The picture Count: {apppictureCount}" });
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromForm] CreateApppictureDtocs dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var apppictures = new Apppicture
            {
                ApppictureName = dto.ApppictureName,
                isDeleted = false
            };

            if (dto.pictures != null && dto.pictures.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{dto.pictures.FileName}";
                var filePath = Path.Combine("wwwroot/images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.pictures.CopyToAsync(fileStream);
                }
                apppictures.pictures = $"/images/{fileName}";
            }
            else
            {
                apppictures.pictures = null;
            }

            await _context.Apppictures.AddAsync(apppictures);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Message = "The picture has already been added", Apppicture = apppictures });
        }

        [HttpPut("UpdatePictureById/{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] CreateApppictureDtocs dto)
        {
            var apppictures = await _context.Apppictures.SingleOrDefaultAsync(u => u.ApppictureId == id && u.isDeleted == false);
            if (apppictures == null)
                return NotFound(new { StatusCode = 404, Message = $"No picture was found with ID: {id}" });

            if (dto.pictures != null && dto.pictures.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{dto.pictures.FileName}";
                var filePath = Path.Combine("wwwroot/images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.pictures.CopyToAsync(fileStream);
                }
                apppictures.pictures = $"/images/{fileName}";
            }

            apppictures.ApppictureName = dto.ApppictureName;
            apppictures.isDeleted = false;

            await _context.SaveChangesAsync();
            return Ok(new { StatusCode = 200, Message = "The picture has already been updated", Apppicture = apppictures });
        }

        [HttpPut("UpdatePictureByName/{name}")]
        public async Task<IActionResult> UpdateByNameAsync(string name, [FromForm] CreateApppictureDtocs dto)
        {
            var apppictures = await _context.Apppictures.SingleOrDefaultAsync(u => u.ApppictureName == name && u.isDeleted == false);
            if (apppictures == null)
                return NotFound(new { StatusCode = 404, Message = $"No picture was found with name: {name}" });

            if (dto.pictures != null && dto.pictures.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{dto.pictures.FileName}";
                var filePath = Path.Combine("wwwroot/images", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.pictures.CopyToAsync(fileStream);
                }
                apppictures.pictures = $"/images/{fileName}";
            }

            apppictures.ApppictureName = dto.ApppictureName;
            apppictures.isDeleted = false;

            await _context.SaveChangesAsync();
            return Ok(new { StatusCode = 200, Message = "The picture has already been updated", Apppicture = apppictures });
        }

        [HttpDelete("DeletePictureById/{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var apppictures = await _context.Apppictures.SingleOrDefaultAsync(u => u.ApppictureId == id && u.isDeleted == false);
            if (apppictures == null)
                return NotFound(new { StatusCode = 404, Message = $"No picture was found with ID: {id}" });

            apppictures.isDeleted = true;   
            await _context.SaveChangesAsync();
            return Ok(new { StatusCode = 200, Message = "The picture has already been deleted", Apppicture = apppictures });
        }

        [HttpDelete("DeletePictureByName/{name}")]
        public async Task<IActionResult> DeleteByNameAsync(string name)
        {
            var apppictures = await _context.Apppictures.SingleOrDefaultAsync(u => u.ApppictureName == name && u.isDeleted == false);
            if (apppictures == null)
                return NotFound(new { StatusCode = 404, Message = $"No picture was found with name: {name}" });

            apppictures.isDeleted = true;
            await _context.SaveChangesAsync();
            return Ok(new { StatusCode = 200, Message = "The picture has already been deleted", Apppicture = apppictures });
        }
    }
}


