import { motion } from 'framer-motion';
import { useEffect, useRef, useState } from 'react';
import { useBookingStore } from '../stores/bookingStore';
import { DateRangePicker } from './DateRangePicker';
import { AssetSelector } from './AssetSelector';
import { calculateDays } from '../utils/priceCalculator';
import { Button } from '@/features/ui/components/Button';
import type { Asset } from '@/entities/asset/asset.types';
import { useI18n } from '@/app/providers/I18nProvider';
import { formatDateForBack, formatPrice } from '@/shared/utils/formatters';
import { useNavigate } from 'react-router-dom';
import { getDaysLabel } from '@/shared/utils/formatters';
import { useBusyPeriods } from '@/entities/availability/hooks/useBusyPeriods';
import { useAvailableAssets } from '@/entities/availability/hooks/useAvailableAssets';
import { useAssets } from '@/entities/asset/asset.hooks';
import { BookingPriceDto } from '@/entities/booking/booking.types';
import { bookingApi } from '@/entities/booking/booking.api';
import { DestinationType } from '@/entities/destination/destination.type';

interface DateRange {
    start: Date | null;
    end: Date | null;
}

interface BookingWidgetProps {
    presetAsset?: Asset | null; 
}

export const BookingWidget = ({ presetAsset, }: BookingWidgetProps) => {
    const [selectedAsset, setSelectedAsset] = useState<Asset | null>(presetAsset || null);
    const [dateRange, setDateRange] = useState<DateRange>({ start: null, end: null });
    const { data: busyPeriods = [] } = useBusyPeriods(selectedAsset?.id);
    const { setBookingDetails } = useBookingStore();
    const [price, setPrice] = useState<BookingPriceDto | null>(null);
    const [calculating, setCalculating] = useState(false); 
    const navigate = useNavigate();

    const widgetRef = useRef<HTMLDivElement>(null);
    
    const proceedToCheckout = () => 
    {
        if (!selectedAsset || !dateRange.start || !dateRange.end)
        {
            return;
        } 
        setBookingDetails({
            asset: selectedAsset,
            startDate: dateRange.start,
            endDate: dateRange.end
        });
        console.log("Go to checkout");
        navigate('/checkout');
    };

    const { data: availableAssets = [], isLoading } = useAvailableAssets
        (
            dateRange.start,
            dateRange.end
        );

    const { data: allAssetsResponse } = useAssets();

    const assetsForResponse = dateRange.start && dateRange.end
        ? availableAssets
        : (allAssetsResponse?.data ?? []);

    const handleReset = () => {
        setSelectedAsset(presetAsset || null);
        setDateRange({ start: null, end: null });
    };

    const isValid = selectedAsset !== null && dateRange.start !== null && dateRange.end !== null;

    useEffect(() => {
        if (!selectedAsset || !dateRange.start || !dateRange.end) {
            setPrice(null);
            return;
        }

        const calculate = async () => { setCalculating(true);
            try {
                const response = await bookingApi.calculatePrice({
                    assetId: selectedAsset.id,
                    pickupDate: formatDateForBack(dateRange.start!),
                    returnDate: formatDateForBack(dateRange.end!),
                    services: [],
                    destinationType: DestinationType.Local,
                    destinationId: null,
                    customRouteDescription: null
                });
                setPrice(response.price);
            } catch (error) {
                console.error('Failed to calculate booking price', error);
                setPrice(null);
            } finally {
                setCalculating(false);
            }
        };

        calculate();
    }, [selectedAsset?.id, dateRange.start, dateRange.end, isValid]);
    
    // Безопасный расчет количества дней
    const daysCount = isValid 
        ? calculateDays(dateRange.start!, dateRange.end!)
        : 0;
    
    const totalPrice = price?.total ?? 0;
    const rentalPrice = price?.rentalPrice;
    const rentalSegments = rentalPrice?.segments ?? [];
    const lastRentalSegment = rentalSegments[rentalSegments.length - 1];
    const { t } = useI18n();

    return (
        <motion.div
            ref={widgetRef}
            initial={{ opacity: 0, x: -50 }}
            animate={{ opacity: 1, x: 0 }}
            transition={{ delay: 0.3, type: 'spring' }}
            className="w-[400px] bg-white/95 dark:bg-gray-900/95 backdrop-blur-xl rounded-2xl shadow-2xl p-6 border border-gray-200 dark:border-gray-700 z-20"
        >
            <h2 className="text-2xl font-bold mb-6 bg-gradient-to-r from-primary-600 to-primary-400 bg-clip-text text-transparent">
                {t('booking.title')}
            </h2>
            
            <div className="space-y-4">
                <AssetSelector 
                    assets = {assetsForResponse}
                    isLoading={isLoading}
                    value={selectedAsset}
                    onChange={setSelectedAsset}
                />
                
                <DateRangePicker
                    value={dateRange}
                    onChange={setDateRange}
                    busyPeriods={busyPeriods}
                />
            </div>
                
            {isValid && (
                    <div className="mt-4 p-4 bg-primary-50 dark:bg-primary-900/20 rounded-lg space-y-3">
                        <div className="flex justify-between text-sm">
                            <span className="text-gray-600 dark:text-gray-300">
                                {t('booking.numberOfDays')}
                            </span>
                            <span className="font-semibold text-gray-600 dark:text-gray-300">
                                {daysCount} {getDaysLabel(daysCount, t)}
                            </span>
                        </div>
                        {lastRentalSegment && (
                        <div className="flex justify-between text-sm">
                            <span className="text-gray-600 dark:text-gray-300">
                                {t('booking.dailyRate')}
                            </span>

                            <span className="font-semibold text-gray-600 dark:text-gray-300">
                                {lastRentalSegment.durationDiscountPercent > 0 ? (
                                    <>
                                        <span className="line-through mr-2 text-gray-400">
                                            {formatPrice(lastRentalSegment.baseDailyPrice)}
                                        </span>

                                        <span>
                                            {formatPrice(lastRentalSegment.discountedDailyPrice)}
                                        </span>
                                    </>
                                ) : (
                                    formatPrice(lastRentalSegment.baseDailyPrice)
                                )}
                            </span>
                        </div>
                    )}
                        <div className="border-t border-gray-200 dark:border-gray-700 pt-3 mt-2">
                            <div className="flex justify-between font-bold text-gray-600 dark:text-gray-300">
                                <span>
                                    {t('booking.total')}
                                </span>
                                <span className="text-xl text-primary-600">
                                    {formatPrice(totalPrice)}
                                </span>
                            </div>
                        </div>
                        
                        <div className="flex justify-center gap-4 mt-4">
                            <Button 
                                onClick={proceedToCheckout}
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