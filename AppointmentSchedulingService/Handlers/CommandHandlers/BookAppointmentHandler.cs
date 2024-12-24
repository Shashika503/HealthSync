namespace AppointmentSchedulingService.Handlers.CommandHandlers
{
    using MongoDB.Driver;
    using System.Threading.Tasks;
    using AppointmentSchedulingService.Models;
    using AppointmentSchedulingService.Commands;
    using System.Net.Http;

    public class BookAppointmentHandler
    {
        private readonly IMongoCollection<Appointment> _appointments;
        private readonly HttpClient _httpClient;

        public BookAppointmentHandler(IMongoDatabase database, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            // Get the collection name from configuration
            var collectionName = config.GetSection("DatabaseSettings:CollectionName").Value;
            _appointments = database.GetCollection<Appointment>(collectionName);
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task Handle(BookAppointmentCommand command)
        {
            var appointment = new Appointment
            {
                AppointmentId = command.AppointmentId,
                PatientId = command.PatientId,
                PatientEmail = command.PatientEmail,
                DoctorId = command.DoctorId,
                DoctorName = command.DoctorName,
                Specialty = command.Specialty,
                AppointmentDate = command.AppointmentDate,
                Reason = command.Reason,
                Status = command.Status
            };

            await _appointments.InsertOneAsync(appointment);

            // Schedule a notification
            await ScheduleNotification(command);
        }

        private async Task<bool> ScheduleNotification(BookAppointmentCommand command)
        {
            var notificationCommand = new
            {
                AppointmentId = command.AppointmentId,
                PatientId = command.PatientId,
                PatientEmail = command.PatientEmail,
                NotificationType = "Reminder",
                ScheduledTime = command.AppointmentDate.AddDays(-1) // Send reminder one day before
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://localhost:7141/api/notifications/sendReminder", notificationCommand);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Notification scheduled successfully.");
                    return true;
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to schedule notification. HTTP {response.StatusCode}: {errorMessage}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scheduling notification: {ex.Message}");
                return false;
            }
        }

    }
}
