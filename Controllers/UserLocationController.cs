using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ma3ak.Models;
using Ma3ak.Dtos;
using NuGet.Packaging.Signing;

namespace Ma3ak.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLocationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserLocationController(ApplicationDbContext context)
        {
            _context = context;
        }

       // GET: api/UserLocation/GetAllLocations
[HttpGet("GetAllLocations")]
public async Task<ActionResult<IEnumerable<UserLocation>>> GetUserLocations()
{
    var userLocations = await _context.userLocations.ToListAsync();
    var userLocationDtos = userLocations.Select(ul => new USERLOCATION2dto
    {
        Id = ul.Id,
        Latitude = ul.Latitude,
        Longitude = ul.Longitude,
        Timestamp = ul.LocalTimestamp
    }).ToList();

    return Ok(userLocationDtos);
}

// GET: api/UserLocation/5
[HttpGet("{id}")]
public async Task<ActionResult<UserLocationDto>> GetUserLocation(int id)
{
    var userLocation = await _context.userLocations.FindAsync(id);

    if (userLocation == null)
    {
        return NotFound();
    }

    var userLocationDto = new USERLOCATION2dto
    {
        Id = userLocation.Id,
        Latitude = userLocation.Latitude,
        Longitude = userLocation.Longitude,
        Timestamp = userLocation.LocalTimestamp
    };

    return Ok(userLocationDto);
}

// DELETE: api/UserLocation/5
[HttpDelete("{id}")]
public async Task<ActionResult<UserLocationDto>> DeleteUserLocation(int id)
{
    var userLocation = await _context.userLocations.FindAsync(id);
    if (userLocation == null)
    {
        return NotFound();
    }

    _context.userLocations.Remove(userLocation);
    await _context.SaveChangesAsync();

    var userLocationDto = new USERLOCATION2dto
    {
        Id = userLocation.Id,
        Latitude = userLocation.Latitude,
        Longitude = userLocation.Longitude,
        Timestamp = userLocation.LocalTimestamp
    };

    return Ok(userLocationDto);
}

private bool UserLocationExists(int id)
{
    return _context.userLocations.Any(e => e.Id == id);
}


        // POST: api/UserLocation/CalculateNearestMaintenanceCenter
        [HttpPost("CalculateNearestMaintenanceCenter")]
        public async Task<ActionResult<NearestMaintenanceCenterDto>> CalculateNearestMaintenanceCenter([FromForm] UserLocationDto userLocationDto)
        {
            // Get user location
            var userLocationLatitude = userLocationDto.Latitude;
            var userLocationLongitude = userLocationDto.Longitude;

            // Save user location to the database
            var userLocation = new UserLocation
            {
                Latitude = userLocationLatitude,
                Longitude = userLocationLongitude,
                Timestamp = DateTime.UtcNow
            };

            _context.userLocations.Add(userLocation);
            await _context.SaveChangesAsync();

            // Get all maintenance centers
            var maintenanceCenters = await _context.MaintenanceCenters.Include(mc => mc.Workers).ToListAsync();

            // Initialize variables for shortest distance
            double shortestDistance = double.MaxValue;
            MaintenanceCenter nearestMaintenanceCenter = null;
            Worker nearestWorker = null;

            // Iterate through each maintenance center
            foreach (var center in maintenanceCenters)
            {
                // Ignore centers that are marked as deleted
                if (center.isDeleted)
                {
                    continue;
                }

                // Calculate distance between user location and maintenance center
                var distance = CalculateDistance(userLocationLatitude, userLocationLongitude, center.Latitude, center.Longitude);

                // Update shortest distance and nearest maintenance center if needed
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestMaintenanceCenter = center;
                    nearestWorker = center.Workers.FirstOrDefault();
                }
            }

            // Return nearest maintenance center
            if (nearestMaintenanceCenter != null)
            {
                var nearestMaintenanceCenterDto = new NearestMaintenanceCenterDto
                {
                    CenterName = nearestMaintenanceCenter.CenterName,
                    
                    PhoneNumber = nearestMaintenanceCenter.PhoneNumber,
                    CentersPoster= nearestMaintenanceCenter.CentersPoster,
                    CenterLocation= nearestMaintenanceCenter.CenterLocation,
                    CenterRate = nearestMaintenanceCenter.CenterRate,
                    //WorkerName = nearestWorker?.Name,
                    //WorkerPhoneNumber = nearestWorker?.Phone,
                    Distance = shortestDistance,
                    Timestamp = userLocation.Timestamp.ToLocalTime(),
                    Message = "The maintenance center has accepted the request. Please wait for them to contact you."
                };
                return Ok(nearestMaintenanceCenterDto);
            }

            return NotFound("No maintenance centers found.");
        }

        private double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            var R = 6371; // Radius of the earth in km
            var dLat = ToRadians((double)(lat2 - lat1));
            var dLon = ToRadians((double)(lon2 - lon1));
            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c; // Distance in km
            return distance;
        }

        private double ToRadians(double deg)
        {
            return deg * (Math.PI / 180);
        }


      


    }
}
