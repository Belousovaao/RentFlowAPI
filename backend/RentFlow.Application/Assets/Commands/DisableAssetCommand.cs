using MediatR;

namespace RentFlow.Application.Assets.Commands;
public class DisableAssetCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    
    public DisableAssetCommand(Guid id)
    {
        Id = id;
    }
}