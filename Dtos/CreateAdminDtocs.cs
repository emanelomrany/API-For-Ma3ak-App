namespace Ma3ak.Dtos
{
    public class CreateAdminDtocs
    {
        [MaxLength(100)]
        public string AdminFirstName { get; set; }


        [MaxLength(100)]
        public string AdminLastName { get; set; }

        [MaxLength(20)]

        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        [MaxLength(10)]
        public string Password { get; set; }
        [MaxLength(14)]
        public string NationalID { get; set; }
        public string Role { get; set; }
        public IFormFile? AdminPhoto { get; set; }
        public bool isDeleted { get; set; }

    }
}
