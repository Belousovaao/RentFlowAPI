import api from '@/shared/api/axios';
import { CreateCustomerRequest, CustomerDetailsDto, CustomerShortDto, CustomerType, UpdateCustomerRequest, VerificationStatus } from '@/entities/customer/customer.type';

export const adminCustomersApi = {
  // Получить список всех клиентов (включая чс)
  getList: (params: { page: number; pageSize: number; showRejected?: boolean; search?: string; type?: CustomerType }) => 
    api.get<{ data: CustomerShortDto[]; totalCount: number }>('/admin/customers', { params }),
  
  // Получить клиента по ID
  getById: (id: string) => api.get<CustomerDetailsDto>(`/admin/customers/${id}`),
  
  // Создать клиента
  create: (data: CreateCustomerRequest) => api.post('/admin/customers', data),
  
  // Обновить клиента
  update: (id: string, data: UpdateCustomerRequest) => api.put<CustomerDetailsDto>(`/admin/customers/${id}`, data),
  
  // Изменить статус
  updateStatus: (id: string, status: VerificationStatus) => api.patch(`/admin/customers/${id}/verificationStatus`, { status }),

  deleteDocument: (customerId: string, documentId: string) => api.delete(`/admin/customers/${customerId}/documents/${documentId}`),

};