namespace Ma3ak.Dtos
{
    public class CreateMaintenanceRequestDtocs
    {

        [MaxLength(50)]
        public string RequestName { get; set; }

        public int UserId { get; set; }
        
        [MaxLength(50)]
        public string CarName { get; set; }

        public int CarModel { get; set; }
        public int ServiceId { get; set; }
        

        public int MaintenanceCenterId { get; set; }

        [MaxLength(255)]
        public string UsercurrentLocation { get; set; }
        [MaxLength(100)]
        public string Distance { get; set; }

        public string ProblemDescription { get; set; }
        [MaxLength(50)]
        public DateTime RequestDate { get; set; }
        [MaxLength(50)]
        public DateTime RequestTime { get; set; }
        [MaxLength(50)]
        public string RequestStatus { get; set; }
        [MaxLength(50)]
        public string RequestActivity { get; set; }
        public IFormFile? Map { get; set; }
        public bool isDeleted { get; set; }
    }
}
