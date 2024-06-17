using Ma3ak.Dtos;
using Ma3ak.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ma3ak.Controllers
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

        [HttpGet("GetAllCars")]
        public async Task<IActionResult> GetAllAsync()
        {

            var cars = await _context.Cars
                .Where(u => u.isDeleted == false)
                .Include(m => m.User)
                .Select(c => new CarDetailsDto
                {
                    UserId = c.UserId,
                    CarId = c.CarId,
                    Model = c.Model,
                    isDeleted = false,
                    UserFirstName = c.User.FirstName,
                    UserLastName = c.User.LastName,
                    CarName = c.CarName




                }).ToListAsync();
            if (cars == null || cars.Count == 0)
                return NotFound(new { StatusCode = 404, Message = $"No Car found !" });
            return Ok(new { StatusCode = 200, Message = $"Here are all the cars in the Database", CarCont = cars.Count, Car = cars });
        }


        [HttpGet("{username},\"GetAllCarsByUserName\"")]
        public async Task<IActionResult> GetAllByusernameAsync(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
            if (user == null)
                return NotFound(new { StatusCode = 404, Message = $"No User was found with UserName : {username}" });

            var cars = await _context.Cars
                .Where(c => c.UserId == user.Id && !c.isDeleted)
                .Include(c => c.User)
                .Select(c => new CarDetailsDto
                {
                    UserId = c.UserId,
                    CarId = c.CarId,
                    Model = c.Model,
                    isDeleted = false,
                    UserFirstName = c.User.FirstName,
                    UserLastName = c.User.LastName,
                    CarName = c.CarName
                })
                .ToListAsync();


            if (cars == null || cars.Count == 0)
                return NotFound(new { StatusCode = 404, Message = $"No Cars were found for User with UserName: {username}" });

            return Ok(new { StatusCode = 200, Message = $"All Cars for User with UserName: {username} have been retrieved", Cars = cars });
        }


        [HttpGet("Count")]
        public async Task<IActionResult> GetCarCount()
        {
            var carCount = await _context.Cars.Where(u => u.isDeleted == false).CountAsync();
            return Ok(new { StatusCode = 200, Message = $"The cars Count : {carCount}" });
        }




        [HttpPost("AddCar")]
        public async Task<IActionResult> PostAsync([FromForm] CreateCarDtocs dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
            {
                return BadRequest(new { StatusCode = 400, Message = "Invalid User ID !" });
            }

            var car = new Car
            {
                UserId = user.Id, // قم بتعيين القيمة الفعلية للمستخدم من خاصية Id
                CarName = dto.CarName,
                Model = dto.Model,
                isDeleted = false
            };

            await _context.Cars.AddAsync(car);
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Message = "The Car has been added successfully", Car = car });
        }



        [HttpPut("{id},\"UpdateByCarId\"")]
        public async Task<IActionResult> UpdateByCarIdAsync(int id, [FromForm] UpdateCarDto dto)
        {

            var car = await _context.Cars.SingleOrDefaultAsync(u => u.CarId == id);
            if (car == null)
                return NotFound(new { StatusCode = 404, Message = $"No car was found with ID : {id}" });
            var isValiedUser = await _context.Users.AnyAsync(u => u.UserName == dto.UserName);
            if (!isValiedUser)
            {
                return BadRequest(new { StatusCode = 400, Message = "InValid User  !" });
            }
            car.CarName = dto.CarName;
            car.Model = dto.Model;
            car.isDeleted = false;


            _context.SaveChanges();
            return Ok(new { StatusCode = 200, Message = $"The Car has already been updated", Car = car });
        }
        [HttpPut("{username},\"UpdateCarByUserName\"")]
        public async Task<IActionResult> UpdatCarsByUserNameAsync(string username, [FromForm] UpdateCarDto dto)
        {
            // البحث عن المستخدم باستخدام اسم المستخدم
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
            if (user == null)
                return NotFound(new { StatusCode = 404, Message = $"No User was found with UserName : {username}" });

            // البحث عن السيارة بناءً على معرف المستخدم المعثور عليه
            var car = await _context.Cars.SingleOrDefaultAsync(c => c.UserId == user.Id && c.CarId == dto.CarId && !c.isDeleted);
            if (car == null)
                return NotFound(new { StatusCode = 404, Message = $"No Car was found for User with UserName: {username} and Car ID: {dto.CarId}" });

            // تحديث بيانات السيارة
            car.CarName = dto.CarName;
            car.Model = dto.Model;
            car.isDeleted = false;
            // يمكنك إضافة تحديثات أخرى هنا

            // حفظ التغييرات
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Message = $"Car with ID: {car.CarId} has been updated successfully for User with UserName: {username}", Car = car });
        }





        [HttpDelete("{username}/{carId}\"DeleteCar\"")]
        public async Task<IActionResult> DeleteAsync(string username, int carId)
        {
            // البحث عن المستخدم باستخدام اسم المستخدم
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
            if (user == null)
                return NotFound(new { StatusCode = 404, Message = $"No User was found with UserName : {username}" });

            // البحث عن السيارة بناءً على معرف المستخدم ومعرف السيارة
            var car = await _context.Cars.SingleOrDefaultAsync(c => c.UserId == user.Id && c.CarId == carId);
            if (car == null)
                return NotFound(new { StatusCode = 404, Message = $"No Car was found for User with UserName: {username} and Car ID: {carId}" });

            // تعيين القيمة isDeleted إلى true بدلاً من حذف السجل بالكامل
            car.isDeleted = true;
            await _context.SaveChangesAsync();

            return Ok(new { StatusCode = 200, Message = $"Car with ID: {car.CarId} has been soft-deleted successfully for User with UserName: {username}", Car = car });
        }




    }
}


