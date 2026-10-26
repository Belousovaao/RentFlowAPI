import { useI18n } from "@/app/providers/I18nProvider"
import { DateRangePicker } from "@/features/booking/components/DateRangePicker";
import { formatDateForBack, formatPrice } from "@/shared/utils/formatters";
import { useEffect, useState } from "react";
import { useAuthStore } from '@/features/auth/stores/authStore';
import { useAuthModalStore } from '@/features/auth/stores/authModalStore';
import { useBookingStore } from '@/features/booking/stores/bookingStore';
import { useNavigate } from 'react-router-dom';
import type { Asset } from '@/entities/asset/asset.types';
import { useBusyPeriods } from '@/entities/availability/hooks/useBusyPeriods';
import { BookingPriceDto } from "@/entities/booking/booking.types";
import { bookingApi } from "@/entities/booking/booking.api";
import { Button } from "@/features/ui/components/Button";
import { DestinationType } from "@/entities/destination/destination.type";

export const BookingPannel = ({ asset }: { asset: Asset }) => {
    const { t } = useI18n();
    const navigate = useNavigate();
    const [startDate, setStartDate] = useState<Date | null>(null);
    const [endDate, setEndDate] = useState<Date | null>(null);
    const { data: busyPeriods = [] } = useBusyPeriods(asset.id);
    const { isAuthenticated } = useAuthStore();
    const { open: openAuthModal } = useAuthModalStore();
    const { setBookingDetails } = useBookingStore();
    const [price, setPrice] = useState<BookingPriceDto | null>(null);
    const [calculating, setCalculating] = useState(false);

    const isValid = startDate && endDate;
    const totalToPay = price?.totalToPay ?? 0;

    useEffect(() => {
        if (!isValid || !startDate || !endDate) {
            setPrice(null);
            return;
        }

        const calculate = async () => {setCalculating(true);
            try {
                const response = await bookingApi.calculatePrice({
                    assetId: asset.id,
                    pickupDate: formatDateForBack(startDate),
                    returnDate: formatDateForBack(endDate),
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
    }, [asset.id, startDate, endDate, isValid]);
    
    const proceedToCheckout = () => {
        if (!startDate || !endDate) return;
        setBookingDetails({
            asset,
            startDate,
            endDate
        });
        navigate('/checkout');
    };

    const handleBook = () => {
        if (!isValid) 
        {
            return;
        }
        if (!isAuthenticated) {
            openAuthModal(proceedToCheckout);
            return;
        }
        proceedToCheckout();
    };

    const rentalSegments = price?.rentalPrice.segments ?? [];
    const currentSegment = rentalSegments[rentalSegments.length - 1];
    
    return (
        <div className="space-y-4">
            <div className="py-2">
                <DateRangePicker
                    value={{ start: startDate, end: endDate }}
                    onChange={(range) => {
                        setStartDate(range.start);
                        setEndDate(range.end);
                    }}
                    busyPeriods={busyPeriods}
                />
            </div>

            {isValid && price && (
                <div className="flex justify-between items-baseline">
                    <span className="text-black dark:text-white">
                        {t('booking.dailyRate')}
                    </span>

                    <div className="font-semibold text-black dark:text-white">
                        {currentSegment.durationDiscountPercent > 0 && (
                            <span className="line-through text-gray-400 mr-2">
                                {formatPrice(currentSegment.baseDailyPrice)}
                            </span>
                        )}

                        {formatPrice(currentSegment.discountedDailyPrice)}
                    </div>
                </div>
            )}

            {asset.deposit && (
                <div className="flex justify-between items-center py-2 border-t border-gray-100 dark:border-gray-700">
                    <span className="text-black dark:text-white">{t('booking.deposit')}</span>
                    <span className="font-semibold text-black dark:text-white">
                        {formatPrice(asset.deposit)}
                    </span>
                </div>
            )}

            {isValid && (
                <div className="flex justify-between items-center pt-4 border-t border-gray-200 dark:border-gray-700">
                    <span className="text-black dark:text-white font-medium">{t('booking.total')}:</span>
                    <span className="text-2xl font-bold text-primary-600">
                        {formatPrice(totalToPay)}
                    </span>
                </div>
            )}

            <Button 
                variant="gradient"
                onClick={handleBook}
                className="w-full px-3 py-2 text-center rounded-xl bg-primary-600 border border-primary-600 text-white rounded-lg hover:bg-primary-50 dark:hover:bg-primary-900/20 transition-colors font-medium text-xl sm:text-xl whitespace-nowrap"
                disabled={!isValid}
            >
                {t('booking.bookNow')}
            </Button>
        </div>
    );
};
