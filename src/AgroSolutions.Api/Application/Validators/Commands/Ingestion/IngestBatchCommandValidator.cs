using AgroSolutions.Api.Application.Commands.Ingestion;
using FluentValidation;

namespace AgroSolutions.Api.Application.Validators.Commands.Ingestion;

/// <summary>
/// Validator for IngestBatchCommand
/// </summary>
public class IngestBatchCommandValidator : AbstractValidator<IngestBatchCommand>
{
    public IngestBatchCommandValidator()
    {
        RuleFor(x => x.Batch)
            .NotNull().WithMessage("Batch is required");

        RuleFor(x => x.Batch.Readings)
            .NotNull().WithMessage("Readings list is required")
            .NotEmpty().WithMessage("At least one reading is required")
            .When(x => x.Batch != null);
    }
}
