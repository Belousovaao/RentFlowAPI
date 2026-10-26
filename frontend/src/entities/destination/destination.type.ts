export enum DestinationType {
    Local = "Local",
    Regional = "Regional",
    LongDistance = "LongDistance",
    Other = "Other",
    Restricted = "Restricted"
}

export interface DestinationDto {
    id: string;
    name: string;
    region: string;
    distanceKm: number;
    type: DestinationType;
    routeSurcharge: number;
}

export enum RouteReviewStatus {
    NotRequired = "NotRequired",
    Pending = "Pending",
    Approved = "Approved",
    Rejected = "Rejected",
}