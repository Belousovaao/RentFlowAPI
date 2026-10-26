import api from '@/shared/api/axios';
import type {CustomerDetailsDto, CreateCustomerRequest, CustomerDocumentDto, CustomerLookup, CustomerType, DocumentType, UpdateCustomerRequest, CustomerShortDto } from '@/entities/customer/customer.type';

export const customerApi = {
    search: async (type: CustomerType, query: string): Promise<CustomerLookup[]> => {
        const { data } = await api.get<{ data: CustomerShortDto[] }>(
            '/admin/customers',
            {
                params: {
                    page: 1,
                    pageSize: 20,
                    type,
                    search: query,
                },
            }
        );

        return data.data.map(customer => ({
            id: customer.id,
            type: customer.type,
            displayName: customer.displayName ?? '',
            phone: customer.phone ?? '',
            email: customer.email ?? undefined,
            verificationStatus: customer.verificationStatus,
        }));
    },
    
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