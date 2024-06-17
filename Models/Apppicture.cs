namespace Ma3ak.Models
{
    public class Apppicture
    {
        [Key]
        public int ApppictureId { get; set; }
        [MaxLength(50)]
        public string ApppictureName { get; set; }
        public string? pictures { get; set; }
        public bool isDeleted { get; set; }
    }
}
