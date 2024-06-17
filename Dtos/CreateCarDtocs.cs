namespace Ma3ak.Dtos
{
    public class CreateCarDtocs
    {

        [MaxLength(100)]
        public string CarName { get; set; }
        public int Model { get; set; }
        public string UserId { get; set; }
        

        public bool isDeleted { get; set; }
    }
}
