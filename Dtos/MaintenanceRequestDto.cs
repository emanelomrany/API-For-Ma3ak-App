namespace Ma3ak.Dtos
{
    public class MaintenanceRequestDto
    {
    //    public string UserId { get; set; }
    //    public string UserName { get; set; }
    //    public string UserPhone { get; set; }
    //    public string ProblemDescription { get; set; }
    //    public DateTime RequestDateTime { get; set; }
           public bool IsActive { get; set; }
  
            [Key]
           public int MaintenanceRequestId { get; set; }


            [MaxLength(50)]
            public string RequestName { get; set; }

            public string? UserId { get; set; }
           
            public string UserPhone { get; set; }
            public string UserName { get; set; }

            [MaxLength(50)]
            public string? CarName { get; set; }

            public int? CarModel { get; set; }
            public int? ServiceId { get; set; }
           

            public int? MaintenanceCenterId { get; set; }
            
          //  [MaxLength(255)]
            public string UsercurrentLocation { get; set; }
          //  [MaxLength(100)]
         //   public string Distance { get; set; }

            public string ProblemDescription { get; set; }
           // [MaxLength(50)]
            public DateTime requestDateTime { get; set; }
           
            [MaxLength(50)]
            public string? RequestStatus { get; set; }
            [MaxLength(50)]
            public string? RequestActivity { get; set; }
            
            public bool isDeleted { get; set; }

           // public decimal UserLatitude { get; set; }
           // public decimal UserLongitude { get; set; }
            //public List<NearestMaintenanceCenter> NearestMaintenanceCenters { get; set; }


    }
}


