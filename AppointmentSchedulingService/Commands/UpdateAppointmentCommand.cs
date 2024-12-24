namespace AppointmentSchedulingService.Commands
{
    public class UpdateAppointmentCommand
    {
        public string AppointmentId { get; set; }
        public DateTime NewAppointmentDate { get; set; }
    }


}
