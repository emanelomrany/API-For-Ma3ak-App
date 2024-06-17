namespace Ma3ak.Dtos
{
    public class CreateCarRideRequestDtocs
    {
        public int CenterId { get; set; }

        public int UserId { get; set; }

        [MaxLength(255)]
        public string When { get; set; }
        public int Numberofpassengers { get; set; }
        [MaxLength(255)]
        public string Source { get; set; }
        [MaxLength(255)]
        public string Destenation { get; set; }
        public string comment { get; set; }
        public string Ofers { get; set; }
        public bool isDeleted { get; set; }
    }
}
