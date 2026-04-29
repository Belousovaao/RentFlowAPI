import { useI18n } from "@/app/providers/I18nProvider"
import { DateRangePicker } from "@/features/booking/components/DateRangePicker";
import { formatPrice } from "@/shared/utils/formatters";
import { useState } from "react";

export const BookingPannel = ({ asset }: any) => {
    const { t } = useI18n();
    const [startDate, setStartDate] = useState<Date | null>(null);
    const [endDate, setEndDate] = useState<Date | null>(null);
    
    return (
        <div className="space-y-4">
            {/* Цена за день */}
            <div className="flex justify-between items-baseline">
                <span className="text-gray-600 dark:text-gray-400">{t('booking.dailyRate')}</span>
                <div className="font-semibold text-gray-900 dark:text-white">
                    {formatPrice(asset.dailyPrice)} <span className="text-sm font-normal text-gray-500">{t('asset.perDay')}</span>
                </div>
            </div>

            {/* Залог */}
            {asset.deposit && (
                <div className="flex justify-between items-center py-2 border-t border-gray-100 dark:border-gray-700">
                    <span className="text-gray-600 dark:text-gray-400">{t('booking.deposit')}</span>
                    <span className="font-semibold text-gray-900 dark:text-white">
                        {formatPrice(asset.deposit)}
                    </span>
                </div>
            )}

            {/* Выбор дат */}
            <div className="py-2">
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
                    {t('booking.selectDates')}
                </label>
                <DateRangePicker
                    value={{ start: startDate, end: endDate }}
                    onChange={(range) => {
                        setStartDate(range.start);
                        setEndDate(range.end);
                    }}
                    minDate={new Date()}
                />
            </div>

            {(startDate && endDate) && (
                <div className="flex justify-between items-center pt-4 border-t border-gray-200 dark:border-gray-700">
                    <span className="text-gray-700 dark:text-gray-300 font-medium">{t('booking.total')}:</span>
                    <span className="text-2xl font-bold text-primary-600">
                        {formatPrice(asset.dailyPrice * Math.ceil((endDate.getTime() - startDate.getTime()) / (1000 * 60 * 60 * 24)))} ₽
                    </span>
                </div>
            )}

            {/* Кнопка бронирования */}
            <button 
                className="w-full py-3 rounded-xl bg-black text-white hover:bg-gray-800 hover:scale-[1.02] active:scale-[0.98] transition-all duration-200 font-medium"
                disabled={!startDate || !endDate}
            >
                {t('booking.bookNow')}
            </button>
        </div>
    );
};