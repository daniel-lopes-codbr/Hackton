using AgroSolutions.Application.Commands.Ingestion;
using FluentValidation;

namespace AgroSolutions.Application.Validators.Commands.Ingestion;

/// <summary>
/// Validator for IngestSingleCommand
/// </summary>
public class IngestSingleCommandValidator : AbstractValidator<IngestSingleCommand>
{
    public IngestSingleCommandValidator()
    {
        RuleFor(x => x.Reading)
            .NotNull().WithMessage("Reading is required");

        RuleFor(x => x.Reading.FieldId)
            .NotEmpty().WithMessage("Field ID is required")
            .When(x => x.Reading != null);

        RuleFor(x => x.Reading.SensorType)
            .NotEmpty().WithMessage("Sensor type is required")
            .MaximumLength(50).WithMessage("Sensor type must not exceed 50 characters")
            .When(x => x.Reading != null);

        RuleFor(x => x.Reading.Value)
            .NotNull().WithMessage("Value is required")
            .When(x => x.Reading != null);

        RuleFor(x => x.Reading.Unit)
            .NotEmpty().WithMessage("Unit is required")
            .MaximumLength(20).WithMessage("Unit must not exceed 20 characters")
            .When(x => x.Reading != null);

        RuleFor(x => x.Reading.ReadingTimestamp)
            .NotEmpty().WithMessage("Reading timestamp is required")
            .When(x => x.Reading != null);

        RuleFor(x => x.Reading.Location)
            .MaximumLength(200).WithMessage("Location must not exceed 200 characters")
            .When(x => x.Reading != null && !string.IsNullOrWhiteSpace(x.Reading.Location));
    }
}
