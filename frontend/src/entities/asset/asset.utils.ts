import { AssetType, FuelType, TransmissionType, DriveType, AssetStatus, AssetCategory } from './asset.types';

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

// Конвертеры для отображения на UI
export const getAssetTypeLabel = (type: AssetType): string => {
    switch (type) {
        case AssetType.Auto: return 'Легковой транспорт';
        case AssetType.Moto: return 'Мототранспорт';
        case AssetType.Truck: return 'Грузовой траспорт';
        case AssetType.Scooter: return 'Мопед';
        case AssetType.Equipment: return 'Оборудование';
        case AssetType.Bike: return 'Велосипед';
        default: return 'Неизвестно';
    }
};

export const getFuelTypeLabel = (fuelType: FuelType | EnumLike): string => {
    switch (toEnumNumber(FuelType, fuelType)) {
        case FuelType.Petrol: return 'Бензин';
        case FuelType.Diesel: return 'Дизель';
        case FuelType.Electric: return 'Электричество';
        case FuelType.Hybrid: return 'Гибрид';
        case FuelType.LPG: return 'Газ';
        default: return 'Неизвестно';
    }
};

export const getTransmissionLabel = (transmission: TransmissionType | EnumLike): string => {
    switch (toEnumNumber(TransmissionType, transmission)) {
        case TransmissionType.Manual: return 'Механическая';
        case TransmissionType.Automatic: return 'Автомат';
        case TransmissionType.Robotic: return 'Робот';
        case TransmissionType.CVT: return 'Вариатор';
        default: return 'Неизвестно';
    }
};

export const getDriveTypeLabel = (driveType: DriveType | EnumLike): string => {
    switch (toEnumNumber(DriveType, driveType)) {
        case DriveType.FrontWheelDrive: return 'Передний привод';
        case DriveType.RearWheelDrive: return 'Задний привод';
        case DriveType.AllWheelDrive: return 'Полный привод';
        default: return 'Неизвестно';
    }
};

export const getStatusLabel = (status: AssetStatus): string => {
    switch (status) {
        case AssetStatus.Available: return 'Доступен';
        case AssetStatus.Reserved: return 'Забронирован';
        case AssetStatus.InRent: return 'В аренде';
        case AssetStatus.Service: return 'В сервисе';
        case AssetStatus.Disabled: return 'Недоступен';
        default: return 'Неизвестно';
    }
};

export const getStatusColor = (status: AssetStatus): string => {
    switch (status) {
        case AssetStatus.Available: return 'green';
        case AssetStatus.Reserved: return 'yellow';
        case AssetStatus.InRent: return 'blue';
        case AssetStatus.Service: return 'orange';
        case AssetStatus.Disabled: return 'red';
        default: return 'gray';
    }
};

export const getAssetCategoryLabel = (category: AssetCategory): string => {
    switch (category) {
        case AssetCategory.Limousine: return 'Лимузин';
        case AssetCategory.Sedan: return 'Седан';
        case AssetCategory.Hatchback: return 'Хэтчбек';
        case AssetCategory.Liftback: return 'Лифтбек';
        case AssetCategory.Universal: return 'Универсал';
        case AssetCategory.Coupe: return 'Купе';
        case AssetCategory.Minivan: return 'Минивен';
        case AssetCategory.Pickup: return 'Пикап';
        case AssetCategory.Crossover: return 'Кроссовер';
        default: return 'Неизвестно';
    }
};