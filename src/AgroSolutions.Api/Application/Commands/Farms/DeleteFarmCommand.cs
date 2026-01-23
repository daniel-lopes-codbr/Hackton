using AgroSolutions.Api.Application.Common.Results;
using MediatR;

namespace AgroSolutions.Api.Application.Commands.Farms;

/// <summary>
/// Command to delete a farm
/// </summary>
public class DeleteFarmCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
