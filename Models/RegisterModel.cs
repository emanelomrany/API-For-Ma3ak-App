namespace Ma3ak.Models
{
    public class RegisterModel
    {
        [MaxLength(100)]
        public string FirstName { get; set; }


        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(20)]

        public string PhoneNumber { get; set; }
        [MaxLength(128)]
        //[EmailAddress]
        //[Required]
        public string Email { get; set; }

        [MaxLength(10)]
        public string Password { get; set; }
        [MaxLength(14)]
        public string NationalID { get; set; }
        [MaxLength(10)]
        public string Gender { get; set; }
        [MaxLength(100)]
        public string UserName { get; set; }

        public IFormFile? Poster { get; set; }
        public bool isDeleted { get; set; }

    }
}
