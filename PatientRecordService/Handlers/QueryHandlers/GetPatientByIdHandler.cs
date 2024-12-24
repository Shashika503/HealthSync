namespace PatientRecordService.Handlers.QueryHandlers
{
    using MongoDB.Driver;
    using System.Threading.Tasks;
    using PatientRecordService.Models;
    using PatientRecordService.Queries;

    public class GetPatientByIdHandler
    {
        private readonly IMongoCollection<Patient> _patients;

        public GetPatientByIdHandler(IMongoDatabase database, IConfiguration config)
        {
            // Fetch the collection name from configuration
            var collectionName = config.GetSection("DatabaseSettings:CollectionName").Value;
            _patients = database.GetCollection<Patient>(collectionName);
        }

        public async Task<Patient> Handle(GetPatientByIdQuery query)
        {
            // Fetch the patient record based on the provided query ID
            return await _patients.Find(p => p.PatientId == query.PatientId).FirstOrDefaultAsync();
        }
    }
}
