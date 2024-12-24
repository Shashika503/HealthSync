namespace AppointmentSchedulingService.Commands
{
    public class BookAppointmentCommand
    {
        public string AppointmentId { get; set; }
        public string PatientId { get; set; }
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Specialty { get; set; }

        public string PatientEmail { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; } = "Scheduled";
    }


}
