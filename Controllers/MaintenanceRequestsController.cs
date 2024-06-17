//using Ma3ak.Models;
//using Ma3ak.Services;
//using Ma3ak.Dtos;  // إضافة مساحة الأسماء لـ UserRequestDto
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//[Route("api/[controller]")]
//[ApiController]
//public class MaintenanceRequestController : ControllerBase
//{
//    private readonly ApplicationDbContext _context;
//    private readonly IDistanceService _distanceService;
//    private readonly HttpClient _httpClient;

//    public MaintenanceRequestController(ApplicationDbContext context, IDistanceService distanceService, HttpClient httpClient)
//    {
//        _context = context;
//        _distanceService = distanceService;
//        _httpClient = httpClient;
//    }

//    [HttpPost]
//    public async Task<IActionResult> CreateRequest([FromForm] UserRequestDto userRequestDto)
//    {
//        // تحويل UserRequestDto إلى UserRequest
//        var userRequest = new UserRequest
//        {
//            UserPhone = userRequestDto.UserPhone,
//            ProblemDescription = userRequestDto.ProblemDescription,
//            CarName = userRequestDto.CarName,
//            CarModel = userRequestDto.CarModel,
//            UserLatitude = userRequestDto.UserLatitude,  // استخدام Latitude
//            UserLongitude = userRequestDto.UserLongitude,  // استخدام Longitude
//            IsActive = true
//        };

//        _context.UserRequests.Add(userRequest);
//        await _context.SaveChangesAsync();

//        // استخدام الموقع الممرر لحساب أقرب المراكز
//        var userLocation = new UserLocation(userRequest.UserLatitude, userRequest.UserLongitude);

//        var centers = await _context.MaintenanceCenters.Include(c => c.Workers).ToListAsync();
//        var closestCenters = await _distanceService.GetClosestCentersAsync(userLocation, centers);

//        foreach (var item in closestCenters)
//        {
//            var distanceInfo = new DistanceInfo
//            {
//                UserRequestId = userRequest.Id,
//                CenterId = item.Center.CenterId,
//                Distance = item.Distance
//            };
//            _context.DistanceInfos.Add(distanceInfo);
//        }

//        await _context.SaveChangesAsync();

//        foreach (var item in closestCenters)
//        {
//            var requestPayload = new
//            {
//                UserRequestId = userRequest.Id,
//                UserPhone = userRequest.UserPhone,
//                ProblemDescription = userRequest.ProblemDescription,
//                CarName = userRequest.CarName,
//                CarModel = userRequest.CarModel,
//                UserLatitude = userRequest.UserLatitude,
//                UserLongitude = userRequest.UserLongitude,
//                Distance = item.Distance,
//                Status = userRequest.IsActive ? "Active" : "Not Active"
//            };

//            var jsonPayload = JsonSerializer.Serialize(requestPayload);
//            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

//            var centerApiUrl = $"http://center-api-url/{item.Center.CenterId}/receive-request";
//            await _httpClient.PostAsync(centerApiUrl, content);
//        }

//        return Ok(userRequest);
//    }

//    [HttpPost("{requestId}/accept")]
//    public async Task<IActionResult> AcceptRequest(int requestId, [FromForm] int centerId)
//    {
//        var request = await _context.UserRequests.FindAsync(requestId);
//        if (request == null || !request.IsActive)
//        {
//            return BadRequest("Request not found or not active.");
//        }

//        request.IsActive = false;
//        await _context.SaveChangesAsync();

//        var center = await _context.MaintenanceCenters.Include(c => c.Workers).FirstOrDefaultAsync(c => c.CenterId == centerId);
//        if (center == null)
//        {
//            return NotFound("Center not found.");
//        }

//        var worker = center.Workers.FirstOrDefault();

//        var acceptedRequest = new AcceptedRequest
//        {
//            UserRequestId = request.Id,
//            CenterId = center.CenterId,
//            CenterName = center.CenterName,
//            CenterPhone = center.PhoneNumber,
//            WorkerName = worker?.Name,
//            WorkerPhone = worker?.Phone,
//            Distance = await _distanceService.CalculateDistance(new UserLocation(request.UserLatitude, request.UserLongitude), center.CenterLocation),
//            Status = "Accepted"
//        };
//        _context.AcceptedRequests.Add(acceptedRequest);
//        await _context.SaveChangesAsync();

//        return Ok(acceptedRequest);
//    }

//    [HttpPost("{requestId}/reject")]
//    public async Task<IActionResult> RejectRequest(int requestId, [FromForm] int centerId)
//    {
//        var request = await _context.UserRequests.FindAsync(requestId);
//        if (request == null || !request.IsActive)
//        {
//            return BadRequest("Request not found or not active.");
//        }

//        return Ok("Request rejected.");
//    }

//    [HttpGet("user/{userPhone}")]
//    public async Task<IActionResult> GetUserRequests(string userPhone)
//    {
//        var requests = await _context.UserRequests
//            .Where(r => r.UserPhone == userPhone)
//            .Include(r => r.AcceptedRequest)
//            .ToListAsync();

//        return Ok(requests);
//    }
//}
