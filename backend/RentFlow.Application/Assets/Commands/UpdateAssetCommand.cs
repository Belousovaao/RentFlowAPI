using MediatR;
using RentFlow.Domain.Assets;

namespace RentFlow.Application.Assets.Commands;
public class UpdateAssetCommand : IRequest<Asset>
{
    public Guid Id { get; set; }
    public string BrandName { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public decimal DailyPrice { get; set; }
    public string FuelType { get; set; }
    public string Transmission { get; set; }
    public string DriveType { get; set; }
    public int Seats { get; set; }
    public int Horsepower { get; set; }
    public string Status { get; set; }
}