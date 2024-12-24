using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace NotificationService.Models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // MongoDB ObjectId (_id)

        [BsonElement("appointment_id")]
        public string AppointmentId { get; set; } // Related Appointment ID

        [BsonElement("patient_id")]
        public string PatientId { get; set; } // Patient ID

        [BsonElement("patient_email")]
        public string PatientEmail { get; set; } // Email of the patient

        [BsonElement("notification_type")]
        public string NotificationType { get; set; } // Reminder or Follow-Up

        [BsonElement("status")]
        public string Status { get; set; } // Status (Sent, Pending)

        [BsonElement("scheduled_time")]
        public DateTime ScheduledTime { get; set; } // Time when notification is scheduled
    }
}
