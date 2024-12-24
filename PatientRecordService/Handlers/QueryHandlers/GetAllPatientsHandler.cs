using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppointmentSchedulingService.Models;
using PatientRecordService.Models;
using PatientRecordService.Queries;

public class GetAllPatientsHandler
{
    private readonly IMongoCollection<Patient> _patients;

    public GetAllPatientsHandler(IMongoDatabase database, IConfiguration config)
    {
        var collectionName = config.GetSection("DatabaseSettings:CollectionName").Value;
        _patients = database.GetCollection<Patient>(collectionName);
    }

    public async Task<List<Patient>> Handle(GetAllPatientsQuery query)
    {
        return await _patients.Find(_ => true).ToListAsync();
    }
}
