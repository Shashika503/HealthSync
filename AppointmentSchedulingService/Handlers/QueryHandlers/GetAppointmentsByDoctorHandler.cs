namespace AppointmentSchedulingService.Handlers.QueryHandlers
{
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AppointmentSchedulingService.Models;
    using AppointmentSchedulingService.Queries;

    public class GetAppointmentsByDoctorHandler
    {
        private readonly IMongoCollection<Appointment> _appointments;

        public GetAppointmentsByDoctorHandler(IMongoDatabase database, IConfiguration config)
        {
            // Get the collection name from configuration
            var collectionName = config.GetSection("DatabaseSettings:CollectionName").Value;
            _appointments = database.GetCollection<Appointment>(collectionName);
        }

        public async Task<List<Appointment>> Handle(GetAppointmentsByDoctorQuery query)
        {
            return await _appointments.Find(a => a.DoctorId == query.DoctorId).ToListAsync();
        }
    }

}
