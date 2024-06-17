namespace Ma3ak.Models
{
    public class GetProfile
    {
        public string FirstName { get; set; }



        public string LastName { get; set; }



        public string PhoneNumber { get; set; }



        public string Email { get; set; }


        public string Password { get; set; }

        public string NationalID { get; set; }

        public string Gender { get; set; }

        public IFormFile? Poster { get; set; }
        public bool isDeleted { get; set; }
    }
}
