namespace Ma3ak.Dtos
{
    public class CreateMaintenanceCenterDtocs
    {
        [MaxLength(100)]
        public string CenterName { get; set; }
        [MaxLength(255)]
        public string CenterLocation { get; set; }
        public double CenterRate { get; set; }
        
        [MaxLength(20)]
        public string PhoneNumber { get; set; }
      
        [MaxLength(50)]
        public string WorkingHours { get; set; }

        public string Email { get; set; }

        [MaxLength(10)]
        public string Password { get; set; }
        [MaxLength(14)]
        public string CenterNationalID { get; set; }

        public IFormFile? CentersPoster { get; set; }
        // تمثيل الإحداثيات الجغرافية
        public decimal Latitude { get; set; } // خاصية العرض

        public decimal Longitude { get; set; } // خاصية الطول
        public bool isDeleted { get; set; }

    }
}
