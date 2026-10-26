import { DateRangePicker } from '../../booking/components/DateRangePicker';
import { useI18n } from '@/app/providers/I18nProvider';

interface DateFilterProps {
    startDate: Date | null;
    endDate: Date | null;
    onChange: (start: Date | null, end: Date | null) => void;
}

export const DateFilter = ({ startDate, endDate, onChange }: DateFilterProps) => {
    const { t } = useI18n();
    return (
        <div className="bg-white dark:bg-gray-800 rounded-xl shadow-lg p-5 mb-6">
            <div className="flex items-center gap-4 flex-wrap">
                <div className="flex items-center gap-2">
                    <span className="text-gray-600 dark:text-gray-300">📅</span>
                    <span className="font-medium text-gray-700 dark:text-gray-300">
                        {t('booking.selectDates')}
                    </span>
                </div>
                <div className="w-80">
                    <DateRangePicker
                        value={{ start: startDate, end: endDate }}
                        onChange={(range) => onChange(range.start, range.end)}
                    />
                </div>
                {(startDate || endDate) && (
                    <button
                        onClick={() => onChange(null, null)}
                        className="text-sm text-gray-900 dark:text-gray-300 hover:text-primary-600 transition-colors"
                    >
                        {t('booking.reset')}
                    </button>
                )}
            </div>
        </div>
    );
};