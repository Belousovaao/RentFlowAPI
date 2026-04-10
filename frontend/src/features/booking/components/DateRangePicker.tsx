import { useEffect, useRef, useState } from 'react';
import { DayPicker, DateRange } from 'react-day-picker';
import { format } from 'date-fns';
import { useI18n } from '@/app/providers/I18nProvider';
import 'react-day-picker/dist/style.css';
import { formatDate } from '@/shared/utils/formatters';

interface DateRangePickerProps {
    value: { start: Date | null; end: Date | null };
    onChange: (range: { start: Date | null; end: Date | null }) => void;
    minDate?: Date;
}

export const DateRangePicker = ({ value, onChange, minDate }: DateRangePickerProps) => {
    const [isOpen, setIsOpen] = useState(false);
    const [calendarKey, setCalendarKey] = useState(0);

    const pickerRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (pickerRef.current && !pickerRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);
    
    const selectedRange: DateRange | undefined = value.start && value.end
        ? { from: value.start, to: value.end }
        : value.start
        ? { from: value.start, to: undefined }
        : undefined;

    const handleSelect = (range: DateRange | undefined) => {
        if (range?.from && range?.to && range.from.getTime() === range.to.getTime()) {
            onChange({
                start: range.from,
                end: null,
            });
            return;
        }

        onChange({
            start: range?.from || null,
            end: range?.to || null,
        });
        
        // Закрываем календарь после выбора обеих дат
        const isRangeValid = range?.from && range?.to && 
                         range.from.getTime() !== range.to.getTime();
    
        if (isRangeValid) {
            setIsOpen(false);
        }
    };

    const { t } = useI18n();

    const displayText = () => {
        if (value.start && value.end) {
            return `${formatDate(value.start)} - ${formatDate(value.end)}`;
        }
        if (value.start) {
            return `${formatDate(value.start)} - ?`;
        }
        return t('booking.selectDates');
    };

    return (
        <div ref={pickerRef} className="relative">
            <div
                onClick={() => setIsOpen(!isOpen)}
                className="w-full p-3 border border-gray-300 dark:border-gray-600 
                           rounded-lg cursor-pointer bg-white dark:bg-gray-800
                           hover:border-primary-500 transition-colors"
            >
                <div className="flex justify-between items-center">
                    <span className={!value.start ? 'text-gray-400' : 'text-gray-900 dark:text-white'}>
                        {displayText()}
                    </span>
                    <svg
                        className={`w-5 h-5 transition-transform ${isOpen ? 'rotate-180' : ''}`}
                        fill="none"
                        stroke="currentColor"
                        viewBox="0 0 24 24"
                    >
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                    </svg>
                </div>
            </div>
            
            {isOpen && (
                <div className="absolute top-full mt-2 z-50 bg-white dark:bg-gray-800 
                                rounded-lg shadow-xl border border-gray-200 dark:border-gray-700">
                    <DayPicker
                        key={calendarKey}
                        mode="range"
                        selected={selectedRange}
                        onSelect={handleSelect}
                        disabled={{ before: minDate || new Date() }}
                        className="p-4"
                        animate={false}
                    />
                </div>
            )}
        </div>
    );
};