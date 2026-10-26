import { apiClient } from "@/shared/api/client";
import type { BusyPeriodDto, AvailableAssetDto } from "./availability.types";

export const availabilityApi = {
    getBusyPeriods: (assetId: string, excludeBookingId?: string) =>
        apiClient.get<BusyPeriodDto[]>(
            `/api/availability/assets/${assetId}/busy-periods`,
            {
                params: {
                    excludeBookingId,
                },
            }
        ),

    getAvailableAssets: (start: string, end: string) =>
        apiClient.get<AvailableAssetDto[]>(
            `/api/availability/assets?startDate=${start}&endDate=${end}`
        ),
};