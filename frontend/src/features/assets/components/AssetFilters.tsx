import { useEffect, useState } from 'react';
import { AnimatePresence, motion } from 'framer-motion';
import { X, SlidersHorizontal, ChevronDown, ChevronUp, Car, Fuel, Users, Settings, Zap } from 'lucide-react';
import { FuelType, TransmissionType, DriveType, AssetStatus, type AssetFilters as AssetFiltersType } from '../../../entities/asset/asset.types';
import { getFuelTypeLabel, getTransmissionLabel, getDriveTypeLabel, getStatusLabel } from '../../../entities/asset/asset.utils';

interface AssetFiltersProps {
    filters: AssetFiltersType;
    onFilterChange: (filters: AssetFiltersType) => void;
    onClose?: () => void;
    isMobile?: boolean;
}

export const AssetFilters = ({ 
    filters, 
    onFilterChange, 
    onClose, 
    isMobile = false 
}: AssetFiltersProps) => {
    const [isExpanded, setIsExpanded] = useState(true);
    const [localFilters, setLocalFilters] = useState<AssetFiltersType>(filters);
    const [sections, setSections] = useState({
        basic: true,
        price: true,
        year: false,
        fuel: false,
        transmission: false,
        drive: false,
        specs: false,
        power: false,
        status: false
    });

    useEffect(() => {
        setLocalFilters(filters);
    }, [filters]);

    // Применение фильтров
    const applyFilters = () => {
        onFilterChange(localFilters);
        if (isMobile && onClose) {
            onClose();
        }
    };

    // Сброс фильтров
    const resetFilters = () => {
        const resetFilters: AssetFiltersType = {
            page: 1,
            limit: filters.limit
        };
        setLocalFilters(resetFilters);
        onFilterChange(resetFilters);
    };

    // Обновление фильтра
    const updateFilter = <K extends keyof AssetFiltersType>(
        key: K, 
        value: AssetFiltersType[K]
    ) => {
        setLocalFilters(prev => ({ ...prev, [key]: value, page: 1 }));
    };

    // Опции для select
    const fuelTypeOptions = [
        { value: FuelType.Petrol, label: getFuelTypeLabel(FuelType.Petrol) },
        { value: FuelType.Diesel, label: getFuelTypeLabel(FuelType.Diesel) },
        { value: FuelType.Electric, label: getFuelTypeLabel(FuelType.Electric) },
        { value: FuelType.Hybrid, label: getFuelTypeLabel(FuelType.Hybrid) },
        { value: FuelType.LPG, label: getFuelTypeLabel(FuelType.LPG) },
    ];

    const transmissionOptions = [
        { value: TransmissionType.Manual, label: getTransmissionLabel(TransmissionType.Manual) },
        { value: TransmissionType.Automatic, label: getTransmissionLabel(TransmissionType.Automatic) },
        { value: TransmissionType.Robotic, label: getTransmissionLabel(TransmissionType.Robotic) },
        { value: TransmissionType.CVT, label: getTransmissionLabel(TransmissionType.CVT) },
    ];

    const driveTypeOptions = [
        { value: DriveType.FrontWheelDrive, label: getDriveTypeLabel(DriveType.FrontWheelDrive) },
        { value: DriveType.RearWheelDrive, label: getDriveTypeLabel(DriveType.RearWheelDrive) },
        { value: DriveType.AllWheelDrive, label: getDriveTypeLabel(DriveType.AllWheelDrive) },
    ];

    const statusOptions = [
        { value: AssetStatus.Available, label: getStatusLabel(AssetStatus.Available) },
        { value: AssetStatus.Reserved, label: getStatusLabel(AssetStatus.Reserved) },
        { value: AssetStatus.InRent, label: getStatusLabel(AssetStatus.InRent) },
    ];

    const activeCount = Object.keys(localFilters).filter((key) => {
        if (key === 'page' || key === 'limit' || key === 'sortBy') {
            return false;
        }
        return Boolean(localFilters[key as keyof AssetFiltersType]);
    }).length;

    const toggleSection = (key: keyof typeof sections) => {
        setSections((prev) => ({ ...prev, [key]: !prev[key] }));
    };

    const SectionHeader = ({ title, sectionKey }: { title: string; sectionKey: keyof typeof sections }) => (
        <button
            type="button"
            onClick={() => toggleSection(sectionKey)}
            className="w-full flex items-center justify-between text-left py-2"
        >
            <span className="text-sm font-semibold text-gray-700 dark:text-gray-200">{title}</span>
            {sections[sectionKey] ? <ChevronUp className="w-4 h-4" /> : <ChevronDown className="w-4 h-4" />}
        </button>
    );

    return (
        <div className="bg-white dark:bg-gray-800 rounded-xl shadow-lg border border-gray-100 dark:border-gray-700">
            {/* Заголовок */}
            <div className="flex items-center justify-between p-3 border-b border-gray-200 dark:border-gray-700">
                <div className="flex items-center space-x-2">
                    <SlidersHorizontal className="w-5 h-5 text-primary-600" />
                    <h3 className="text-base font-semibold text-gray-900 dark:text-white">
                        Фильтры
                    </h3>
                    {activeCount > 0 && (
                        <span className="px-2 py-0.5 text-xs bg-primary-100 dark:bg-primary-900 
                                       text-primary-600 dark:text-primary-300 rounded-full">
                            {activeCount}
                        </span>
                    )}
                </div>
                
                <button
                    onClick={() => setIsExpanded(!isExpanded)}
                    className="p-1 hover:bg-gray-100 dark:hover:bg-gray-700 rounded-lg transition-colors"
                >
                    {isExpanded ? <ChevronUp className="w-5 h-5" /> : <ChevronDown className="w-5 h-5" />}
                </button>
                
                {isMobile && onClose && (
                    <button onClick={onClose} className="p-1 hover:bg-gray-100 dark:hover:bg-gray-700 rounded-lg">
                        <X className="w-5 h-5" />
                    </button>
                )}
            </div>

            <AnimatePresence>
                {isExpanded && (
                    <motion.div
                        initial={{ height: 0, opacity: 0 }}
                        animate={{ height: 'auto', opacity: 1 }}
                        exit={{ height: 0, opacity: 0 }}
                        transition={{ duration: 0.3 }}
                        style={{overflow: 'hidden' }}
                    >
                        <div className="p-3 space-y-3">
                            {/* Поиск по тексту */}
                            <div>
                                <SectionHeader title="Основное" sectionKey="basic" />
                                {sections.basic && (
                                    <input
                                        type="text"
                                        value={localFilters.search || ''}
                                        onChange={(e) => updateFilter('search', e.target.value)}
                                        placeholder="Поиск по марке, модели..."
                                        className="w-full px-3 py-2 text-sm border border-gray-300 dark:border-gray-600 
                                                   rounded-lg bg-white dark:bg-gray-700 
                                                   focus:ring-2 focus:ring-primary-500 focus:border-transparent"
                                    />
                                )}
                            </div>

                            {/* Ценовой диапазон */}
                            <div>
                                <SectionHeader title="Цена в сутки" sectionKey="price" />
                                {sections.price && (
                                    <div className="grid grid-cols-2 gap-2">
                                        <input
                                            type="number"
                                            value={localFilters.priceFrom || ''}
                                            onChange={(e) => updateFilter('priceFrom', e.target.value ? Number(e.target.value) : undefined)}
                                            placeholder="От"
                                            className="w-full px-3 py-2 text-sm border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700"
                                        />
                                        <input
                                            type="number"
                                            value={localFilters.priceTo || ''}
                                            onChange={(e) => updateFilter('priceTo', e.target.value ? Number(e.target.value) : undefined)}
                                            placeholder="До"
                                            className="w-full px-3 py-2 text-sm border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700"
                                        />
                                    </div>
                                )}
                            </div>

                            {/* Год выпуска */}
                            <div>
                                <SectionHeader title="Год выпуска" sectionKey="year" />
                                {sections.year && (
                                    <div className="grid grid-cols-2 gap-2">
                                        <input
                                            type="number"
                                            value={localFilters.yearFrom || ''}
                                            onChange={(e) => updateFilter('yearFrom', e.target.value ? Number(e.target.value) : undefined)}
                                            placeholder="От"
                                            className="w-full px-3 py-2 text-sm border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700"
                                        />
                                        <input
                                            type="number"
                                            value={localFilters.yearTo || ''}
                                            onChange={(e) => updateFilter('yearTo', e.target.value ? Number(e.target.value) : undefined)}
                                            placeholder="До"
                                            className="w-full px-3 py-2 text-sm border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700"
                                        />
                                    </div>
                                )}
                            </div>

                            {/* Тип топлива */}
                            <div>
                                <SectionHeader title="Тип топлива" sectionKey="fuel" />
                                {sections.fuel && (
                                    <div className="grid grid-cols-2 gap-2">
                                        {fuelTypeOptions.map(option => (
                                            <button
                                                key={option.value}
                                                onClick={() => updateFilter('fuelType', 
                                                    localFilters.fuelType === option.value ? undefined : option.value
                                                )}
                                                className={`px-2.5 py-2 text-xs rounded-lg border transition-all
                                                    ${localFilters.fuelType === option.value
                                                        ? 'border-primary-500 bg-primary-50 dark:bg-primary-900/20 text-primary-700 dark:text-primary-300'
                                                        : 'border-gray-300 dark:border-gray-600 hover:border-primary-300'
                                                    }`}
                                            >
                                                {option.label}
                                            </button>
                                        ))}
                                    </div>
                                )}
                            </div>

                            {/* Тип трансмиссии */}
                            <div>
                                <SectionHeader title="Трансмиссия" sectionKey="transmission" />
                                {sections.transmission && (
                                    <div className="grid grid-cols-2 gap-2">
                                        {transmissionOptions.map(option => (
                                            <button
                                                key={option.value}
                                                onClick={() => updateFilter('transmission', 
                                                    localFilters.transmission === option.value ? undefined : option.value
                                                )}
                                                className={`px-2.5 py-2 text-xs rounded-lg border transition-all
                                                    ${localFilters.transmission === option.value
                                                        ? 'border-primary-500 bg-primary-50 dark:bg-primary-900/20 text-primary-700 dark:text-primary-300'
                                                        : 'border-gray-300 dark:border-gray-600 hover:border-primary-300'
                                                    }`}
                                            >
                                                {option.label}
                                            </button>
                                        ))}
                                    </div>
                                )}
                            </div>

                            {/* Тип привода */}
                            <div>
                                <SectionHeader title="Привод" sectionKey="drive" />
                                {sections.drive && (
                                    <div className="grid grid-cols-1 gap-2">
                                        {driveTypeOptions.map(option => (
                                            <button
                                                key={option.value}
                                                onClick={() => updateFilter('driveType',
                                                    localFilters.driveType === option.value ? undefined : option.value
                                                )}
                                                className={`px-2.5 py-2 text-xs rounded-lg border transition-all
                                                    ${localFilters.driveType === option.value
                                                        ? 'border-primary-500 bg-primary-50 dark:bg-primary-900/20 text-primary-700 dark:text-primary-300'
                                                        : 'border-gray-300 dark:border-gray-600 hover:border-primary-300'
                                                    }`}
                                            >
                                                {option.label}
                                            </button>
                                        ))}
                                    </div>
                                )}
                            </div>

                            {/* Количество мест и дверей */}
                            <div>
                                <SectionHeader title="Места и двери" sectionKey="specs" />
                                {sections.specs && (
                                    <div className="grid grid-cols-2 gap-2">
                                        <select
                                            value={localFilters.minSeats || ''}
                                            onChange={(e) => updateFilter('minSeats', e.target.value ? Number(e.target.value) : undefined)}
                                            className="w-full px-3 py-2 text-sm border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700"
                                        >
                                            <option value="">Места</option>
                                            {[2, 4, 5, 6, 7, 8].map(seats => (
                                                <option key={seats} value={seats}>{seats}+</option>
                                            ))}
                                        </select>
                                        <select
                                            value={localFilters.minDoors || ''}
                                            onChange={(e) => updateFilter('minDoors', e.target.value ? Number(e.target.value) : undefined)}
                                            className="w-full px-3 py-2 text-sm border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700"
                                        >
                                            <option value="">Двери</option>
                                            {[2, 3, 4, 5].map(doors => (
                                                <option key={doors} value={doors}>{doors}+</option>
                                            ))}
                                        </select>
                                    </div>
                                )}
                            </div>

                            {/* Мощность двигателя */}
                            <div>
                                <SectionHeader title="Мощность" sectionKey="power" />
                                {sections.power && (
                                    <div className="grid grid-cols-2 gap-2">
                                        <input
                                            type="number"
                                            value={localFilters.minHorsepower || ''}
                                            onChange={(e) => updateFilter('minHorsepower', e.target.value ? Number(e.target.value) : undefined)}
                                            placeholder="От л.с."
                                            className="w-full px-3 py-2 text-sm border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700"
                                        />
                                        <input
                                            type="number"
                                            value={localFilters.maxHorsepower || ''}
                                            onChange={(e) => updateFilter('maxHorsepower', e.target.value ? Number(e.target.value) : undefined)}
                                            placeholder="До л.с."
                                            className="w-full px-3 py-2 text-sm border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700"
                                        />
                                    </div>
                                )}
                            </div>

                            {/* Статус доступности */}
                            <div>
                                <SectionHeader title="Статус" sectionKey="status" />
                                {sections.status && (
                                    <div className="flex flex-wrap gap-2">
                                        {statusOptions.map(option => (
                                            <button
                                                key={option.value}
                                                onClick={() => updateFilter('status', 
                                                    localFilters.status === option.value ? undefined : option.value
                                                )}
                                                className={`px-2.5 py-2 text-xs rounded-lg border transition-all
                                                    ${localFilters.status === option.value
                                                        ? 'border-primary-500 bg-primary-50 dark:bg-primary-900/20 text-primary-700 dark:text-primary-300'
                                                        : 'border-gray-300 dark:border-gray-600 hover:border-primary-300'
                                                    }`}
                                            >
                                                {option.label}
                                            </button>
                                        ))}
                                    </div>
                                )}
                            </div>

                            {/* Кнопки действий */}
                            <div className="grid grid-cols-2 gap-2 pt-3 border-t border-gray-200 dark:border-gray-700">
                                <button
                                    onClick={resetFilters}
                                    className="px-3 py-2 text-sm border border-gray-300 dark:border-gray-600 
                                               rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 
                                               transition-colors text-gray-700 dark:text-gray-300"
                                >
                                    Сброс
                                </button>
                                <button
                                    onClick={applyFilters}
                                    className="px-3 py-2 text-sm bg-primary-600 text-white rounded-lg 
                                               hover:bg-primary-700 transition-colors"
                                >
                                    Применить
                                </button>
                            </div>
                        </div>
                    </motion.div>
                )}
            </AnimatePresence>
        </div>
    );
};