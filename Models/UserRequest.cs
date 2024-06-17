using System.ComponentModel.DataAnnotations.Schema;

namespace Ma3ak.Models
{
    public class UserRequest
    {
        public int Id { get; set; }
        public string UserPhone { get; set; }
        public string ProblemDescription { get; set; }
        public string CarName { get; set; }
        public string CarModel { get; set; }
 
        public bool IsActive { get; set; } = true;

      
    }
}
