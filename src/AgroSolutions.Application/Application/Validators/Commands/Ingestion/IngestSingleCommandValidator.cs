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
        // Reading must contain either legacy single-sensor fields (SensorType + Value) OR aggregated telemetry fields
        RuleFor(x => x.Reading)
            .Must(r =>
                (!string.IsNullOrWhiteSpace(r.SensorType) && r.Value.HasValue) ||
                r.SoilMoisture.HasValue || r.AirTemperature.HasValue || r.Precipitation.HasValue || r.IsRichInPests.HasValue
            ).WithMessage("Reading must contain either SensorType+Value or at least one aggregated telemetry field (SoilMoisture/AirTemperature/Precipitation/IsRichInPests)");

        RuleFor(x => x.Reading.Location)
            .MaximumLength(200).WithMessage("Location must not exceed 200 characters")
            .When(x => x.Reading != null && !string.IsNullOrWhiteSpace(x.Reading.Location));
    }
}
