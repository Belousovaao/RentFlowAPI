import api from '@/shared/api/axios';
import type {CustomerDetailsDto, CreateCustomerRequest, CustomerDocumentDto, CustomerLookup, CustomerType, DocumentType, UpdateCustomerRequest } from '@/entities/customer/customer.type';

export const customerApi = {
    search: (type: CustomerType, query: string) => api.get<CustomerLookup[]>(
            '/customer/search',
            {
                params: {
                    type,
                    q: query,
                },
            }
        ),
    
    create: (data: CreateCustomerRequest) => api.post<CustomerLookup>('/customer', data),

    getById: (id: string) => api.get<CustomerDetailsDto>(`/customer/${id}`),

    getMe: () =>api.get<CustomerDetailsDto>('/customer/me'),

    update: (data: UpdateCustomerRequest) => api.put<CustomerDetailsDto>(
            `/customer/me`,
            data
        ),

    uploadDocuments: (type: DocumentType, file: File) => {
        const formData = new FormData();
        formData.append('type', type.toString());
        formData.append('file', file);

        return api.post(`/booking/documents`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data',
            },
        });
    },

    getDocuments: async (customerId: string): Promise<CustomerDocumentDto[]> =>
    {
        const { data } = await api.get<CustomerDocumentDto[]>(
            `/customers/${customerId}/documents`
        );
        return data;
    }
};