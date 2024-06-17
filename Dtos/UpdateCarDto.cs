namespace Ma3ak.Dtos
{
    public class UpdateCarDto
    {
        public int CarId { get; set; }
        [MaxLength(100)]
        public string CarName { get; set; }
        public int Model { get; set; }

        public string UserName { get; set; }
        //public bool isDeleted { get; set; }
    }
}
