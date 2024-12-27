using FluentValidation;
using AppointmentSchedulingService.Commands;

namespace AppointmentSchedulingService.Validations
{
    

    public class CancelAppointmentValidator : AbstractValidator<CancelAppointmentCommand>
    {
        public CancelAppointmentValidator()
        {
            RuleFor(x => x.AppointmentId)
                .NotEmpty().WithMessage("Appointment ID is required.")
                .Length(1, 50).WithMessage("Appointment ID must be between 1 and 50 characters.");
        }
    }

}
