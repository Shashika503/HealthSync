namespace AggregatorService.Models
{
    public class AggregatedRow
    {
        public string Id { get; set; }
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int? AppointmentCount { get; set; }
        public string Period { get; set; }
        public string Specialty { get; set; }
        public string CommonConditions { get; set; }
    }
}
