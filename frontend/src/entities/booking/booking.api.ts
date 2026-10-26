import { apiClient } from "../../shared/api/client";
import type { CreateBookingCommand, BookingCalculationResultDto, CalculateBookingPriceRequest, BookingListItemDto, AdminBookingListItemDto, AdminBookingDetailsDto, BookingStatus, ChangeBookingPeriodPreviewDto, BookingServicesChangePreviewDto, BookingServiceRequest, BookingDetailsDto} from "./booking.types";

export const bookingApi = {
    // CUSTOMER
    getMyBookings: () => apiClient.get<BookingListItemDto[]>('/api/booking/my'),
    getMyBooking: (id: string) => apiClient.get<BookingDetailsDto>(`/api/booking/my/${id}`),
    
    cancel: (id: string, reason?: string) => apiClient.post(`/api/booking/${id}/cancel`, {
        reason: reason ?? null,}),
    // ADMIN
    getAdminBookings: () => apiClient.get<AdminBookingListItemDto[]>('/api/booking/admin'),
    getAdminBooking: (id: string) => apiClient.get<AdminBookingDetailsDto>(`/api/booking/admin/${id}`),

    changeStatus: (id: string, status: BookingStatus) => apiClient.patch(`/api/booking/admin/${id}/status`, {
            bookingId: id,
            status,
        }),

    createByAdmin: (command: CreateBookingCommand) => apiClient.post('/api/booking/admin', command),

    getAdminCustomerBookings: (customerId: string) => apiClient.get<BookingListItemDto[]>(`/api/booking/admin/customer/${customerId}`),

    // CREATE
    create: (command: CreateBookingCommand) => apiClient.post('/api/booking', command),

    // DOCUMENTS
    uploadDocuments: (formData: FormData) => apiClient.post('/api/booking/documents', formData),

    // PRICE
    calculatePrice: (command: CalculateBookingPriceRequest) => apiClient.post<BookingCalculationResultDto>('/api/booking/calculate-price', command),

    previewChangePeriod: ( id: string, startDate: string, endDate: string ) => apiClient.post<ChangeBookingPeriodPreviewDto>(`/api/booking/${id}/period/preview`, { startDate, endDate, } ), 

    changePeriod: ( id: string, startDate: string, endDate: string ) => apiClient.patch( `/api/booking/${id}/period`, { startDate, endDate, } ),

    previewChangeServices: (id: string, services: BookingServiceRequest[]
    ) => apiClient.post<BookingServicesChangePreviewDto>(`/api/booking/${id}/services/preview`, { services }),

    changeServices: (id: string, services: BookingServiceRequest[]
    ) => apiClient.patch(`/api/booking/${id}/services`, {bookingId: id, services }),

};
