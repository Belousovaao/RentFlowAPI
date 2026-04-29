import api from './axios';
import { Asset, AssetStatus } from '@/entities/asset/asset.types';

export const adminAssetsApi = {
  getList: (params: { page: number; pageSize: number; showDisabled?: boolean; search?: string }) =>
    api.get<{ data: Asset[]; totalCount: number }>('/admin/assets', { params }),
  
  getById: (id: string) => api.get<Asset>(`/admin/assets/${id}`),
  
  create: (data: Partial<Asset>) => api.post<Asset>('/admin/assets', data),
  
  update: (id: string, data: Partial<Asset>) => api.put<Asset>(`/admin/assets/${id}`, { ...data, id }),
  
  updateStatus: (id: string, status: AssetStatus) => api.patch(`/admin/assets/${id}/status`, { status }),
  
  disable: (id: string) => api.patch(`/admin/assets/${id}/disable`),
  
  enable: (id: string) => api.patch(`/admin/assets/${id}/enable`),
  
  deletePhoto: (assetId: string, photoId: string) => 
    api.delete(`/admin/assets/${assetId}/photos/${photoId}`),
};