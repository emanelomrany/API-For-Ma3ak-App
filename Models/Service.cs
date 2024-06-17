namespace Ma3ak.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        [MaxLength(50)]
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public string? ServicePoster { get; set; }
        public string? ServiceLogo { get; set; }

        public bool isDeleted { get; set; }
    }
}
