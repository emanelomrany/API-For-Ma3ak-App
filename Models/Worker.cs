namespace Ma3ak.Models
{
    public class Worker
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public int MaintenanceCenterId { get; set; }
        public MaintenanceCenter MaintenanceCenter { get; set; }
    }

}
