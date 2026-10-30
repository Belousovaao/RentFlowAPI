import { apiClient } from "@/shared/api/client";

export type BookingChargeType =
    | "LateCancellation"
    | "TrafficFine"
    | "Cleaning"
    | "Refueling"
    | "ExcessMileage"
    | "PropertyLossOrDamage"
    | "Other";

export type BookingChargeStatus = "Accrued" | "Rejected";

export interface BookingChargeDto {
    id: string;
    bookingId: string;
    type: BookingChargeType;
    status: BookingChargeStatus;
    title: string;
    description: string | null;
    referenceNumber: string | null;
    quantity: number;
    unit: string;
    unitPrice: number;
    amount: number;
    currency: string;
    paidAmount: number;
    balance: number;
    createdAt: string;
    reviewedAt: string | null;
}

export interface RentalTariffDto {
    code: string;
    price: number;
    unit: string;
    updatedAtUtc: string;
}

export type PaymentType = "Prepayment" | "AdditionalPayment" | "Refund";
export type PaymentMethod = "BankTransfer" | "Online";

export interface PaymentTransactionDto {
    id: string;
    bookingId: string;
    originalPaymentId: string | null;
    amount: number;
    currency: string;
    type: PaymentType;
    method: PaymentMethod;
    externalOperationId: string | null;
    createdAt: string;
    note: string | null;
}

export interface BookingPaymentSummaryDto {
    bookingId: string;
    currency: string;
    totalReceived: number;
    totalRefunded: number;
    pendingRefunds: number;
    netPaid: number;
    availableToRefund: number;
}

export interface CreateBookingChargeRequest {
    bookingId: string;
    type: BookingChargeType;
    amount?: number | null;
    quantity?: number | null;
    title?: string | null;
    description?: string | null;
    referenceNumber?: string | null;
}

export interface RegisterIncomingPaymentRequest {
    bookingId: string;
    amount: number;
    type: "Prepayment" | "AdditionalPayment";
    method: PaymentMethod;
    note?: string | null;
}

export interface CreateRefundRequest {
    originalPaymentId: string;
    amount: number;
    note?: string | null;
}

export interface PaymentAllocationItem {
    bookingChargeId: string;
    amount: number;
}

export const bookingFinanceApi = {
    getCharges: (bookingId: string) =>
        apiClient.get<BookingChargeDto[]>(`/api/booking-charges/booking/${bookingId}`),

    getRentalTariffs: () =>
        apiClient.get<RentalTariffDto[]>("/api/rental-tariffs"),

    updateRentalTariff: (code: string, price: number, unit: string) =>
        apiClient.put<RentalTariffDto>(`/api/rental-tariffs/${encodeURIComponent(code)}`, { price, unit }),

    createCharge: (bookingId: string, request: CreateBookingChargeRequest) =>
        apiClient.post<{ id: string }>(`/api/booking-charges/admin/booking/${bookingId}`, request),

    confirmCharge: (bookingId: string, chargeId: string) =>
        apiClient.post<void>(`/api/booking-charges/admin/booking/${bookingId}/${chargeId}/confirm`),

    rejectCharge: (bookingId: string, chargeId: string) =>
        apiClient.post<void>(`/api/booking-charges/admin/booking/${bookingId}/${chargeId}/reject`),

    getTransactions: (bookingId: string) =>
        apiClient.get<PaymentTransactionDto[]>(`/api/payments/booking/${bookingId}/transactions`),

    getSummary: (bookingId: string) =>
        apiClient.get<BookingPaymentSummaryDto>(`/api/payments/booking/${bookingId}/summary`),

    registerIncoming: (request: RegisterIncomingPaymentRequest) =>
        apiClient.post<{ id: string }>("/api/payments/incoming", request),


    createRefund: (request: CreateRefundRequest) =>
        apiClient.post<{ id: string }>("/api/payments/refunds", request),


    allocatePayment: (paymentId: string, allocations: PaymentAllocationItem[]) =>
        apiClient.put<void>(`/api/payments/${paymentId}/allocations`, { allocations }),
};
