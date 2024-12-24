namespace PatientRecordService.Commands
{
    public class UpdatePatientCommand
    {

        public string PatientId { get; set; } // Unique patient identifier
        public string Name { get; set; } // Patient name
        public int Age { get; set; } // Patient age
        public string Gender { get; set; } // Patient gender
        public string ContactNumber { get; set; } // Contact number
        public string Email { get; set; } // Email

        public string MedicalConditionId { get; set; } // ID of the medical condition

        public string MedicalCondition { get; set; } // Description of the medical condition
        public string MedicalHistory { get; set; } // Medical history
        public string Prescription { get; set; } // Prescription
        public string LabResult { get; set; } // Lab result
        public string AppointmentDate { get; set; } // Appointment date
    }
}
