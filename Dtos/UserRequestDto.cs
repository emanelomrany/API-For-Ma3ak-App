using System.ComponentModel.DataAnnotations.Schema;

namespace Ma3ak.Dtos
{
    public class UserRequestDto
    {
        public string UserPhone { get; set; }
        public string ProblemDescription { get; set; }
        public string CarName { get; set; }
        public string CarModel { get; set; }
       

    }
}
