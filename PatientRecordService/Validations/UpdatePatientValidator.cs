using FluentValidation;
using PatientRecordService.Commands;

namespace PatientRecordService.Validations
{
    public class UpdatePatientValidator : AbstractValidator<UpdatePatientCommand>
    {
        public UpdatePatientValidator()
        {
            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("Patient ID is required.")
                .Length(1, 50).WithMessage("Patient ID must be between 1 and 50 characters.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Length(1, 100).WithMessage("Name must be between 1 and 100 characters.");

            RuleFor(x => x.Age)
                .InclusiveBetween(0, 120).WithMessage("Age must be between 0 and 120.");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .Must(x => x == "Male" || x == "Female" || x == "Other")
                .WithMessage("Gender must be Male, Female, or Other.");

            RuleFor(x => x.ContactNumber)
                .NotEmpty().WithMessage("Contact Number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Contact Number must be a valid phone number.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email address.");

            RuleFor(x => x.MedicalHistory)
                .MaximumLength(1000).WithMessage("Medical History cannot exceed 1000 characters.");
        }
    }


}
