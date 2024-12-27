using FluentValidation;
using PatientRecordService.Commands;

namespace PatientRecordService.Validations
{
    public class DeletePatientValidator : AbstractValidator<DeletePatientCommand>
    {
        public DeletePatientValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("Patient ID is required.");
        }
    }
}
