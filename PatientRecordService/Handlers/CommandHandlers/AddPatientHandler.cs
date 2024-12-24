namespace PatientRecordService.Handlers.CommandHandlers
{
    using MongoDB.Driver;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using PatientRecordService.Models;
    using PatientRecordService.Commands;

    public class AddPatientHandler
    {
        private readonly IMongoCollection<Patient> _patients;

        public AddPatientHandler(IMongoDatabase database, IConfiguration config)
        {
            // Get the collection name from configuration
            var collectionName = config.GetSection("DatabaseSettings:CollectionName").Value;
            _patients = database.GetCollection<Patient>(collectionName);
        }

        public async Task Handle(AddPatientCommand command)
        {
            // Map command fields to Patient model
            var patient = new Patient
            {
                PatientId = command.PatientId,
                Name = command.Name,
                Age = command.Age,
                Gender = command.Gender,
                ContactNumber = command.ContactNumber,
                Email = command.Email,
                MedicalConditionId = command.MedicalConditionId,
                MedicalCondition = command.MedicalCondition,
                MedicalHistory = command.MedicalHistory,
                Prescription = command.Prescription,
                LabResult = command.LabResult,
                AppointmentDate = command.AppointmentDate
            };

            // Insert the patient record into MongoDB
            await _patients.InsertOneAsync(patient);
        }
    }
}
