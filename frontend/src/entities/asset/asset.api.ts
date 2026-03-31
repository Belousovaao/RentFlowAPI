import { apiClient } from "../../shared/api/client";
import type { Asset } from "./asset.types";

export const getAssets = async (): Promise<Asset[]> => {
    return apiClient.get<Asset[]>("/api/assets");
};