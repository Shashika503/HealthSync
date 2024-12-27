using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AggregatorService.Models
{
    public class Patient
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } // MongoDB ObjectId (_id)

        [BsonElement("patient_id")]
        public string PatientId { get; set; } // Unique patient identifier

        [BsonElement("name")]
        public string Name { get; set; } // Patient name

        [BsonElement("age")]
        public int Age { get; set; } // Patient age

        [BsonElement("gender")]
        public string Gender { get; set; } // Patient gender

        [BsonElement("contact_number")]
        public string ContactNumber { get; set; } // Patient contact number

        [BsonElement("email")]
        public string Email { get; set; } // Patient email

        [BsonElement("medical_condition_id")]
        public string MedicalConditionId { get; set; } // ID of the medical condition

        [BsonElement("medical_condition")]
        public string MedicalCondition { get; set; } // Description of the medical condition

        [BsonElement("medical_history")]
        public string MedicalHistory { get; set; } // Patient medical history

        [BsonElement("prescription")]
        public string Prescription { get; set; } // Patient prescription

        [BsonElement("lab_result")]
        public string LabResult { get; set; } // Lab results

        [BsonElement("appointment_date")]
        public string AppointmentDate { get; set; } // Appointment date
    }
}
