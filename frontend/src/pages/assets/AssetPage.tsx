import { useState } from 'react';
import { useAssets } from '../../entities/asset/asset.hooks';
import { AssetCard } from '../../features/assets/components/AssetCard';
import { AssetFilters } from '../../features/assets/components/AssetFilters';
import { DateFilter } from '../../features/assets/components/DateFilter';
import { AssetFilters as AssetFiltersType } from '../../entities/asset/asset.types';
import { useBookingModal } from '@/features/booking/hooks/useBookingModal';
import { BookingModal } from '../../features/booking/components/BookingModal';

export const AssetsPage = () => {
    console.log('🔥 AssetsPage MOUNTED');
    const [filters, setFilters] = useState<AssetFiltersType>({
        page: 1,
        limit: 12,
        sortBy: 'popular'
    });

    console.log('🔍 Current filters:', filters);
    
    const [dateRange, setDateRange] = useState<{ start: Date | null; end: Date | null }>({
        start: null,
        end: null
    });
    
    const { data: response, isLoading, error } = useAssets({
        ...filters,
        startDate: dateRange.start?.toISOString(),
        endDate: dateRange.end?.toISOString(),
    });
    
    console.log('📦 useAssets response:', { response, isLoading, error }); 
    
    const { isOpen, selectedAsset, openBookingModal, closeBookingModal } = useBookingModal();
    
    const handleDateChange = (start: Date | null, end: Date | null) => {
        setDateRange({ start, end });
        setFilters(prev => ({ ...prev, page: 1 }));
    };
    
    const handleSortChange = (sortBy: string) => {
        setFilters(prev => ({ ...prev, sortBy: sortBy as any, page: 1 }));
    };
    
    const assets = response?.data || [];
    const total = response?.total || 0;
    
    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-900 pt-24 pb-12">
            <div className="container mx-auto px-4">
                {/* Заголовок */}
                <div className="mb-6">
                    <h1 className="text-3xl font-bold text-gray-900 dark:text-white">
                        Наш автопарк
                    </h1>
                    <p className="text-gray-600 dark:text-gray-400 mt-1">
                        {total} автомобилей в наличии
                    </p>
                </div>
                
                {/* Фильтр по датам */}
                <DateFilter
                    startDate={dateRange.start}
                    endDate={dateRange.end}
                    onChange={handleDateChange}
                />
                
                <div className="flex flex-col lg:flex-row gap-6">
                    {/* Боковая панель с фильтрами */}
                    <div className="lg:w-80 flex-shrink-0">
                        <AssetFilters
                            filters={filters}
                            onFilterChange={setFilters}
                        />
                    </div>
                    
                    {/* Основной контент */}
                    <div className="flex-1">
                        {/* Сортировка */}
                        <div className="flex justify-between items-center mb-6">
                            <p className="text-sm text-gray-500">
                                Показано: {assets.length} из {total}
                            </p>
                            <select
                                value={filters.sortBy || 'popular'}
                                onChange={(e) => handleSortChange(e.target.value)}
                                className="px-3 py-2 border border-gray-300 dark:border-gray-600 
                                           rounded-lg bg-white dark:bg-gray-800 text-sm
                                           text-gray-700 dark:text-gray-300"
                            >
                                <option value="popular">Сначала популярные</option>
                                <option value="price_asc">Сначала дешевле</option>
                                <option value="price_desc">Сначала дороже</option>
                                <option value="year_desc">Сначала новее</option>
                            </select>
                        </div>
                        
                        {/* Сетка карточек */}
                        {isLoading ? (
                            <div className="flex justify-center py-12">
                                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
                            </div>
                        ) : error ? (
                            <div className="text-center py-12 text-red-500">
                                Ошибка загрузки автомобилей
                            </div>
                        ) : assets.length === 0 ? (
                            <div className="text-center py-12 bg-white dark:bg-gray-800 rounded-xl">
                                <p className="text-gray-500">Автомобили не найдены</p>
                                <p className="text-sm text-gray-400 mt-2">
                                    Попробуйте изменить параметры поиска
                                </p>
                            </div>
                        ) : (
                            <div className="grid grid-cols-1 md:grid-cols-2 xl:grid-cols-3 gap-6">
                                {assets.map((asset, index) => (
                                    <AssetCard
                                        key={asset.id}
                                        asset={asset}
                                        index={index}
                                        onBookClick={openBookingModal}
                                    />
                                ))}
                            </div>
                        )}
                        
                        {/* Пагинация */}
                        {response && response.totalPages > 1 && (
                            <div className="flex justify-center gap-2 mt-8">
                                <button
                                    onClick={() => setFilters(prev => ({ ...prev, page: prev.page! - 1 }))}
                                    disabled={!response.hasPreviousPage}
                                    className="px-4 py-2 border rounded-lg disabled:opacity-50
                                             hover:bg-gray-100 dark:hover:bg-gray-700
                                             text-gray-700 dark:text-gray-300"
                                >
                                    ← Назад
                                </button>
                                <span className="px-4 py-2 text-gray-600 dark:text-gray-400">
                                    Страница {response.page} из {response.totalPages}
                                </span>
                                <button
                                    onClick={() => setFilters(prev => ({ ...prev, page: prev.page! + 1 }))}
                                    disabled={!response.hasNextPage}
                                    className="px-4 py-2 border rounded-lg disabled:opacity-50
                                             hover:bg-gray-100 dark:hover:bg-gray-700
                                             text-gray-700 dark:text-gray-300"
                                >
                                    Вперед →
                                </button>
                            </div>
                        )}
                    </div>
                </div>
            </div>
            
            {/* Модальное окно бронирования */}
            <BookingModal
                isOpen={isOpen}
                asset={selectedAsset}
                onClose={closeBookingModal}
            />
        </div>
    );
};