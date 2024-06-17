namespace Ma3ak.Models
{
    public class AuthModel
    {
       public string Id { get; set; }
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        //[Required]
        //[EmailAddress]
        
        public string Email { get; set; }


     
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public string NationalID { get; set; }
 
        public string Gender { get; set; }

        public string? Poster { get; set; }
        public bool isDeleted { get; set; }
        public List<string> Roles {  get; set; } 
        public string Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        


    }
}
