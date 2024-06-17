using Ma3ak.Dtos;
using Ma3ak.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Ma3ak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WorkerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Workers
        [HttpGet("GetAllWorkers")]
        public async Task<IActionResult> GetAllAsync()
        {
            var workers = await _context.Workers.Include(w => w.MaintenanceCenter).ToListAsync();
            if (workers == null || workers.Count == 0)
                return NotFound(new { StatusCode = 404, Message = "No workers found!" });

            return Ok(new
            {
                StatusCode = 200,
                Message = "Here are all the workers in the Database",
                Workers = workers.Select(worker => new
                {
                    worker.Id,
                    worker.Name,
                    worker.Phone,
                    worker.MaintenanceCenterId,
                    MaintenanceCenterName = worker.MaintenanceCenter.CenterName
                })
            });
        }

        // GET: api/Workers/5
        [HttpGet("{id},\"GetAllWorkersById\"")]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            var worker = await _context.Workers
                .Include(w => w.MaintenanceCenter)
                .SingleOrDefaultAsync(w => w.Id == id);

            if (worker == null)
                return NotFound(new { StatusCode = 404, Message = $"No worker found with ID: {id}" });

            return Ok(new
            {
                StatusCode = 200,
                Message = $"Worker with ID: {id} found",
                Worker = new
                {
                    worker.Id,
                    worker.Name,
                    worker.Phone,
                    worker.MaintenanceCenterId,
                    MaintenanceCenterName = worker.MaintenanceCenter.CenterName
                }
            });
        }

        // POST: api/Workers
        [HttpPost("AddWorker")]
        public async Task<IActionResult> PostAsync([FromForm] WorkerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var worker = new Worker
            {
                Name = dto.Name,
                Phone = dto.Phone,
                MaintenanceCenterId = dto.MaintenanceCenterId
            };

            try
            {
                await _context.Workers.AddAsync(worker);
                await _context.SaveChangesAsync();

                var createdWorker = await _context.Workers
                    .Include(w => w.MaintenanceCenter)
                    .SingleOrDefaultAsync(w => w.Id == worker.Id);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Worker added successfully",
                    Worker = new
                    {
                        createdWorker.Id,
                        createdWorker.Name,
                        createdWorker.Phone,
                        createdWorker.MaintenanceCenterId,
                        MaintenanceCenterName = createdWorker.MaintenanceCenter.CenterName
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to add worker", Error = ex.Message });
            }
        }

        // PUT: api/Workers/5
        [HttpPut("{id},\"UpdateById\"")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] WorkerDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var worker = await _context.Workers.FindAsync(id);
            if (worker == null)
            {
                return NotFound(new { StatusCode = 404, Message = $"No worker found with ID: {id}" });
            }

            worker.Name = dto.Name;
            worker.Phone = dto.Phone;
            worker.MaintenanceCenterId = dto.MaintenanceCenterId;

            try
            {
                _context.Workers.Update(worker);
                await _context.SaveChangesAsync();

                var updatedWorker = await _context.Workers
                    .Include(w => w.MaintenanceCenter)
                    .SingleOrDefaultAsync(w => w.Id == worker.Id);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Worker updated successfully",
                    Worker = new
                    {
                        updatedWorker.Id,
                        updatedWorker.Name,
                        updatedWorker.Phone,
                        updatedWorker.MaintenanceCenterId,
                        MaintenanceCenterName = updatedWorker.MaintenanceCenter.CenterName
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to update worker", Error = ex.Message });
            }
        }

        // DELETE: api/Workers/5
        [HttpDelete("{id},\"DeleteByCarId\"")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var worker = await _context.Workers.FindAsync(id);
            if (worker == null)
            {
                return NotFound(new { StatusCode = 404, Message = $"No worker found with ID: {id}" });
            }

            try
            {
                _context.Workers.Remove(worker);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Message = "Worker deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to delete worker", Error = ex.Message });
            }
        }
    }
}





//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Ma3ak.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class WorkerController : ControllerBase
//    {




//        private readonly ApplicationDbContext _context;

//        public WorkerController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/Workers
//        [HttpGet("GetAllWorkers")]
//        public async Task<IActionResult> GetAllAsync()
//        {
//            var workers = await _context.Workers.ToListAsync();
//            if (workers == null || workers.Count == 0)
//                return NotFound(new { StatusCode = 404, Message = "No workers found!" });

//            return Ok(new { StatusCode = 200, Message = "Here are all the workers in the Database", Workers = workers });
//        }

//        // GET: api/Workers/5
//        [HttpGet("{id},\"GetAllWorkersById\"")]
//        public async Task<IActionResult> GetByIdAsync([FromForm]int id)
//        {
//            var worker = await _context.Workers.FindAsync(id);
//            if (worker == null)
//                return NotFound(new { StatusCode = 404, Message = $"No worker found with ID: {id}" });

//            return Ok(new { StatusCode = 200, Message = $"Worker with ID: {id} found", Worker = worker });
//        }

//        // POST: api/Workers
//        [HttpPost("AddWorker")]
//        public async Task<IActionResult> PostAsync([FromForm] WorkerDto dto)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var worker = new Worker
//            {
//                Name = dto.Name,
//                Phone = dto.Phone,
//                MaintenanceCenterId = dto.MaintenanceCenterId
//            };

//            try
//            {
//                await _context.Workers.AddAsync(worker);
//                await _context.SaveChangesAsync();
//                return Ok(new { StatusCode = 200, Message = "Worker added successfully", Worker = worker });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { StatusCode = 500, Message = "Failed to add worker", Error = ex.Message });
//            }
//        }

//        // PUT: api/Workers/5
//        [HttpPut("{id},\"UpdateById\"")]
//        public async Task<IActionResult> UpdateAsync(int id, [FromBody] WorkerDto dto)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var worker = await _context.Workers.FindAsync(id);
//            if (worker == null)
//            {
//                return NotFound(new { StatusCode = 404, Message = $"No worker found with ID: {id}" });
//            }

//            worker.Name = dto.Name;
//            worker.Phone = dto.Phone;
//            worker.MaintenanceCenterId = dto.MaintenanceCenterId;

//            try
//            {
//                _context.Workers.Update(worker);
//                await _context.SaveChangesAsync();
//                return Ok(new { StatusCode = 200, Message = "Worker updated successfully", Worker = worker });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { StatusCode = 500, Message = "Failed to update worker", Error = ex.Message });
//            }
//        }

//        // DELETE: api/Workers/5
//        [HttpDelete("{id},\"DeleteByCarId\"")]
//        public async Task<IActionResult> DeleteAsync(int id)
//        {
//            var worker = await _context.Workers.FindAsync(id);
//            if (worker == null)
//            {
//                return NotFound(new { StatusCode = 404, Message = $"No worker found with ID: {id}" });
//            }

//            try
//            {
//                _context.Workers.Remove(worker);
//                await _context.SaveChangesAsync();
//                return Ok(new { StatusCode = 200, Message = "Worker deleted successfully" });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { StatusCode = 500, Message = "Failed to delete worker", Error = ex.Message });
//            }
//        }
//    }
//}
