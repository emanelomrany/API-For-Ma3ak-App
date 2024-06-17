using System.ComponentModel.DataAnnotations.Schema;

namespace Ma3ak.Models
{
    public class Car
    {
        public int CarId { get; set; }
        [MaxLength(100)]
        public string CarName { get; set; }
        public int Model { get; set; }


        public string UserId { get; set; }
        public User User { get; set; }
        public bool isDeleted { get; set; }

    }
}
