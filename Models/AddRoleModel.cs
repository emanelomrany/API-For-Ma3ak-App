namespace Ma3ak.Models
{
    public class AddRoleModel
    {

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
