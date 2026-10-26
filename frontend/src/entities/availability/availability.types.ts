import type { Asset } from "@/entities/asset/asset.types";

export interface BusyPeriodDto {
    startDate: string;
    endDate: string;
}

export type AvailableAssetDto = Asset;