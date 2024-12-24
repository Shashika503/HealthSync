namespace PatientRecordService.Handlers.CommandHandlers
{
    using MongoDB.Driver;
    using System.Threading.Tasks;
    using PatientRecordService.Models;
    using PatientRecordService.Commands;
    using Microsoft.Extensions.Configuration;

    public class UpdatePatientHandler
    {
        private readonly IMongoCollection<Patient> _patients;

        public UpdatePatientHandler(IMongoDatabase database, IConfiguration config)
        {
            // Get the collection name from configuration
            var collectionName = config.GetSection("DatabaseSettings:CollectionName").Value;
            _patients = database.GetCollection<Patient>(collectionName);
        }

        public async Task Handle(UpdatePatientCommand command)
        {
            // Define the filter to locate the patient by ID
            var filter = Builders<Patient>.Filter.Eq(p => p.PatientId, command.PatientId);

            // Define the updates for the specified fields
            var update = Builders<Patient>.Update
                .Set(p => p.PatientId, command.PatientId)
                .Set(p => p.Name, command.Name)
                .Set(p => p.Age, command.Age)
                .Set(p => p.Gender, command.Gender)
                .Set(p => p.ContactNumber, command.ContactNumber)
                .Set(p => p.Email, command.Email)
                .Set(p => p.MedicalConditionId , command.MedicalConditionId)
                .Set(p => p.MedicalCondition, command.MedicalCondition)
                .Set(p => p.MedicalHistory, command.MedicalHistory)
                .Set(p => p.Prescription, command.Prescription)
                .Set(p => p.LabResult, command.LabResult)
                .Set(p => p.AppointmentDate, command.AppointmentDate);

            // Execute the update operation in the MongoDB collection
            await _patients.UpdateOneAsync(filter, update);
        }
    }
}
