using MediatR;

namespace RentFlow.Application.Assets.Commands;
public class EnableAssetCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    
    public EnableAssetCommand(Guid id)
    {
        Id = id;
    }
}