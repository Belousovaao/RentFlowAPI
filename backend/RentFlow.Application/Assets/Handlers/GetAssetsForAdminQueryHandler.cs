using MediatR;
using RentFlow.Application.Assets.Dtos;
using RentFlow.Application.Interfaces;
using RentFlow.Domain.Assets;
using RentFlow.Application.Assets.Queries;
using RentFlow.Application.Assets.Mappers;

namespace RentFlow.Application.Assets.Handlers;
public class GetAssetsForAdminQueryHandler : IRequestHandler<GetAssetsForAdminQuery, PaginatedResponse<AssetDto>>
{
    private readonly IAssetRepository _assetRepository;
    
    public GetAssetsForAdminQueryHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }
    
    public async Task<PaginatedResponse<AssetDto>> Handle(GetAssetsForAdminQuery request, CancellationToken cancellationToken)
    {
        var filter = request.Filter;
        
        // Получаем все автомобили
        var allAssets = await _assetRepository.GetAllAsync();
        var query = allAssets.AsQueryable();
        
        // Фильтрация по поиску
        if (!string.IsNullOrEmpty(filter.Search))
        {
            query = query.Where(a => 
                a.BrandName.Contains(filter.Search) || 
                a.Model.Contains(filter.Search));
        }
        
        // Фильтрация по статусу
        if (filter.ShowDisabled == false)
        {
            // Если НЕ показывать отключенные - исключаем Disabled
            query = query.Where(a => a.Status != AssetStatus.Disabled);
        }
        
        // Пагинация
        var total = query.Count();
        var items = query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        
        var totalPages = (int)Math.Ceiling((double)total / filter.PageSize);
        
        return new PaginatedResponse<AssetDto>(
            items.Select(AssetMapper.ToDto).ToList(),
            total,
            filter.Page,
            filter.PageSize,
            totalPages,
            filter.Page < totalPages,
            filter.Page > 1
        );
    }
}