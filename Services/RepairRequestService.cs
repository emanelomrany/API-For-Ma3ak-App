//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Ma3ak.Models;
//using Microsoft.EntityFrameworkCore;

//namespace Ma3ak.Services
//{
//    public class RepairRequestService : IRepairRequestService
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly IDistanceService _distanceService;

//        public RepairRequestService(ApplicationDbContext context, IDistanceService distanceService)
//        {
//            _context = context;
//            _distanceService = distanceService;
//        }

//        public async Task ProcessUserRequest(UserRequest userRequest)
//        {
//             جلب جميع مراكز الصيانة من قاعدة البيانات
//            var repairCenters = await _context.MaintenanceCenters.ToListAsync();

//             حساب المسافات باستخدام خدمة المسافات
//            var userLocation = $"{userRequest.UserLatitude},{userRequest.UserLongitude}";
//            var closestCenters = await _distanceService.GetClosestCentersAsync(userLocation, repairCenters);

//             إنشاء طلبات الصيانة
//            foreach (var item in closestCenters)
//            {
//                var distanceInfo = new DistanceInfo
//                {
//                    UserRequestId = userRequest.Id,
//                    CenterId = item.Center.CenterId,
//                    Distance = item.Distance
//                };
//                _context.DistanceInfos.Add(distanceInfo);
//            }

//            await _context.SaveChangesAsync();
//        }
//    }
//}
