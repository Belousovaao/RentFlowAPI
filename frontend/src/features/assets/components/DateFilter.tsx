import { DateRangePicker } from '../../booking/components/DateRangePicker';

interface DateFilterProps {
    startDate: Date | null;
    endDate: Date | null;
    onChange: (start: Date | null, end: Date | null) => void;
}

export const DateFilter = ({ startDate, endDate, onChange }: DateFilterProps) => {
    return (
        <div className="bg-white dark:bg-gray-800 rounded-xl shadow-lg p-5 mb-6">
            <div className="flex items-center gap-4 flex-wrap">
                <div className="flex items-center gap-2">
                    <span className="text-gray-600 dark:text-gray-300">📅</span>
                    <span className="font-medium text-gray-700 dark:text-gray-300">
                        Период аренды:
                    </span>
                </div>
                <div className="w-80">
                    <DateRangePicker
                        value={{ start: startDate, end: endDate }}
                        onChange={(range) => onChange(range.start, range.end)}
                        minDate={new Date()}
                    />
                </div>
                {(startDate || endDate) && (
                    <button
                        onClick={() => onChange(null, null)}
                        className="text-sm text-gray-500 hover:text-primary-600 transition-colors"
                    >
                        Очистить
                    </button>
                )}
            </div>
        </div>
    );
};