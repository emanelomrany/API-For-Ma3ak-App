namespace Ma3ak.Dtos
{
    public class CarDetailsDto
    {
        public int CarId { get; set; }

        public string CarName { get; set; }
        public int Model { get; set; }
        public string UserId { get; set; }
       
        public bool isDeleted { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
    }
}
