using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AggregatorService.Models
{
    public class Appointment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // MongoDB ObjectId (_id)

        [BsonElement("appointment_id")]
        public string AppointmentId { get; set; } // Unique appointment identifier

        [BsonElement("patient_id")]
        public string PatientId { get; set; } // ID of the patient

        [BsonElement("doctor_id")]
        public string DoctorId { get; set; } // ID of the doctor

        [BsonElement("doctor_name")]
        public string DoctorName { get; set; } // Doctor's name

        [BsonElement("specialty")]
        public string Specialty { get; set; } // Doctor's specialty

        [BsonElement("appointment_date")]
        public DateTime AppointmentDate { get; set; } // Appointment date and time

        [BsonElement("reason")]
        public string Reason { get; set; } // Reason for the appointment

        [BsonElement("status")]
        public string Status { get; set; } // Appointment status (e.g., Scheduled, Cancelled, Completed)
        [BsonElement("patient_email")]
        public string PatientEmail { get; internal set; }
    }
}
