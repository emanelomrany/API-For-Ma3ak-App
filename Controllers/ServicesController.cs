using Ma3ak.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Ma3ak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private new List<string> _allowedExtentions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;
        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetAllServices")]
        public async Task<IActionResult> GetAllAsync()
        {
            var services = await _context.Service
                .Where(u => u.isDeleted == false)
                .Select(s => new
                {
                    s.ServiceId,
                    s.ServiceName,
                    s.ServiceDescription,
                    ServicePoster = s.ServicePoster == null ? null : $"{Request.Scheme}://{Request.Host}{s.ServicePoster}",
                    ServiceLogo = s.ServiceLogo == null ? null : $"{Request.Scheme}://{Request.Host}{s.ServiceLogo}",
                    s.isDeleted
                })
                .ToListAsync();

            if (services == null || services.Count == 0)
                return NotFound(new { StatusCode = 404, Message = $"No Services found." });

            return Ok(new { StatusCode = 200, Message = $"Here are all the services in the Database", ServiceCount = services.Count, Service = services });
        }

        [HttpGet("{id:int}", Name = "GetServiceByServiceID")]
        public async Task<IActionResult> GetServiceByIdAsync(int id)
        {
            var service = await _context.Service
                .Where(u => u.ServiceId == id && u.isDeleted == false)
                .Select(s => new
                {
                    s.ServiceId,
                    s.ServiceName,
                    s.ServiceDescription,
                    ServicePoster = s.ServicePoster == null ? null : $"{Request.Scheme}://{Request.Host}{s.ServicePoster}",
                    ServiceLogo = s.ServiceLogo == null ? null : $"{Request.Scheme}://{Request.Host}{s.ServiceLogo}",
                    s.isDeleted
                })
                .SingleOrDefaultAsync();

            if (service == null)
                return NotFound(new { StatusCode = 404, Message = $"No Service was found with ID : {id}" });

            return Ok(new { StatusCode = 200, Message = $"The Service with ID: {service.ServiceId} already exists", Service = service });
        }

        [HttpGet("{name}", Name = "GetServiceByNameAsync")]
        public async Task<IActionResult> GetServiceByNameAsync(string name)
        {
            var services = await _context.Service
                .Where(u => u.ServiceName == name && u.isDeleted == false)
                .Select(s => new
                {
                    s.ServiceId,
                    s.ServiceName,
                    s.ServiceDescription,
                    ServicePoster = s.ServicePoster == null ? null : $"{Request.Scheme}://{Request.Host}{s.ServicePoster}",
                    ServiceLogo = s.ServiceLogo == null ? null : $"{Request.Scheme}://{Request.Host}{s.ServiceLogo}",
                    s.isDeleted
                })
                .ToListAsync();

            if (services == null || services.Count == 0)
                return NotFound(new { StatusCode = 404, Message = $"No Service was found with Name : {name}" });
            else if (services.Count > 1)
            {
                var servicesDetails = services.Select((service, index) => new
                {
                    Index = index + 1,
                    ServiceId = service.ServiceId,
                    ServiceName = service.ServiceName,
                    ServiceDescription = service.ServiceDescription,
                    ServicePoster = service.ServicePoster,
                    ServiceLogo = service.ServiceLogo,
                    isDeleted = service.isDeleted
                });

                return Ok(new { StatusCode = 200, Message = $"Multiple services found with the name: {name}", ServicesDetails = servicesDetails });
            }
            else
            {
                var service = services.FirstOrDefault();
                return Ok(new { StatusCode = 200, Message = $"The Service with Name: {service.ServiceName} already exists", Service = service });
            }
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetServiceCount()
        {
            var serviceCount = await _context.Service.Where(u => u.isDeleted == false).CountAsync();
            return Ok(new { StatusCode = 200, Message = $"The services Count : {serviceCount}" });
        }

        [HttpPost("AddService")]
        public async Task<IActionResult> AddServiceAsync([FromForm] CreateServiceDtocs serviceDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var service = new Service
            {
                ServiceName = serviceDto.ServiceName,
                ServiceDescription = serviceDto.ServiceDescription,
                isDeleted = false
            };

            if (serviceDto.ServicePoster != null)
            {
                var posterFileName = $"{Guid.NewGuid()}_{serviceDto.ServicePoster.FileName}";
                var posterFilePath = Path.Combine("wwwroot/images", posterFileName);
                using (var posterStream = new FileStream(posterFilePath, FileMode.Create))
                {
                    await serviceDto.ServicePoster.CopyToAsync(posterStream);
                }
                service.ServicePoster = $"/images/{posterFileName}";
            }

            if (serviceDto.ServiceLogo != null)
            {
                var logoFileName = $"{Guid.NewGuid()}_{serviceDto.ServiceLogo.FileName}";
                var logoFilePath = Path.Combine("wwwroot/images", logoFileName);
                using (var logoStream = new FileStream(logoFilePath, FileMode.Create))
                {
                    await serviceDto.ServiceLogo.CopyToAsync(logoStream);
                }
                service.ServiceLogo = $"/images/{logoFileName}";
            }

            try
            {
                await _context.Service.AddAsync(service);
                await _context.SaveChangesAsync();
                return Ok(new { StatusCode = 200, Message = "Service added successfully", Service = service });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Failed to add service", Error = "The ServerName is already in use. Try using another ServerName" });
            }
        }

        [HttpPut("UpdateById/{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromForm] CreateServiceDtocs dto)
        {
            var service = await _context.Service.SingleOrDefaultAsync(u => u.ServiceId == id);
            if (service == null)
                return NotFound(new { StatusCode = 404, Message = $"No Service was found with ID : {id}" });

            if (dto.ServicePoster != null && dto.ServicePoster.Length > 0)
            {
                var posterFileName = $"{Guid.NewGuid()}_{dto.ServicePoster.FileName}";
                var posterFilePath = Path.Combine("wwwroot/images", posterFileName);
                using (var posterStream = new FileStream(posterFilePath, FileMode.Create))
                {
                    await dto.ServicePoster.CopyToAsync(posterStream);
                }
                service.ServicePoster = $"/images/{posterFileName}";
            }

            if (dto.ServiceLogo != null && dto.ServiceLogo.Length > 0)
            {
                var logoFileName = $"{Guid.NewGuid()}_{dto.ServiceLogo.FileName}";
                var logoFilePath = Path.Combine("wwwroot/images", logoFileName);
                using (var logoStream = new FileStream(logoFilePath, FileMode.Create))
                {
                    await dto.ServiceLogo.CopyToAsync(logoStream);
                }
                service.ServiceLogo = $"/images/{logoFileName}";
            }

            service.ServiceName = dto.ServiceName;
            service.ServiceDescription = dto.ServiceDescription;
            service.isDeleted = false;

            _context.SaveChanges();
            return Ok(new { StatusCode = 200, Message = $"The service has already been updated", Service = service });
        }

        [HttpPut("{name}", Name = "UpdateServiceByName")]
        public async Task<IActionResult> UpdateByNameAsync(string name, [FromForm] CreateServiceDtocs dto)
        {
            var service = await _context.Service.SingleOrDefaultAsync(u => u.ServiceName == name);
            if (service == null)
                return NotFound(new { StatusCode = 404, Message = $"No Service was found with Name : {name}" });

            if (dto.ServicePoster != null)
            {
                var posterFileName = $"{Guid.NewGuid()}_{dto.ServicePoster.FileName}";
                var posterFilePath = Path.Combine("wwwroot/images", posterFileName);
                using (var posterStream = new FileStream(posterFilePath, FileMode.Create))
                {
                    await dto.ServicePoster.CopyToAsync(posterStream);
                }
                service.ServicePoster = $"/images/{posterFileName}";
            }

            if (dto.ServiceLogo != null)
            {
                var logoFileName = $"{Guid.NewGuid()}_{dto.ServiceLogo.FileName}";
                var logoFilePath = Path.Combine("wwwroot/images", logoFileName);
                using (var logoStream = new FileStream(logoFilePath, FileMode.Create))






                {
                    await dto.ServiceLogo.CopyToAsync(logoStream);
                }
                service.ServiceLogo = $"/images/{logoFileName}";
            }

            service.ServiceName = dto.ServiceName;
            service.ServiceDescription = dto.ServiceDescription;
            service.isDeleted = false;

            _context.SaveChanges();

            return Ok(new { StatusCode = 200, Message = $"The service has already been updated", Service = service });
        }

        [HttpDelete("{id},\"DeleteService\"")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var services = await _context.Service.SingleOrDefaultAsync(u => u.ServiceId == id);
            if (services == null || services.isDeleted == true)
                return NotFound(new { StatusCode = 404, Message = $"No Service was found with ID : {id}" });
            services.isDeleted = true;
            _context.SaveChanges();
            return Ok(new { StatusCode = 200, Message = $"The Service has already been Deleted", Service = services });
        }

        [HttpDelete("{name}", Name = "DeleteServiceByName")]
        public async Task<IActionResult> DeleteServiceByNameAsync(string name)
        {
            var service = await _context.Service.SingleOrDefaultAsync(u => u.ServiceName == name);
            if (service == null || service.isDeleted == true)
                return NotFound(new { StatusCode = 404, Message = $"No Service was found with Name : {name}" });

            service.isDeleted = true;
            _context.SaveChanges();

            return Ok(new { StatusCode = 200, Message = $"The Service has already been Deleted", Service = service });
        }

        [HttpDelete("DeleteFieldByServiceNameAsync/{serviceName}")]
        public async Task<IActionResult> DeleteFieldByServiceNameAsync(string serviceName, [FromForm] string fieldName)
        {
            try
            {
                var service = await _context.Service.SingleOrDefaultAsync(s => s.ServiceName == serviceName);

                if (service == null)
                {
                    return NotFound(new { StatusCode = 404, Message = $"No service found with the name: {serviceName}" });
                }

                if (string.IsNullOrEmpty(fieldName))
                {
                    return BadRequest(new { StatusCode = 400, Message = "Field name cannot be empty" });
                }

                switch (fieldName)
                {
                    case "ServicePoster":
                        service.ServicePoster = null;
                        break;
                    case "ServiceLogo":
                        service.ServiceLogo = null;
                        break;
                    default:
                        return BadRequest(new { StatusCode = 400, Message = "Invalid field name" });
                }

                await _context.SaveChangesAsync();

                return Ok(new { StatusCode = 200, Message = $"Data in the field '{fieldName}' of service '{serviceName}' has been deleted successfully", Service = service });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while deleting data: {ex.Message}" });
            }
        }
    }
}





//using Ma3ak.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Ma3ak.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ServicesController : ControllerBase
//    {

//        private readonly ApplicationDbContext _context;
//        private new List<string> _allowedExtentions = new List<string> { ".jpg", ".png" };
//        private long _maxAllowedPosterSize = 1048576;
//        public ServicesController(ApplicationDbContext context)
//        {
//            _context = context;
//        }


//        [HttpGet("GetAllServices")]
//        public async Task<IActionResult> GetAllAsync()
//        {
//            var services = await _context.Service
//                .Where(u => u.isDeleted == false)
//                .Select(s => new
//                {
//                    s.ServiceId,
//                    s.ServiceName,
//                    s.ServiceDescription,
//                    ServicePoster = s.ServicePoster == null ? null : s.ServicePoster,
//                    ServiceLogo = s.ServiceLogo == null ? null : s.ServiceLogo,
//                    s.isDeleted
//                })
//                .ToListAsync();

//            if (services == null || services.Count == 0)
//                return NotFound(new { StatusCode = 404, Message = $"No Services found." });

//            return Ok(new { StatusCode = 200, Message = $"Here are all the services in the Database", ServiceCount = services.Count, Service = services });
//        }


//        [HttpGet("{id:int}", Name = "GetServiceByServiceID")]
//        public async Task<IActionResult> GetServiceByIdAsync(int id)
//        {
//            var service = await _context.Service
//                .Where(u => u.ServiceId == id && u.isDeleted == false)
//                .Select(s => new
//                {
//                    s.ServiceId,
//                    s.ServiceName,
//                    s.ServiceDescription,
//                    ServicePoster = s.ServicePoster == null ? null : s.ServicePoster,
//                    ServiceLogo = s.ServiceLogo == null ? null : s.ServiceLogo,
//                    s.isDeleted
//                })
//                .SingleOrDefaultAsync();

//            if (service == null)
//                return NotFound(new { StatusCode = 404, Message = $"No Service was found with ID : {id}" });

//            return Ok(new { StatusCode = 200, Message = $"The Service with ID: {service.ServiceId} already exists", Service = service });
//        }
//        [HttpGet("{name}", Name = "GetServiceByNameAsync")]
//        public async Task<IActionResult> GetServiceByNameAsync(string name)
//        {
//            var services = await _context.Service
//                .Where(u => u.ServiceName == name && u.isDeleted == false)
//                .Select(s => new
//                {
//                    s.ServiceId,
//                    s.ServiceName,
//                    s.ServiceDescription,
//                    ServicePoster = s.ServicePoster == null ? null : s.ServicePoster,
//                    ServiceLogo = s.ServiceLogo == null ? null : s.ServiceLogo,
//                    s.isDeleted
//                })
//                .ToListAsync();

//            if (services == null || services.Count == 0)
//                return NotFound(new { StatusCode = 404, Message = $"No Service was found with Name : {name}" });
//            else if (services.Count > 1)
//            {
//                // إذا وُجِد أكثر من خدمة بنفس الاسم
//                var servicesDetails = services.Select((service, index) => new
//                {
//                    Index = index + 1,
//                    ServiceId = service.ServiceId,
//                    ServiceName = service.ServiceName,
//                    ServiceDescription = service.ServiceDescription,
//                    ServicePoster = service.ServicePoster,
//                    ServiceLogo = service.ServiceLogo,
//                    isDeleted = service.isDeleted
//                });

//                return Ok(new { StatusCode = 200, Message = $"Multiple services found with the name: {name}", ServicesDetails = servicesDetails });
//            }
//            else
//            {
//                // إذا وُجِد خدمة واحدة فقط بالاسم المحدد
//                var service = services.FirstOrDefault();
//                return Ok(new { StatusCode = 200, Message = $"The Service with Name: {service.ServiceName} already exists", Service = service });
//            }
//        }




//        [HttpGet("count")]
//        public async Task<IActionResult> GetServiceCount()
//        {
//            var serviceCount = await _context.Service.Where(u => u.isDeleted == false).CountAsync();
//            return Ok(new { StatusCode = 200, Message = $"The services Count : {serviceCount}" });
//        }

//        [HttpPost("AddService")]
//        public async Task<IActionResult> AddServiceAsync([FromForm] CreateServiceDtocs serviceDto)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            // تحويل ServiceDto إلى Service
//            var service = new Service
//            {
//                ServiceName = serviceDto.ServiceName,
//                ServiceDescription = serviceDto.ServiceDescription,
//                isDeleted = false
//            };

//            // إذا تم رفع ServicePoster
//            if (serviceDto.ServicePoster != null)
//            {
//                using (var posterStream = new MemoryStream())
//                {
//                    await serviceDto.ServicePoster.CopyToAsync(posterStream);
//                    service.ServicePoster = posterStream.ToArray();
//                }
//            }

//            // إذا تم رفع ServiceLogo
//            if (serviceDto.ServiceLogo != null)
//            {
//                using (var logoStream = new MemoryStream())
//                {
//                    await serviceDto.ServiceLogo.CopyToAsync(logoStream);
//                    service.ServiceLogo = logoStream.ToArray();
//                }
//            }

//            try
//            {
//                // إضافة الخدمة إلى قاعدة البيانات
//                await _context.Service.AddAsync(service);
//                // حفظ التغييرات
//                await _context.SaveChangesAsync();
//                // إرجاع استجابة ناجحة
//                return Ok(new { StatusCode = 200, Message = "Service added successfully", Service = service });
//            }
//            catch (Exception ex)
//            {
//                // إرجاع استجابة فشل في حالة حدوث خطأ
//                return StatusCode(500, new { StatusCode = 500, Message = "Failed to add service  ", Error = "The ServerName is already in use. Try using another ServerName " });
//            }
//        }
//        [HttpPut("UpdateById/{id}")]
//        public async Task<IActionResult> UpdateAsync(int id, [FromForm] CreateServiceDtocs dto)
//        {
//            var service = await _context.Service.SingleOrDefaultAsync(u => u.ServiceId == id);
//            if (service == null)
//                return NotFound(new { StatusCode = 404, Message = $"No Service was found with ID : {id}" });

//            // إذا تم رفع ServicePoster
//            if (dto.ServicePoster != null && dto.ServicePoster.Length > 0)
//            {
//                using (var posterStream = new MemoryStream())
//                {
//                    await dto.ServicePoster.CopyToAsync(posterStream);
//                    service.ServicePoster = posterStream.ToArray();
//                }
//            }

//            // إذا تم رفع ServiceLogo
//            if (dto.ServiceLogo != null && dto.ServiceLogo.Length > 0)
//            {
//                using (var logoStream = new MemoryStream())
//                {
//                    await dto.ServiceLogo.CopyToAsync(logoStream);
//                    service.ServiceLogo = logoStream.ToArray();
//                }
//            }

//            service.ServiceName = dto.ServiceName;
//            service.ServiceDescription = dto.ServiceDescription;
//            service.isDeleted = false;

//            _context.SaveChanges();
//            return Ok(new { StatusCode = 200, Message = $"The service has already been updated", Service = service });
//        }


//        [HttpPut("{name}", Name = "UpdateServiceByName")]
//        public async Task<IActionResult> UpdateByNameAsync(string name, [FromForm] CreateServiceDtocs dto)
//        {
//            var service = await _context.Service.SingleOrDefaultAsync(u => u.ServiceName == name);
//            if (service == null)
//                return NotFound(new { StatusCode = 404, Message = $"No Service was found with Name : {name}" });

//            // إذا تم رفع ServicePoster
//            if (dto.ServicePoster != null)
//            {
//                using (var posterStream = new MemoryStream())
//                {
//                    await dto.ServicePoster.CopyToAsync(posterStream);
//                    service.ServicePoster = posterStream.ToArray();
//                }
//            }

//            // إذا تم رفع ServiceLogo
//            if (dto.ServiceLogo != null)
//            {
//                using (var logoStream = new MemoryStream())
//                {
//                    await dto.ServiceLogo.CopyToAsync(logoStream);
//                    service.ServiceLogo = logoStream.ToArray();
//                }
//            }

//            service.ServiceName = dto.ServiceName;
//            service.ServiceDescription = dto.ServiceDescription;
//            service.isDeleted = false;

//            _context.SaveChanges();

//            return Ok(new { StatusCode = 200, Message = $"The service has already been updated", Service = service });
//        }




//        [HttpDelete("{id},\"DeleteService\"")]
//        public async Task<IActionResult> DeleteAsync(int id)
//        {

//            var services = await _context.Service.SingleOrDefaultAsync(u => u.ServiceId == id);
//            if (services == null || services.isDeleted == true)
//                return NotFound(new { StatusCode = 404, Message = $"No Service was found with ID : {id}" });
//            services.isDeleted = true; // تعيين قيمة isDeleted إلى true بدلاً من إزالة السجل
//                                       //  _context.Users.Remove(user);
//            _context.SaveChanges();
//            return Ok(new { StatusCode = 200, Message = $"The Service has already been Deleted", Service = services });
//        }

//        [HttpDelete("{name}", Name = "DeleteServiceByName")]
//        public async Task<IActionResult> DeleteServiceByNameAsync(string name)
//        {
//            var service = await _context.Service.SingleOrDefaultAsync(u => u.ServiceName == name);
//            if (service == null || service.isDeleted == true)
//                return NotFound(new { StatusCode = 404, Message = $"No Service was found with Name : {name}" });

//            service.isDeleted = true;
//            _context.SaveChanges();

//            return Ok(new { StatusCode = 200, Message = $"The Service has already been Deleted", Service = service });
//        }
//        [HttpDelete("DeleteFieldByServiceNameAsync/{serviceName}")]
//        public async Task<IActionResult> DeleteFieldByServiceNameAsync(string serviceName, [FromForm] string fieldName)
//        {
//            try
//            {
//                // البحث عن الخدمة باستخدام اسم الخدمة
//                var service = await _context.Service.SingleOrDefaultAsync(s => s.ServiceName == serviceName);

//                if (service == null)
//                {
//                    return NotFound(new { StatusCode = 404, Message = $"No service found with the name: {serviceName}" });
//                }

//                // التأكد من أن اسم الحقل لم يأتِ فارغًا
//                if (string.IsNullOrEmpty(fieldName))
//                {
//                    return BadRequest(new { StatusCode = 400, Message = "Field name cannot be empty" });
//                }

//                // حذف البيانات من الحقل المحدد
//                switch (fieldName)
//                {

//                    case "ServicePoster":
//                        service.ServicePoster = null;
//                        break;
//                    case "ServiceLogo":
//                        service.ServiceLogo = null;
//                        break;
 
//                    // قم بإضافة المزيد من الحقول إذا كان هناك حاجة إليها
//                    default:
//                        return BadRequest(new { StatusCode = 400, Message = "Invalid field name" });
//                }

//                // حفظ التغييرات في قاعدة البيانات
//                await _context.SaveChangesAsync();

//                return Ok(new { StatusCode = 200, Message = $"Data in the field '{fieldName}' of service '{serviceName}' has been deleted successfully", Service = service });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { StatusCode = 500, Message = $"An error occurred while deleting data: {ex.Message}" });
//            }
//        }


//    }
//}

