using MongoDB.Driver;
using NotificationService.Commands;
using NotificationService.Models;
using NotificationService.Services;
using System.Threading.Tasks;

namespace NotificationService.Handlers.CommandHandlers
{
    public class SendReminderHandler
    {
        private readonly IMongoCollection<Notification> _notifications;
        private readonly NotificationSender _notificationSender;

        public SendReminderHandler(IMongoDatabase database, IConfiguration config, NotificationSender notificationSender)
        {
            var collectionName = config.GetSection("DatabaseSettings:CollectionName").Value;
            _notifications = database.GetCollection<Notification>(collectionName);
            _notificationSender = notificationSender;
        }

        public async Task Handle(SendReminderCommand command)
        {
            // Create a notification document
            var notification = new Notification
            {
                AppointmentId = command.AppointmentId,
                PatientId = command.PatientId,
                PatientEmail = command.PatientEmail,
                NotificationType = command.NotificationType,
                Status = "Pending",
                ScheduledTime = command.ScheduledTime
            };

            // Save to MongoDB
            await _notifications.InsertOneAsync(notification);

            // Send the email
            string subject = $"{command.NotificationType} for Appointment";
            string body = $"Dear Patient,\n\nThis is a {command.NotificationType} for your upcoming appointment scheduled at {command.ScheduledTime}.\n\nThank you.";
            await _notificationSender.SendEmailAsync(command.PatientEmail, subject, body);

            // Update the status to "Sent"
            var filter = Builders<Notification>.Filter.Eq(n => n.Id, notification.Id);
            var update = Builders<Notification>.Update.Set(n => n.Status, "Sent");
            await _notifications.UpdateOneAsync(filter, update);
        }
    }
}
