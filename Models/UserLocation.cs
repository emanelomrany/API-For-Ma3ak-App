namespace Ma3ak.Models
{
    public class UserLocation
    {
        public int Id { get; set; }     
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime Timestamp { get; set; }

        public DateTime LocalTimestamp => Timestamp.ToLocalTime();

    }
}
