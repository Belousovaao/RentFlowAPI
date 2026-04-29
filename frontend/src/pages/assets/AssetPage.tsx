import { useState } from 'react';
import { useAssets } from '../../entities/asset/asset.hooks';
import { AssetCard } from '../../features/assets/components/AssetCard';
import { AssetFilters } from '../../features/assets/components/AssetFilters';
import { DateFilter } from '../../features/assets/components/DateFilter';
import { AssetFilters as AssetFiltersType } from '../../entities/asset/asset.types';
import { useBookingModal } from '@/features/booking/hooks/useBookingModal';
import { BookingModal } from '../../features/booking/components/BookingModal';
import { SlidersHorizontal } from 'lucide-react';
import { useI18n } from '@/app/providers/I18nProvider';

export const AssetsPage = () => {
    const [isFiltersOpen, setIsFiltersOpen] = useState(false);
    const [filters, setFilters] = useState<AssetFiltersType>({
        page: 1,
        limit: 12,
        sortBy: 'popular'
    });

    const { t } = useI18n();
    
    const [dateRange, setDateRange] = useState<{ start: Date | null; end: Date | null }>({
        start: null,
        end: null
    });
    
    const { data: response, isLoading, error } = useAssets({
        ...filters,
        startDate: dateRange.start?.toISOString(),
        endDate: dateRange.end?.toISOString(),
    });
    
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
                
                {/* Фильтр по датам */}
                <DateFilter
                    startDate={dateRange.start}
                    endDate={dateRange.end}
                    onChange={handleDateChange}
                />
                
                <div className="mb-4">
                    <button
                        onClick={() => setIsFiltersOpen(prev => !prev)}
                        className="inline-flex items-center gap-2 px-4 py-2 rounded-lg border border-gray-300 dark:border-gray-600
                                   bg-white dark:bg-gray-800 text-sm font-medium text-gray-700 dark:text-gray-200
                                   hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
                    >
                        <SlidersHorizontal className="w-4 h-4" />
                        {isFiltersOpen ? t('asset.filter.hide') : t('asset.filter.show')}
                    </button>
                </div>

                <div className="flex flex-col xl:flex-row gap-4">
                    {/* Боковая панель с фильтрами */}
                    <div className={`${isFiltersOpen ? 'block' : 'hidden'} xl:block xl:w-72 2xl:w-80 flex-shrink-0`}>
                        <AssetFilters
                            filters={filters}
                            onFilterChange={setFilters}
                        />
                    </div>
                    
                    {/* Основной контент */}
                    <div className="min-w-0 flex-1">
                        {/* Сортировка */}
                        <div className="flex justify-between items-center mb-6">
                            <p className="text-sm text-gray-500">
                                {t('common.showed')} {assets.length} / {total}
                            </p>
                            <select
                                value={filters.sortBy || 'popular'}
                                onChange={(e) => handleSortChange(e.target.value)}
                                className="px-3 py-2 border border-gray-300 dark:border-gray-600 
                                           rounded-lg bg-white dark:bg-gray-800 text-sm
                                           text-gray-700 dark:text-gray-300"
                            >
                                <option value="popular">{t('asset.sort.popular')}</option>
                                <option value="price_asc">{t('asset.sort.priceAsc')}</option>
                                <option value="price_desc">{t('asset.sort.priceDesc')}</option>
                                <option value="year_desc">{t('asset.sort.new')}</option>
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
                                <p className="text-gray-500">{t('asset.notfound')}</p>
                                <p className="text-sm text-gray-400 mt-2">
                                    Попробуйте изменить параметры поиска
                                </p>
                            </div>
                        ) : (
                            <div className="grid grid-cols-1 sm:grid-cols-2 2xl:grid-cols-3 gap-4 lg:gap-5">
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
                                    ← {t('common.prev')}
                                </button>
                                <span className="px-4 py-2 text-gray-600 dark:text-gray-400">
                                    {t('common.page')} {response.page} / {response.totalPages}
                                </span>
                                <button
                                    onClick={() => setFilters(prev => ({ ...prev, page: prev.page! + 1 }))}
                                    disabled={!response.hasNextPage}
                                    className="px-4 py-2 border rounded-lg disabled:opacity-50
                                             hover:bg-gray-100 dark:hover:bg-gray-700
                                             text-gray-700 dark:text-gray-300"
                                >
                                    {t('coomon.next')} →
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