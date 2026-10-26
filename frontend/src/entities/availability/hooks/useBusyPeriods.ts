import { useQuery } from "@tanstack/react-query";
import { availabilityApi } from "../availability.api";

export const useBusyPeriods = (assetId?: string, excludeBookingId?: string) =>
    useQuery({
        queryKey: ["busyPeriods", assetId, excludeBookingId],
        queryFn: () => availabilityApi.getBusyPeriods(assetId!, excludeBookingId),
        enabled: !!assetId,
    });