import { useQuery } from '@tanstack/react-query';
import { searchAssets, getAssetById, getPopularAssets } from './asset.api';
import { AssetFilters, PaginatedResponse, Asset } from './asset.types';

export const useAssets = (filters?: AssetFilters) => {
    return useQuery<PaginatedResponse<Asset>>({
        queryKey: ['assets', filters],
        queryFn: () => searchAssets(filters),
        staleTime: 5 * 60 * 1000,
    });
};

export const useAsset = (id: string) => {
    return useQuery({
        queryKey: ['asset', id],
        queryFn: () => getAssetById(id),
        enabled: !!id,
        staleTime: 5 * 60 * 1000,
    });
};

export const usePopularAssets = (limit: number = 6) => {
  return useQuery({
      queryKey: ['assets', 'popular', limit],
      queryFn: () => getPopularAssets(limit),
      staleTime: 5 * 60 * 1000,
  });
};