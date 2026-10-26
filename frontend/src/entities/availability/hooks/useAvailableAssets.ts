import { useQuery } from "@tanstack/react-query";
import { availabilityApi } from "../availability.api";
import { formatDateForBack } from "@/shared/utils/formatters";

export const useAvailableAssets = (
    start?: Date | null,
    end?: Date | null
) =>
    useQuery({
        queryKey: ["availableAssets", start, end],
        queryFn: () =>
            availabilityApi.getAvailableAssets(
                formatDateForBack(start!),
                formatDateForBack(end!)
            ),
        enabled: !!start && !!end,
    });