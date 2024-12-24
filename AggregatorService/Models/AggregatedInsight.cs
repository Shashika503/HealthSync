using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AggregatorService.Models
{
    public class AggregatedInsight
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("doctor_insights")]
        public DoctorInsight[] DoctorInsights { get; set; }

        [BsonElement("appointment_frequency")]
        public AppointmentFrequency[] AppointmentFrequency { get; set; }

        [BsonElement("conditions_by_specialty")]
        public SpecialtyInsight[] ConditionsBySpecialty { get; set; }
    }

    public class DoctorInsight
    {
        public string DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int AppointmentCount { get; set; }
    }

    public class AppointmentFrequency
    {
        public string Period { get; set; } // e.g., "2024-Q1", "2024-W05"
        public int AppointmentCount { get; set; }
    }

    public class SpecialtyInsight
    {
        public string Specialty { get; set; }
        public string[] CommonConditions { get; set; }
    }
}
