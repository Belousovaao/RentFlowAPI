export interface AdditionalService {
    id: string;
    name: string;
    type: ServiceType,
    price: number;
}

export interface SelectedAdditionalService {
    id: string;
    name: string;
    type: ServiceType;
    price: number;
    selectedTime?: string;
}

export enum ServiceType {
    Regular = "Regular",
    EarlyPickup = "EarlyPickup",
    LateReturn = "LateReturn"
}