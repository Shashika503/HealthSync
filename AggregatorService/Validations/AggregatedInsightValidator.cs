using FluentValidation;
using AggregatorService.Models;

namespace AggregatorService.Validations
{
    

    public class AggregatedInsightValidator : AbstractValidator<AggregatedInsight>
    {
        public AggregatedInsightValidator()
        {
            RuleFor(x => x.DoctorInsights)
                .NotNull().WithMessage("Doctor insights cannot be null.")
                .Must(x => x.Length > 0).WithMessage("Doctor insights must contain at least one item.")
                .ForEach(insight => insight.SetValidator(new DoctorInsightValidator()));

            RuleFor(x => x.AppointmentFrequency)
                .NotNull().WithMessage("Appointment frequency cannot be null.")
                .Must(x => x.Length > 0).WithMessage("Appointment frequency must contain at least one item.")
                .ForEach(freq => freq.SetValidator(new AppointmentFrequencyValidator()));

            RuleFor(x => x.ConditionsBySpecialty)
                .NotNull().WithMessage("Conditions by specialty cannot be null.")
                .Must(x => x.Length > 0).WithMessage("Conditions by specialty must contain at least one item.")
                .ForEach(condition => condition.SetValidator(new SpecialtyInsightValidator()));
        }
    }

    public class DoctorInsightValidator : AbstractValidator<DoctorInsight>
    {
        public DoctorInsightValidator()
        {
            RuleFor(x => x.DoctorId)
                .NotEmpty().WithMessage("Doctor ID is required.")
                .Length(1, 50).WithMessage("Doctor ID must be between 1 and 50 characters.");

            RuleFor(x => x.DoctorName)
                .NotEmpty().WithMessage("Doctor name is required.")
                .Length(1, 100).WithMessage("Doctor name must be between 1 and 100 characters.");

            RuleFor(x => x.AppointmentCount)
                .GreaterThanOrEqualTo(0).WithMessage("Appointment count must be zero or greater.");
        }
    }

    public class AppointmentFrequencyValidator : AbstractValidator<AppointmentFrequency>
    {
        public AppointmentFrequencyValidator()
        {
            RuleFor(x => x.Period)
                .NotEmpty().WithMessage("Period is required.")
                .Matches(@"^\d{4}-(Q[1-4]|W\d{2}|M\d{2})$").WithMessage("Period must follow the format 'YYYY-Qx', 'YYYY-Wxx', or 'YYYY-Mxx'.");

            RuleFor(x => x.AppointmentCount)
                .GreaterThanOrEqualTo(0).WithMessage("Appointment count must be zero or greater.");
        }
    }

    public class SpecialtyInsightValidator : AbstractValidator<SpecialtyInsight>
    {
        public SpecialtyInsightValidator()
        {
            RuleFor(x => x.Specialty)
                .NotEmpty().WithMessage("Specialty is required.")
                .Length(1, 100).WithMessage("Specialty must be between 1 and 100 characters.");

            RuleFor(x => x.CommonConditions)
                .NotNull().WithMessage("Common conditions cannot be null.")
                .Must(x => x.Length > 0).WithMessage("Common conditions must contain at least one item.")
                .ForEach(condition =>
                    condition.NotEmpty().WithMessage("Condition must not be empty.")
                    .Length(1, 200).WithMessage("Condition must be between 1 and 200 characters.")
                );
        }
    }

}
