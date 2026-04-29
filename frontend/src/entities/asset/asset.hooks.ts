import { useQuery } from '@tanstack/react-query';
import { searchAssets, getAssetById, getPopularAssets } from './asset.api';
import { AssetFilters, PaginatedResponse, Asset } from './asset.types';
import { AssetType, FuelType, TransmissionType, DriveType, AssetStatus, AssetCategory } from './asset.types';
import { useI18n } from '@/app/providers/I18nProvider';

type EnumLike = string | number | null | undefined;

const toEnumNumber = <T extends Record<string, string | number>>(enumMap: T, value: EnumLike): number | null => {
    if (typeof value === 'number') {
        return value;
    }

    if (typeof value === 'string') {
        const direct = enumMap[value as keyof T];
        if (typeof direct === 'number') {
            return direct;
        }
        const numeric = Number(value);
        return Number.isNaN(numeric) ? null : numeric;
    }

    return null;
};

export const useAssets = (filters?: AssetFilters) => {
    return useQuery<PaginatedResponse<Asset>>({
        queryKey: ['assets', filters],
        queryFn: () => searchAssets(filters),
        staleTime: 5 * 60 * 1000,
    });
};

export const useAsset = (id: string) => {
    return useQuery({
        queryKey: ['asset', id],
        queryFn: () => getAssetById(id),
        enabled: !!id,
        staleTime: 5 * 60 * 1000,
    });
};

export const usePopularAssets = (limit: number = 4) => {
  return useQuery({
      queryKey: ['assets', 'popular', limit],
      queryFn: () => getPopularAssets(limit),
      staleTime: 5 * 60 * 1000,
  });
};

// Конвертеры для отображения на UI
export const useAssetTypeLabel = () => {
    const { t } = useI18n();
    return (type: AssetType): string => {
        switch (type) {
            case AssetType.Auto: return t('asset.util.car');
            case AssetType.Moto: return t('asset.util.moto');
            case AssetType.Truck: return t('asset.util.truck');
            case AssetType.Scooter: return t('asset.util.scooter');
            case AssetType.Equipment: return t('asset.util.equip');
            case AssetType.Bike: return t('asset.util.bike');
            default: return t('asset.util.unknown');
        }
    };
};

export const useFuelTypeLabel = () => {
    const { t } = useI18n();
    return  (fuelType: FuelType | EnumLike): string => {
        switch (toEnumNumber(FuelType, fuelType)) {
            case FuelType.Petrol: return t('asset.util.petrol');
            case FuelType.Diesel: return t('asset.util.diesel');
            case FuelType.Electric: return t('asset.util.electric');
            case FuelType.Hybrid: return t('asset.util.hybrid');
            case FuelType.LPG: return t('asset.util.gas');
            default: return t('asset.util.unknown');
        }
    };
};

export const useTransmissionLabel = () => {
    const { t } = useI18n();
    return (transmission: TransmissionType | EnumLike): string => {
        switch (toEnumNumber(TransmissionType, transmission)) {
            case TransmissionType.Manual: return t('asset.util.manual');
            case TransmissionType.Automatic: return t('asset.util.automatic');
            case TransmissionType.Robotic: return t('asset.util.robotic');
            case TransmissionType.CVT: return t('asset.util.cvt');
            default: return t('asset.util.unknown');
        }
    };
};

export const useDriveTypeLabel = () => {
    const { t } = useI18n();
    return (driveType: DriveType | EnumLike): string => {
        switch (toEnumNumber(DriveType, driveType)) {
            case DriveType.FrontWheelDrive: return t('asset.util.front');
            case DriveType.RearWheelDrive: return t('asset.util.rear');
            case DriveType.AllWheelDrive: return t('asset.util.allWheel');
            default: return t('asset.util.unknown');
        }
    };
};

export const useStatusLabel = () => {
    const { t } = useI18n();
    return (status: AssetStatus): string => {
        switch (status) {
            case AssetStatus.Available: return t('asset.util.available');
            case AssetStatus.Reserved: return t('asset.util.reserved');
            case AssetStatus.InRent: return t('asset.util.inRent');
            case AssetStatus.Service: return t('asset.util.inService');
            case AssetStatus.Disabled: return t('asset.util.unavailable');
            default: return t('asset.util.unknown');
        }
    };
};

export const useAssetCategoryLabel = () => {
    const { t } = useI18n();
    return (category: AssetCategory): string => {
        switch (category) {
            case AssetCategory.Limousine: return t('asset.util.limousine');
            case AssetCategory.Sedan: return t('asset.util.sedan');
            case AssetCategory.Hatchback: return t('asset.util.hatch');
            case AssetCategory.Liftback: return t('asset.util.liftBack');
            case AssetCategory.Universal: return t('asset.util.stationWagon');
            case AssetCategory.Coupe: return t('asset.util.coupe');
            case AssetCategory.Minivan: return t('asset.util.minivan');
            case AssetCategory.Pickup: return t('asset.util.pickup');
            case AssetCategory.Crossover: return t('asset.util.crossover');
            default: return t('asset.util.unknown');
        }
    };
};