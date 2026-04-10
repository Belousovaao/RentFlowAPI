export enum AssetType {
    Auto = 1,
    Moto = 2,
    Truck = 3,
    Scooter = 4,
    Equipment = 5,
    Bike = 6
}

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

export enum FuelType {
    Petrol = 1,
    Diesel = 2,
    Electric = 3,
    Hybrid = 4,
    LPG = 5
}

export enum TransmissionType {
    Manual = 1,
    Automatic = 2,
    Robotic = 3,
    CVT = 4
}

export enum DriveType {
    FrontWheelDrive = 1, //передний привод
    RearWheelDrive = 2,  //задний привод
    AllWheelDrive = 3,  // полный привод
}

export enum AssetStatus {
    Available = 1,
    Reserved = 2,
    InRent = 3,
    Service = 4,
    Disabled = 5
}

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
    photos: string[];
    isAvailable: boolean;
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