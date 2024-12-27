using FluentValidation;
using AppointmentSchedulingService.Commands;

namespace AppointmentSchedulingService.Validations
{
  

    public class UpdateAppointmentValidator : AbstractValidator<UpdateAppointmentCommand>
    {
        public UpdateAppointmentValidator()
        {
            RuleFor(x => x.AppointmentId)
                .NotEmpty().WithMessage("Appointment ID is required.")
                .Length(1, 50).WithMessage("Appointment ID must be between 1 and 50 characters.");

            RuleFor(x => x.NewAppointmentDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("New appointment date must be in the future.");
        }
    }

}
