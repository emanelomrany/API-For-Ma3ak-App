namespace Ma3ak.Dtos
{
    public class CreateApppictureDtocs
    {
        [MaxLength(50)]
        public string ApppictureName { get; set; }
        public IFormFile? pictures { get; set; }
        public bool isDeleted { get; set; }

    }
}
