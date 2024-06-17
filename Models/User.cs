using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;



namespace Ma3ak.Models
{
    public class User : IdentityUser
    {

        [MaxLength(100)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        public string LastName { get; set; }
        [MaxLength(20)]
        public string PhoneNumber { get; set; }
        [MaxLength(10)]
        public string Password { get; set; }
        [MaxLength(14)]
        public string NationalID { get; set; }
        [MaxLength(10)]
        public string Gender { get; set; }
        public string? Poster { get; set; }
        public bool isDeleted { get; set; }

    }
}
