using AppointmentSchedulingService.Commands;
using AppointmentSchedulingService.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

public class UpdateAppointmentHandler
{
    private readonly IMongoCollection<Appointment> _appointments;

    public UpdateAppointmentHandler(IMongoDatabase database, IConfiguration config)
    {
        // Get the collection name from configuration
        var collectionName = config.GetSection("DatabaseSettings:CollectionName").Value;
        _appointments = database.GetCollection<Appointment>(collectionName);
    }

    public async Task Handle(UpdateAppointmentCommand command)
    {
        var filter = Builders<Appointment>.Filter.Eq(a => a.AppointmentId, command.AppointmentId);
        var update = Builders<Appointment>.Update.Set(a => a.AppointmentDate, command.NewAppointmentDate);

        await _appointments.UpdateOneAsync(filter, update);
    }
}
