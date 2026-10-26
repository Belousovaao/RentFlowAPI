import { useEffect, useRef, useState } from 'react';
import { DayPicker } from 'react-day-picker';
import { useI18n } from '@/app/providers/I18nProvider';
import 'react-day-picker/dist/style.css';
import './DateRangePicker.css'
import { formatDate } from '@/shared/utils/formatters';
import startOfDay from 'date-fns/esm/fp/startOfDay/index.js';
import { BusyPeriodDto } from '@/entities/booking/booking.hooks';

interface DateRangePickerProps {
    value: { start: Date | null; end: Date | null };
    onChange: (range: { start: Date | null; end: Date | null }) => void;
    busyPeriods?: BusyPeriodDto[];
}

export const DateRangePicker = ({
    value,
    onChange,
    busyPeriods
}: DateRangePickerProps) => {
    const [isOpen, setIsOpen] = useState(false);
    const pickerRef = useRef<HTMLDivElement>(null);
    const { t } = useI18n();
    const modifiersClassNames = 
    {
        rangeStart: 'date-range-start',
        rangeEnd: 'date-range-end',
        rangeMiddle: 'date-range-middle',
        rangeSingle: 'date-range-single',
    };

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (pickerRef.current && !pickerRef.current.contains(event.target as Node)) 
            {
                setIsOpen(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);

        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);
    
    const modifiers = {
        rangeStart:
            value.start && value.end
                ? value.start
                : undefined,

        rangeEnd:
            value.start && value.end
                ? value.end
                : undefined,

        rangeMiddle:
            value.start && value.end
                ? {
                    after: value.start,
                    before: value.end,
                }
                : undefined,

        rangeSingle:
            value.start && !value.end
                ? value.start
                : undefined,
    };


    const handleDayClick = (day: Date) => {
        // 1. Уже есть полный диапазон:
        // любой новый клик начинает новый выбор.
        if (value.start && value.end) {
            onChange({
                start: day,
                end: null,
            });

            return;
        }

        // 2. Начальная дата уже выбрана:
        // текущий клик становится конечной датой.
        if (value.start) {
            if (day.getTime() === value.start.getTime()) {
                return;
            }

            // Если пользователь кликнул раньше начала,
            // делаем эту дату новой начальной,
            // а старую дату — конечной.
            if (day < value.start) {
                onChange({
                    start: day,
                    end: value.start,
                });

                setIsOpen(false);
                return;
            }

            onChange({
                start: value.start,
                end: day,
            });

            setIsOpen(false);
            return;
        }

        // 3. Ничего не выбрано:
        // первый клик становится начальной датой.
        onChange({
            start: day,
            end: null,
        });
    };

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
                className="w-full p-3 border border-gray-300 dark:border-gray-600 rounded-lg cursor-pointer bg-white dark:bg-gray-800 hover:border-primary-500 transition-colors"
            >
                <div className="flex justify-between items-center">
                    <span className="text-gray-900 dark:text-white">
                        {displayText()}
                    </span>

                    <svg
                        className={`w-5 h-5 transition-transform ${
                            isOpen ? 'rotate-180' : ''
                        }`}
                        fill="none"
                        stroke="currentColor"
                        viewBox="0 0 24 24"
                    >
                        <path
                            strokeLinecap="round"
                            strokeLinejoin="round"
                            strokeWidth={2}
                            d="M19 9l-7 7-7-7"
                        />
                    </svg>
                </div>
            </div>

            {isOpen && (
                <div className="absolute z-50 top-full mt-2 bg-white dark:bg-gray-800 rounded-lg shadow-xl border border-gray-200 dark:border-gray-700">
                    <DayPicker
                        mode="single"
                        modifiers={modifiers}
                        modifiersClassNames={modifiersClassNames}
                        onDayClick={handleDayClick}
                        disabled={[
                            {
                                before: startOfDay(new Date()),
                            },

                            ...(busyPeriods ?? []).map(x => ({
                                from: new Date(x.startDate),
                                to: new Date(x.endDate),
                            })),
                        ]}
                        className="p-4 text-gray-900 dark:text-white"
                        animate={false}
                    />
                </div>
            )}
        </div>
    );
};