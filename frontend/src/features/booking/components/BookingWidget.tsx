import { motion, AnimatePresence } from 'framer-motion';
import { useEffect, useRef, useState } from 'react';
import { useBookingStore } from '../stores/bookingStore';
import { useAuthStore } from '@/features/auth/stores/authStore';
import { DateRangePicker } from './DateRangePicker';
import { AssetSelector } from './AssetSelector';
import { calculateTotalPrice, calculateDays } from '../utils/priceCalculator';
import { Button } from '@/features/ui/components/Button';
import type { Asset } from '@/entities/asset/asset.types';
import { useI18n } from '@/app/providers/I18nProvider';
import { formatPrice } from '@/shared/utils/formatters';

interface DateRange {
    start: Date | null;
    end: Date | null;
}

interface BookingWidgetProps {
    presetAsset?: Asset | null; 
}

export const BookingWidget = ({ presetAsset }: BookingWidgetProps) => {
    const [selectedAsset, setSelectedAsset] = useState<Asset | null>(presetAsset || null);
    const [dateRange, setDateRange] = useState<DateRange>({ start: null, end: null });
    const { setBookingDetails } = useBookingStore();
    const { isAuthenticated } = useAuthStore();

    const widgetRef = useRef<HTMLDivElement>(null);
    
    const handleBookNow = () => {
        // Проверяем, что все данные есть и пользователь авторизован
        if (!isAuthenticated) {
            // Открыть модалку авторизации
            console.log('Please login first');
            return;
        }
        
        if (!selectedAsset || !dateRange.start || !dateRange.end) {
            console.log('Please select car and dates');
            return;
        }
        
        const totalPrice = calculateTotalPrice(
            selectedAsset.dailyPrice,
            dateRange.start,
            dateRange.end
        );
        
        setBookingDetails({
            asset: selectedAsset,
            startDate: dateRange.start,
            endDate: dateRange.end,
            totalPrice: totalPrice,
        });
        
        // Переход на страницу оформления
        window.location.href = '/checkout';
    };

    const handleReset = () => {
        setSelectedAsset(presetAsset || null);
        setDateRange({ start: null, end: null });
    };
    
    const isValid = selectedAsset !== null && dateRange.start !== null && dateRange.end !== null;
    
    // Безопасный расчет количества дней
    const daysCount = isValid 
        ? calculateDays(dateRange.start!, dateRange.end!)
        : 0;
    
    const totalPrice = isValid 
        ? calculateTotalPrice(selectedAsset!.dailyPrice, dateRange.start!, dateRange.end!)
        : 0;
    const { t } = useI18n();

    const getDaysLabel = (days: number): string => {
        const lastDigit = days % 10;
        const lastTwoDigits = days % 100;
        
        // 11-19 всегда "суток"
        if (lastTwoDigits >= 11 && lastTwoDigits <= 19) {
            return t('booking.days');
        }
        
        // 1, 21, 31, 41... -> "сутки"
        if (lastDigit === 1) {
            return t('booking.day1');
        }
        
        // 2,3,4, 22,23,24... -> "суток"
        return t('booking.days');
    };

    return (
        <motion.div
            ref={widgetRef}
            initial={{ opacity: 0, x: -50 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ delay: 0.3, type: 'spring' }}
            className="w-[400px]
                       bg-white/95 dark:bg-gray-900/95 backdrop-blur-xl 
                       rounded-2xl shadow-2xl p-6 border border-gray-200 
                       dark:border-gray-700 z-20"
        >
            <h2 className="text-2xl font-bold mb-6 bg-gradient-to-r 
                           from-primary-600 to-primary-400 bg-clip-text 
                           text-transparent">
                {t('booking.title')}
            </h2>
            
            <div className="space-y-4">
                <AssetSelector 
                    value={selectedAsset}
                    onChange={setSelectedAsset}
                />
                
                <DateRangePicker
                    value={dateRange}
                    onChange={setDateRange}
                    minDate={new Date()}
                />
            </div>
                
            {isValid && (
                    <div className="mt-4 p-4 bg-primary-50 dark:bg-primary-900/20 
                                    rounded-lg space-y-3">
                        <div className="flex justify-between text-sm">
                            <span className="text-gray-600 dark:text-gray-300">
                                {t('booking.dailyRate')}
                            </span>
                            <span className="font-semibold">
                            {formatPrice(selectedAsset!.dailyPrice)}
                            </span>
                        </div>
                        <div className="flex justify-between text-sm">
                            <span className="text-gray-600 dark:text-gray-300">
                                {t('booking.numberOfDays')}
                            </span>
                            <span className="font-semibold">
                                {daysCount} {getDaysLabel(daysCount)}
                            </span>
                        </div>
                        <div className="border-t border-gray-200 dark:border-gray-700 
                                        pt-3 mt-2">
                            <div className="flex justify-between font-bold">
                                <span>{t('booking.total')}</span>
                                <span className="text-xl text-primary-600">
                                    {formatPrice(totalPrice)}
                                </span>
                            </div>
                        </div>
                        
                        <div className="flex justify-center gap-4 mt-4">
                            <Button 
                                onClick={handleBookNow}
                                className="w-full mt-4"
                                size="md"
                                variant="gradient"
                            >
                                {t('booking.bookNow')}
                            </Button>
                            <Button 
                                onClick={handleReset}
                                className="w-full mt-4"
                                size="md"
                                variant="gradient"
                            >
                                {t('booking.reset')}
                            </Button>
                        </div>
                    </div>
            )}
        </motion.div>
    );
};