using MediatR;

namespace RentFlow.Application.Assets.Commands;

public class UpdateAssetStatusCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Status { get; set; }
    
    public UpdateAssetStatusCommand(Guid id, string status)
    {
        Id = id;
        Status = status;
    }
}