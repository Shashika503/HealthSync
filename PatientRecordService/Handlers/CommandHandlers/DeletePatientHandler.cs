namespace PatientRecordService.Handlers.CommandHandlers
{
    using MongoDB.Driver;
    using PatientRecordService.Commands;
    using PatientRecordService.Models;
    using System.Threading.Tasks;

    public class DeletePatientHandler
    {
        private readonly IMongoCollection<Patient> _patients;

        public DeletePatientHandler(IMongoDatabase database, IConfiguration config)
        {
            // Get the collection name from configuration
            var collectionName = config.GetSection("DatabaseSettings:CollectionName").Value;
            _patients = database.GetCollection<Patient>(collectionName);
        }
        public async Task Handle(DeletePatientCommand command)
        {
            var filter = Builders<Patient>.Filter.Eq(p => p.PatientId, command.PatientId);
            await _patients.DeleteOneAsync(filter);
        }
    }

}
