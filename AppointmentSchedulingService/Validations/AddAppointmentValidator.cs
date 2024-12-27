using FluentValidation;
using AppointmentSchedulingService.Models;
using AppointmentSchedulingService.Commands;

namespace AppointmentSchedulingService.Validations
{
   

    public class AddAppointmentValidator : AbstractValidator<BookAppointmentCommand>
    {
        public AddAppointmentValidator()
        {
            RuleFor(x => x.AppointmentId)
                .NotEmpty().WithMessage("Appointment ID is required.")
                .Length(1, 50).WithMessage("Appointment ID must be between 1 and 50 characters.");

            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("Patient ID is required.")
                .Length(1, 50).WithMessage("Patient ID must be between 1 and 50 characters.");

            RuleFor(x => x.DoctorId)
                .NotEmpty().WithMessage("Doctor ID is required.")
                .Length(1, 50).WithMessage("Doctor ID must be between 1 and 50 characters.");

            RuleFor(x => x.DoctorName)
                .NotEmpty().WithMessage("Doctor's name is required.")
                .Length(1, 100).WithMessage("Doctor's name must be between 1 and 100 characters.");

            RuleFor(x => x.Specialty)
                .NotEmpty().WithMessage("Doctor's specialty is required.")
                .Length(1, 100).WithMessage("Specialty must be between 1 and 100 characters.");

            RuleFor(x => x.AppointmentDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Appointment date must be in the future.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Reason for the appointment is required.")
                .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Appointment status is required.")
                .Must(x => x == "Scheduled" || x == "Cancelled" || x == "Completed")
                .WithMessage("Status must be one of the following: Scheduled, Cancelled, Completed.");

            RuleFor(x => x.PatientEmail)
                .NotEmpty().WithMessage("Patient email is required.")
                .EmailAddress().WithMessage("Patient email must be a valid email address.");
        }
    }

}
