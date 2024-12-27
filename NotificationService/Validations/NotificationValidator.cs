using FluentValidation;
using NotificationService.Commands;


namespace NotificationService.Validations
{
    public class NotificationValidator : AbstractValidator<SendReminderCommand>
    {
        public NotificationValidator()
        {
            RuleFor(x => x.AppointmentId)
                .NotEmpty().WithMessage("Appointment ID is required.")
                .Length(1, 50).WithMessage("Appointment ID must be between 1 and 50 characters.");

            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("Patient ID is required.")
                .Length(1, 50).WithMessage("Patient ID must be between 1 and 50 characters.");

            RuleFor(x => x.PatientEmail)
                .NotEmpty().WithMessage("Patient email is required.")
                .EmailAddress().WithMessage("Invalid email address.");

            RuleFor(x => x.NotificationType)
                .NotEmpty().WithMessage("Notification type is required.")
                .Must(x => x == "Reminder" || x == "Follow-Up")
                .WithMessage("Notification type must be either 'Reminder' or 'Follow-Up'.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Notification status is required.")
                .Must(x => x == "Sent" || x == "Pending")
                .WithMessage("Status must be either 'Sent' or 'Pending'.");

            RuleFor(x => x.ScheduledTime)
                .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Scheduled time must not be in the past.");
        }
    }
}