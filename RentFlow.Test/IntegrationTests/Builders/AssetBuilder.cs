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
            dailyPrice: 6000,
            deposit: 10000,
            locationId: Guid.NewGuid(),
            canDeliver: true,
            deliveryPrice: 2000  
        );

    public Asset Build() => _factory();

}
