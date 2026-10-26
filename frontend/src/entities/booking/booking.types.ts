import { AssetShortDto } from "../asset/asset.types";
import { CustomerShortDto } from "../customer/customer.type";
import { DestinationDto, DestinationType, RouteReviewStatus } from "../destination/destination.type";
import { ServiceType } from "../service/service.types";

export interface RentalPeriodDto {
    startDate: string;
    endDate: string;
}

export type BookingServiceRequest = {
    additionalServiceId: string;
    selectedTime?: string;
};

export type CreateBookingCommand = {
    assetId: string;
    customerId: string;
    destinationType: DestinationType;
    destinationId: string | null;
    customRouteDescription: string | null;
    pickupDate: string;
    pickupTime: string;
    returnDate: string;
    returnTime: string;
    driver?: {
        firstName: string;
        lastName: string;
        middleName?: string;
        lisenceNumber: string;
        categories: string[];
        issueDate: string;
        expirationDate: string;
        experienceFrom: number;
        phone: string;
    };
    services: BookingServiceRequest[];
};

export type CalculateBookingPriceRequest = {
    assetId: string;
    destinationType: DestinationType;
    destinationId: string | null;
    customRouteDescription: string | null;
    pickupDate: string;
    pickupTime?: string;
    returnDate: string;
    returnTime?: string;
    services: BookingServiceRequest[];
};

export interface RentalPriceSegmentDto {
    days: number;
    baseDailyPrice: number;
    durationDiscountPercent: number;
    discountedDailyPrice: number;
    baseRentalPrice: number;
    discountAmount: number;
    total: number;
    appliedForDays: number;
}

export interface BookingPenaltyDto {
    id: string;
    type: BookingPenaltyType;
    amount: number;
}

export enum BookingPenaltyType {
    PeriodReduction = 'PeriodReduction',
}

export interface RentalPrice {
    segments: RentalPriceSegmentDto[];
    /** Стоимость аренды без скидки */
    baseRentalPrice: number;
    /** Размер скидки в рублях */
    discountAmount: number;
    /** Стоимость аренды с учетом скидки */
    rentalPriceWithDiscount: number;
}

export interface BusyPeriodDto {
    startDate: string;
    endDate: string;
}

export interface BookingPriceDto {
    rentalPrice: RentalPrice;
    /** Депозит */
    deposit: number;
    /** Стоимость дополнительных услуг */
    servicesPrice: number;
    /** Доплата за маршрут */
    routeSurcharge: number;
    /** Итоговая стоимость аренды без депозита */
    total: number;
    /** Итоговая сумма к оплате, включая депозит */
    totalToPay: number;
    penalties: BookingPenaltyDto[];
    penaltiesPrice: number;
}

export interface BookingServiceDto {
    additionalServiceId: string;
    type: ServiceType;
    name: string;
    price: number;
    selectedTime?: string;
}

export interface BookingCalculationResultDto {
    price: BookingPriceDto;
    services: BookingServiceDto[];
}

export interface BookingListItemDto {
    id: string;
    asset: AssetShortDto;
    rentalPeriod: RentalPeriodDto;
    price: BookingPriceDto;
    status: BookingStatus;
    services: BookingServiceDto[];
}

export interface AdminBookingListItemDto {
    id: string;
    asset: AssetShortDto;
    customer: CustomerShortDto;
    rentalPeriod: RentalPeriodDto;
    price: BookingPriceDto;
    status: BookingStatus;
}

export interface AdminBookingDetailsDto {
    id: string;
    status: BookingStatus;
    asset: AssetShortDto;
    rentalPeriod: RentalPeriodDto;
    price: BookingPriceDto;
    services: BookingServiceDto[];
    customer: CustomerShortDto;
    destination: BookingDestinationDto;
    destinationDetails: DestinationDto | null;
}

export interface BookingDetailsDto {
    id: string;
    status: BookingStatus;
    customerId: string;
    customer: CustomerShortDto;
    assetId: string;
    asset: AssetShortDto;
    rentalPeriod: RentalPeriodDto;
    price: BookingPriceDto;
    services: BookingServiceDto[];
    destination: BookingDestinationDto;
    destinationDetails: DestinationDto | null;
}

export type BookingStatus = 'Pending' | 'Confirmed' | 'Cancelled' | 'Completed' | 'Active';

export interface ChangeBookingPeriodPreviewDto { 
    startDate: string; 
    endDate: string; 
    currentRentalDays: number;
    newRentalDays: number;
    currentDailyPrice: number;
    newDailyPrice: number;
    changedDays: number;
    currentRentalPrice: number; 
    newRentalPrice: number; 
    penalty: number; 
    currentTotal: number; 
    newTotal: number; 
    difference: number;
    isExtension: boolean; 
    isReduction: boolean;
    isAdditionalPayment: boolean; 
    isRefund: boolean;
    currentSegments: RentalPriceSegmentDto[];
    changedSegments: RentalPriceSegmentDto[];
}

export interface BookingServicesChangePreviewDto {
    currentServicesPrice: number;
    newServicesPrice: number;
    difference: number;
    currentTotal: number;
    newTotal: number;
    services: BookingServiceDto[];
}

export interface BookingDestinationDto {
    destinationId: string | null;
    type: DestinationType;
    customRouteDescription: string | null;
    reviewStatus: RouteReviewStatus;
    destinationNameSnapshot: string | null;
}