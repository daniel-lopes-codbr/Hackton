using AgroSolutions.Application.Common.Results;
using MediatR;

namespace AgroSolutions.Application.Commands.Farms;

/// <summary>
/// Command to delete a farm
/// </summary>
public class DeleteFarmCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
