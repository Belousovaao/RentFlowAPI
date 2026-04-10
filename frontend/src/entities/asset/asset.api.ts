import { apiClient } from "../../shared/api/client";
import type { Asset, AssetFilters, PaginatedResponse } from "./asset.types";

// Получение всех активов
export const getAssets = async (filters?: AssetFilters): Promise<Asset[]> => {
    const params = new URLSearchParams();
    if (filters) {
        Object.entries(filters).forEach(([key, value]) => {
            if (value !== undefined && value !== null && value != '') {
                params.append(key, value.toString());
            }
        });
    }
    const queryString = params.toString();
    const url = queryString ? `/api/assets?${queryString}` : "/api/assets";
    const response = await apiClient.get<PaginatedResponse<Asset>>(url);
    
    return response.data || [];
};

// Получение актива по ID
export const getAssetById = async (id: string): Promise<Asset> => {
    return apiClient.get<Asset>(`/api/assets/${id}`);
};

// Получение актива по коду
export const getAssetByCode = async (code: string): Promise<Asset> => {
    return apiClient.get<Asset>(`/api/assets/code/${code}`);
};

// Поиск активов с пагинацией
export const searchAssets = async (filters?: AssetFilters): Promise<PaginatedResponse<Asset>> => {
    const params = new URLSearchParams();
    if (filters) {
        Object.entries(filters).forEach(([key, value]) => {
            if (value !== undefined && value !== null && value != '') {
                params.append(key, value.toString());
            }
        });
    }
    const queryString = params.toString();
    const url = queryString ? `/api/assets?${queryString}` : "/api/assets";
    
    return apiClient.get<PaginatedResponse<Asset>>(url);
};

// Получение популярных активов
export const getPopularAssets = async (limit: number = 6): Promise<Asset[]> => {
    const allAssets = await getAssets();
    
    const randomAssets = [...allAssets].sort(() => 0.5 - Math.random());
    
    return randomAssets.slice(0, limit);
};


// Получение активов рядом с пользователем
export const getNearbyAssets = async (latitude: number, longitude: number, radius: number = 10): Promise<Asset[]> => {
    return apiClient.get<Asset[]>(`/api/assets/nearby?lat=${latitude}&lng=${longitude}&radius=${radius}`);
};