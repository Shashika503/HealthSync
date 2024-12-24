namespace AppointmentSchedulingService.Handlers.QueryHandlers
{
    using MongoDB.Driver;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AppointmentSchedulingService.Models;

    public class GetAppointmentsByDateHandler
    {
        private readonly IMongoCollection<Appointment> _appointments;

        public GetAppointmentsByDateHandler(IMongoDatabase database, IConfiguration config)
        {
            // Get the collection name from configuration
            var collectionName = config.GetSection("DatabaseSettings:CollectionName").Value;
            _appointments = database.GetCollection<Appointment>(collectionName);
        }

        public async Task<List<Appointment>> Handle(GetAppointmentsByDateQuery query)
        {
            return await _appointments.Find(a => a.AppointmentDate.Date == query.AppointmentDate.Date).ToListAsync();
        }
    }

}
