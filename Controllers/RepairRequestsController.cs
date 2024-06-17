//using System.Linq;
//using System.Threading.Tasks;
//using Ma3ak.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace Ma3ak.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class RepairRequestsController : ControllerBase
//    {
//        private readonly ApplicationDbContext _context;

//        public RepairRequestsController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        [HttpPost("{id}/response")]
//        public async Task<IActionResult> RespondToRepairRequest(int id, [FromBody] bool isAccepted)
//        {
//            var distanceInfo = await _context.DistanceInfos
//                .Include(d => d.UserRequest)
//                .Include(d => d.Center)
//                .FirstOrDefaultAsync(d => d.Id == id);

//            if (distanceInfo == null)
//            {
//                return NotFound();
//            }

//            var acceptedRequest = new AcceptedRequest
//            {
//                UserRequestId = distanceInfo.UserRequestId,
//                CenterId = distanceInfo.CenterId,
//                CenterName = distanceInfo.Center.CenterName,
//                CenterPhone = distanceInfo.Center.PhoneNumber,
//                WorkerName = distanceInfo.Center.Workers.FirstOrDefault()?.Name,
//                WorkerPhone = distanceInfo.Center.Workers.FirstOrDefault()?.Phone,
//                Distance = distanceInfo.Distance,
//                Status = isAccepted ? "Accepted" : "Rejected"
//            };

//            _context.AcceptedRequests.Add(acceptedRequest);
//            await _context.SaveChangesAsync();

//            if (isAccepted)
//            {
//                // إرسال إشعار إلى اليوزر
//                NotifyUser(distanceInfo.UserRequest, distanceInfo.Center, distanceInfo.Distance);
//            }

//            return Ok(acceptedRequest);
//        }

//        private void NotifyUser(UserRequest userRequest, MaintenanceCenter repairCenter, double distance)
//        {
//            // هنا يمكن استخدام خدمة مثل SignalR لإرسال إشعار إلى تطبيق اليوزر
//            // For simplicity, we will just print to console
//            Console.WriteLine($"User {userRequest.UserPhone} notified about acceptance by {repairCenter.CenterName}.");
//        }
//    }
//}
