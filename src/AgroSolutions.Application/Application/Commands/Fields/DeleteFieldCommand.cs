using AgroSolutions.Application.Common.Results;
using MediatR;

namespace AgroSolutions.Application.Commands.Fields;

/// <summary>
/// Command to delete a field
/// </summary>
public class DeleteFieldCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
