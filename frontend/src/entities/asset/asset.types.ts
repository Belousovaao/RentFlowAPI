export enum AssetType {
    Auto = 1,
    Moto = 2,
    Truck = 3,
    Scooter = 4,
    Equipment = 5,
    Bike = 6
}

export const AssetTypeLabels: Record<AssetType, string> = {
    [AssetType.Auto]: "Автомобиль",
    [AssetType.Moto]: "Мотоцикл",
    [AssetType.Truck]: "Грузовик",
    [AssetType.Scooter]: "Скутер",
    [AssetType.Equipment]: "Спецтехника",
    [AssetType.Bike]: "Велосипед"
};

export enum AssetCategory {
    Limousine = 1,
    Sedan = 2,
    Hatchback = 3,
    Liftback = 4,
    Universal = 5,
    Coupe = 6,
    Minivan = 7,
    Pickup = 8,
    Crossover = 9
}

export const AssetCategoryLabels: Record<AssetCategory, string> = {
    [AssetCategory.Limousine]: "Лимузин",
    [AssetCategory.Sedan]: "Седан",
    [AssetCategory.Hatchback]: "Хэтчбек",
    [AssetCategory.Liftback]: "Лифтбек",
    [AssetCategory.Universal]: "Универсал",
    [AssetCategory.Coupe]: "Купе",
    [AssetCategory.Minivan]: "Минивэн",
    [AssetCategory.Pickup]: "Пикап",
    [AssetCategory.Crossover]: "Кроссовер"
};

export enum FuelType {
    Petrol = 1,
    Diesel = 2,
    Electric = 3,
    Hybrid = 4,
    LPG = 5
}

export const FuelTypeLabels: Record<FuelType, string> = {
    [FuelType.Petrol]: "Бензин",
    [FuelType.Diesel]: "Дизель",
    [FuelType.Electric]: "Электро",
    [FuelType.Hybrid]: "Гибрид",
    [FuelType.LPG]: "Газ"
};

export enum TransmissionType {
    Manual = 1,
    Automatic = 2,
    Robotic = 3,
    CVT = 4
}

export const TransmissionTypeLabels: Record<TransmissionType, string> = {
    [TransmissionType.Manual]: "Механика",
    [TransmissionType.Automatic]: "Автомат",
    [TransmissionType.Robotic]: "Робот",
    [TransmissionType.CVT]: "Вариатор"
};

export enum DriveType {
    FrontWheelDrive = 1, //передний привод
    RearWheelDrive = 2,  //задний привод
    AllWheelDrive = 3,  // полный привод
}

export const DriveTypeLabels: Record<DriveType, string> = {
    [DriveType.FrontWheelDrive]: "Передний",
    [DriveType.RearWheelDrive]: "Задний",
    [DriveType.AllWheelDrive]: "Полный"
};

export enum AssetStatus {
    Available = "Available",
    Reserved = "Reserved", 
    InRent = "InRent",
    Service = "Service",
    Disabled = "Disabled"
}

export const AssetStatusLabels: Record<AssetStatus, string> = {
    [AssetStatus.Available]: "Доступен",
    [AssetStatus.Reserved]: "Зарезервирован",
    [AssetStatus.InRent]: "В аренде",
    [AssetStatus.Service]: "На обслуживании",
    [AssetStatus.Disabled]: "Отключен"
};

export const AssetStatusColors: Record<AssetStatus, string> = {
    [AssetStatus.Available]: "bg-green-100 text-green-800",
    [AssetStatus.Reserved]: "bg-yellow-100 text-yellow-800",
    [AssetStatus.InRent]: "bg-blue-100 text-blue-800",
    [AssetStatus.Service]: "bg-orange-100 text-orange-800",
    [AssetStatus.Disabled]: "bg-red-100 text-red-800"
};

// Основной DTO
export type Asset = {
    id: string;
    code: string;
    brandName: string;
    model: string;
    year: number;
    shortDescription: string;
    fullDescription: string;
    type: AssetType;
    category: AssetCategory;
    fuelType: FuelType;
    transmission: TransmissionType;
    driveType: DriveType;
    seats: number;
    doors: number;
    engine: string;
    horsepower: number;
    acceleration: string;
    topSpeed: number;
    color: string;
    features: string[];
    dailyPrice: number;
    deposit: number | null;
    status: AssetStatus;
    locationId: string;
    locationName: string;
    latitude: number | null;
    longitude: number | null;
    distanceFromUser: number | null;
    canDeliver: boolean;
    deliveryPrice: number;
    photos: Photo[];
    exteriorPhotos: Photo[];
    interiorPhotos: Photo[];
};

export interface Photo {
    id: string;
    url: string;
    type: 'Exterior' | 'Interior';
    order: number;
};

export type AssetFilters = {
    // Поиск
    search?: string;
    brand?: string;
    
    // Основные фильтры
    type?: AssetType;
    category?: AssetCategory;
    
    // Технические характеристики
    yearFrom?: number;
    yearTo?: number;
    fuelType?: FuelType;
    transmission?: TransmissionType;
    driveType?: DriveType;
    minSeats?: number;
    maxSeats?: number;
    minDoors?: number;
    maxDoors?: number;
    minHorsepower?: number;
    maxHorsepower?: number;
    
    // Ценовые фильтры
    priceFrom?: number;
    priceTo?: number;
    
    // Геолокация
    locationId?: string;
    latitude?: number;
    longitude?: number;
    radiusKm?: number;
    
    // Доступность
    startDate?: string;
    endDate?: string;
    status?: AssetStatus;
    
    // Пагинация и сортировка
    page?: number;
    limit?: number;
    sortBy?: 'price_asc' | 'price_desc' | 'year_desc' | 'popular';
};

// Ответ с пагинацией
export type PaginatedResponse<T> = {
    data: T[];
    total: number;
    page: number;
    limit: number;
    totalPages: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
};

// Для создания/обновления
export type CreateAssetDto = {
    code: string;
    brandName: string;
    model: string;
    year: number;
    shortDescription: string;
    fullDescription: string;
    type: AssetType;
    category: AssetCategory;
    fuelType: FuelType;
    transmission: TransmissionType;
    driveType: DriveType;
    seats: number;
    doors: number;
    engine: string;
    horsepower: number;
    acceleration: string;
    topSpeed: number;
    color: string;
    features: string[];
    dailyPrice: number;
    deposit?: number | null;
    locationId: string;
    canDeliver: boolean;
    deliveryPrice?: number | null;
    latitude?: number | null;
    longitude?: number | null;
};

// Для обновления
export type UpdateAssetDto = Partial<CreateAssetDto>;