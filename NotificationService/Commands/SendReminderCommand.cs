using System;

namespace NotificationService.Commands
{
    public class SendReminderCommand
    {
        public string AppointmentId { get; set; }
        public string PatientId { get; set; }
        public string PatientEmail { get; set; }
        public string NotificationType { get; set; } // Reminder or Follow-Up
        public DateTime ScheduledTime { get; set; }
    }
}
