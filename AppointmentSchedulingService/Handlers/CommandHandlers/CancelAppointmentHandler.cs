namespace AppointmentSchedulingService.Handlers.CommandHandlers
{
    using AppointmentSchedulingService.Commands;
    using AppointmentSchedulingService.Models;
    using MongoDB.Driver;
    using System.Threading.Tasks;

    public class CancelAppointmentHandler
    {
        private readonly IMongoCollection<Appointment> _appointments;

        public CancelAppointmentHandler(IMongoDatabase database, IConfiguration config)
        {
            // Get the collection name from configuration
            var collectionName = config.GetSection("DatabaseSettings:CollectionName").Value;
            _appointments = database.GetCollection<Appointment>(collectionName);
        }

        public async Task Handle(CancelAppointmentCommand command)
        {
            var filter = Builders<Appointment>.Filter.Eq(a => a.AppointmentId, command.AppointmentId);
            var update = Builders<Appointment>.Update.Set(a => a.Status, "Cancelled");

            await _appointments.UpdateOneAsync(filter, update);
        }
    }

}
