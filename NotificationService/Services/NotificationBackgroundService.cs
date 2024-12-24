using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotificationService.Models;
using NotificationService.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class NotificationBackgroundService : IHostedService
{
    private readonly IMongoClient _mongoClient;
    private readonly NotificationSender _notificationSender;
    private readonly string _databaseName;
    private readonly string _collectionName;

    public NotificationBackgroundService(IMongoClient mongoClient, IOptions<MongoDbSettings> mongoDbSettings, NotificationSender notificationSender, IConfiguration config)
    {
        _mongoClient = mongoClient;
        _notificationSender = notificationSender;
        _databaseName = mongoDbSettings.Value.DatabaseName;
        _collectionName = config.GetSection("DatabaseSettings:CollectionName").Value;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(() => ProcessNotificationsAsync(cancellationToken), cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task ProcessNotificationsAsync(CancellationToken cancellationToken)
    {
        var database = _mongoClient.GetDatabase(_databaseName);
        var notificationsCollection = database.GetCollection<Notification>(_collectionName);

        while (!cancellationToken.IsCancellationRequested)
        {
            // Fetch pending notifications scheduled within the next 24 hours
            var now = DateTime.UtcNow;
            var filter = Builders<Notification>.Filter.And(
                Builders<Notification>.Filter.Eq(n => n.Status, "Pending"),
                Builders<Notification>.Filter.Lte(n => n.ScheduledTime, now.AddHours(24))
            );

            var pendingNotifications = await notificationsCollection.Find(filter).ToListAsync(cancellationToken);

            foreach (var notification in pendingNotifications)
            {
                try
                {
                    // Send the email
                    string subject = $"{notification.NotificationType} for Appointment";
                    string body = $"Dear Patient,\n\nThis is a {notification.NotificationType} for your upcoming appointment scheduled at {notification.ScheduledTime}.\n\nThank you.";
                    await _notificationSender.SendEmailAsync(notification.PatientEmail, subject, body);

                    // Mark as sent
                    var updateFilter = Builders<Notification>.Filter.Eq(n => n.Id, notification.Id);
                    var update = Builders<Notification>.Update.Set(n => n.Status, "Sent");

                    await notificationsCollection.UpdateOneAsync(updateFilter, update, null, cancellationToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send notification: {ex.Message}");
                }
            }

            // Wait for a short period before checking again
            await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);
        }
    }
}
