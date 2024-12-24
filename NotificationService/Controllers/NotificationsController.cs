using Microsoft.AspNetCore.Mvc;
using NotificationService.Commands;
using NotificationService.Handlers.CommandHandlers;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly SendReminderHandler _sendReminderHandler;

    public NotificationsController(SendReminderHandler sendReminderHandler)
    {
        _sendReminderHandler = sendReminderHandler;
    }

    [HttpPost("sendReminder")]
    public async Task<IActionResult> SendReminder([FromBody] SendReminderCommand command)
    {
        try
        {
            // Validate the incoming request
            if (command == null)
            {
                return BadRequest("Command cannot be null.");
            }

            if (string.IsNullOrEmpty(command.AppointmentId) ||
                string.IsNullOrEmpty(command.PatientId) ||
                string.IsNullOrEmpty(command.PatientEmail))
            {
                return BadRequest("Invalid command. AppointmentId, PatientId, and PatientEmail are required.");
            }

            // Handle the command
            await _sendReminderHandler.Handle(command);

            // Return success response
            return Ok("Reminder sent successfully.");
        }
        catch (Exception ex)
        {
            // Log the error (in a real application, use a logging framework like Serilog or NLog)
            Console.WriteLine($"Error sending reminder: {ex.Message}");

            // Return error response
            return StatusCode(500, "An error occurred while sending the reminder. Please try again later.");
        }
    }
}
