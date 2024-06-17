namespace Ma3ak.Dtos
{
    public class CreateServiceDtocs
    {
        [MaxLength(50)]
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public IFormFile? ServicePoster { get; set; }
        public IFormFile? ServiceLogo { get; set; }
        public bool isDeleted { get; set; }
    }
}
