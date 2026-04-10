using RentFlow.Domain.Assets;

namespace RentFlow.Test.IntegrationTests.Builders;

public class AssetBuilder
{
    private Func<Asset> _factory = DefaultAsset;

    private static Asset DefaultAsset()
        => Asset.Create(
            code: "BOYUE-276",
            brandName: "Geely",
            model: "Cityray",
            shortDescription: "Cерый, передний привод, АКПП, Бензин 1,5Т, 174 л.с.",
            fullDescription: "Основные характеристики:\n",
            type: AssetType.Auto,
            category: AssetCategory.Crossover,
            year: 2024,
            fuelType: FuelType.Petrol,
            transmission: TransmissionType.Robotic,
            driveType: Domain.Assets.DriveType.FrontWheelDrive,
            seats: 5,
            doors: 5,
            engine: "1.5T",
            horsepower: 174,
            acceleration: "7.9s",
            topSpeed: 190,
            color: "Серый",
            features: new List<string>
            {
                "Расход в городе - 8 л/100 км",
                "Расход по трассе - 5.7 л/100 км",
                "Расход смешанный - 6.5 л/100 км",
                "Объем топливного бака - 51 л",
                "Руль - левый"
            }, 
            dailyPrice: 6000,
            deposit: 10000,
            locationId: Guid.NewGuid(),
            canDeliver: true,
            deliveryPrice: 2000,
            latitude: null,
            longitude: null
        );

    public Asset Build() => _factory();

}
