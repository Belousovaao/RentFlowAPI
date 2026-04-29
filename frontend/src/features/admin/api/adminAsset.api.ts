import api from '@/shared/api/axios';
import { Asset, AssetStatus, DriveType, FuelType, TransmissionType } from '@/entities/asset/asset.types';

export const adminAssetsApi = {
  // Получить список всех автомобилей (включая недоступные)
  getList: (params: { page: number; pageSize: number; search?: string; showDisabled?: boolean }) =>
    api.get<{ items: Asset[]; totalCount: number }>('/admin/assets', { params }),
  
  // Получить автомобиль по ID
  getById: (id: string) => api.get<Asset>(`/admin/assets/${id}`),
  
  // Создать автомобиль
  create: (data: {
    brandName: string;
    model: string;
    year: number;
    dailyPrice: number;
    fuelType: FuelType;
    transmission: TransmissionType;
    driveType: DriveType;
    seats: number;
    horsepower: number;
    status: AssetStatus;
  }) => api.post<Asset>('/admin/assets', data),
  
  // Обновить автомобиль
  update: (id: string, data: Partial<Asset>) => api.put<Asset>(`/admin/assets/${id}`, data),
  
  // Отключить автомобиль
  disable: (id: string) => api.patch(`/admin/assets/${id}/disable`),
  
  // Включить автомобиль
  enable: (id: string) => api.patch(`/admin/assets/${id}/enable`),
  
  // Изменить статус
  updateStatus: (id: string, status: AssetStatus) => api.patch(`/admin/assets/${id}/status`, { status }),

  // Удалить фото
  deletePhoto: (assetId: string, photoId: string) => api.delete(`/admin/assets/${assetId}/photos/${photoId}`),
};