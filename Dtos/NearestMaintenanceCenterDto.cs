namespace Ma3ak.Dtos
{
    public class NearestMaintenanceCenterDto
    {


        public string CenterName { get; set; }
        public string PhoneNumber { get; set; }
        public string? CentersPoster { get; set; }
        public string CenterLocation { get; set; }
        public double CenterRate { get; set; }
        //public string WorkerName { get; set; }
        //public string WorkerPhoneNumber { get; set; }
        public double Distance { get; set; }
        public string Message { get; set; }

        public DateTime Timestamp { get; set; }

    }
}
