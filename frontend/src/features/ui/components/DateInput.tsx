import { useEffect, useRef, useState, } from 'react';
import { createPortal } from 'react-dom';
import { CalendarDays } from 'lucide-react';
import { DayPicker } from 'react-day-picker';
import { useI18n } from '@/app/providers/I18nProvider';
import { formatDate, formatDateForBack, formatDateInput, parseDateForInput, parseDateInput, } from '@/shared/utils/formatters';
import 'react-day-picker/dist/style.css';
import './DateInput.css';

interface DateInputProps {
    value: string; // YYYY-MM-DD
    onChange: (value: string) => void;
    placeholder?: string;
    readOnly?: boolean;
    minDate?: Date;
    maxDate?: Date;
    className?: string;
    invalid?: boolean;
}

const MIN_YEAR = 1900;

const getInputValue = (value: string): string => {
        const date = parseDateForInput(value);

        return date
            ? formatDate(date)
            : '';
    };

interface CalendarPosition {
    top: number;
    left: number;
}

export const DateInput = ({
    value,
    onChange,
    placeholder = 'ДД.ММ.ГГГГ',
    readOnly = false,
    minDate,
    maxDate,
    className = '',
    invalid = false
}: DateInputProps) => {
    const { t } = useI18n();
    const [isOpen, setIsOpen] = useState(false);
    const [inputError, setInputError] = useState<string | null>(null);

    /*
     * Отдельное значение именно пользовательского ввода.
     * value: YYYY-MM-DD
     * inputValue: DD.MM.YYYY
     */
    const [inputValue, setInputValue] = useState(() => getInputValue(value));
    const [calendarPosition, setCalendarPosition] = useState<CalendarPosition | null>(null);
    const containerRef = useRef<HTMLDivElement>(null);
    const inputRef = useRef<HTMLInputElement>(null);
    const selectedDate = parseDateForInput(value);
    const today = new Date();
    const calendarStartMonth = minDate ?? new Date(MIN_YEAR, 0, 1);
    const calendarEndMonth = maxDate ?? new Date(today.getFullYear() + 10, 11, 31);
    /*
     * Синхронизируем текстовое представление,
     * если value изменился извне.
     */
    useEffect(() => {
        setInputValue(getInputValue(value));
    }, [value]);

    /*
     * Рассчитываем позицию календаря относительно input.
     */
    const updateCalendarPosition = () => {
        const input = inputRef.current;

        if (!input) {
            return;
        }

        const rect = input.getBoundingClientRect();
        setCalendarPosition({
            top: rect.bottom + 8,
            left: rect.left,
        });
    };

    /*
     * Click outside.
     */
    useEffect(() => {
        if (!isOpen) {
            return;
        }

        const handleMouseDown = (event: MouseEvent) => {
            const target = event.target as Node;
            const clickedInput = containerRef.current?.contains(target);
            const clickedCalendar = document.querySelector('.date-input-calendar')?.contains(target);

            if (!clickedInput && !clickedCalendar) {
                setIsOpen(false);
            }
        };

        document.addEventListener('mousedown', handleMouseDown);

        return () => {
            document.removeEventListener('mousedown', handleMouseDown);
        };
    }, [isOpen]);

    /*
     * При открытом календаре корректируем его положение
     * при scroll / resize.
     */
    useEffect(() => {
        if (!isOpen) {
            return;
        }

        updateCalendarPosition();

        const handleViewportChange = () => {
            updateCalendarPosition();
        };

        window.addEventListener('resize', handleViewportChange);
        window.addEventListener('scroll', handleViewportChange, true);

        return () => {
            window.removeEventListener('resize', handleViewportChange);
            window.removeEventListener('scroll', handleViewportChange, true);
        };
    }, [isOpen]);

    const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        if (readOnly) return;

        const rawValue = event.target.value;
        const digits = rawValue
            .replace(/\D/g, '')
            .slice(0, 8);
        const formatted = formatDateInput(digits);
        setInputValue(formatted);

        // Пока дата ещё не введена полностью —
        // это не ошибка.
        if (digits.length < 8) {
            setInputError(null);
            onChange('');
            return;
        }

        const day = Number(digits.slice(0, 2));
        const month = Number(digits.slice(2, 4));
        const year = Number(digits.slice(4, 8));

        // Базовые ограничения отдельных компонентов даты
        if (day < 1 || day > 31) {
            setInputError('Некорректный день');
            return;
        }

        if (month < 1 || month > 12) {
            setInputError('Некорректный месяц');
            return;
        }

        if (year < MIN_YEAR) {
            setInputError(`Год должен быть не раньше ${MIN_YEAR}`);
            return;
        }

        const date = parseDateInput(digits);

        // Например, 31.02.2018:
        // JS создаст другую дату, поэтому проверяем
        // результат обратно.
        if (!date) {
            setInputError('Некорректная дата');
            return;
        }

        if (minDate && date < minDate) {
            setInputError(`Дата не может быть раньше ${formatDate(minDate)}`);
            return;
        }

        if (maxDate && date > maxDate) {
            setInputError(`Дата не может быть позже ${formatDate(maxDate)}`);
            return;
        }

        // Всё корректно
        setInputError(null);
        onChange(formatDateForBack(date));

        requestAnimationFrame(() => {
            const input = inputRef.current;
            if (!input) return;
            input.setSelectionRange(formatted.length, formatted.length);
        });
    };

    const handleCalendarSelect = (date: Date | undefined) => {
        if (!date || readOnly) {
            return;
        }

        if (minDate && date < minDate) {
            return;
        }

        if (maxDate && date > maxDate) {
            return;
        }
        setInputError(null);
        setInputValue(formatDate(date));
        onChange(formatDateForBack(date));
        setIsOpen(false);
        requestAnimationFrame(() => {
            inputRef.current?.focus();
        });
    };

    const openCalendar = () => {
        if (readOnly) {
            return;
        }

        updateCalendarPosition();
        setIsOpen(true);
    };

    const toggleCalendar = () => {
        if (readOnly) {
            return;
        }

        if (isOpen) {
            setIsOpen(false);
            return;
        }

        updateCalendarPosition();
        setIsOpen(true);
    };

    return (
        <div
            ref={containerRef}
            className={`date-input relative ${inputError ? 'has-error' : ''}`}
        >
            <div className="relative">
                <input
                    ref={inputRef}
                    type="text"
                    inputMode="numeric"
                    autoComplete="off"
                    maxLength={10}
                    placeholder={placeholder}
                    value={inputValue}
                    readOnly={readOnly}
                    className={`${className} pl-3 ${
                        inputError || invalid
                            ? 'border-red-500 focus:border-red-500 focus:ring-red-500'
                            : ''
                    }`}
                    onChange={handleInputChange}
                    onFocus={openCalendar}
                />

                {!readOnly && (
                    <button
                        type="button"
                        tabIndex={-1}
                        aria-label={t('common.selectDate')}
                        onMouseDown={event => {event.preventDefault();}}
                        onClick={toggleCalendar}
                        className="absolute right-2 top-1/2 flex h-8 w-8 -translate-y-1/2 items-center justify-center rounded-md text-gray-400 transition-colors hover:bg-gray-100 hover:text-gray-600 dark:hover:bg-gray-700 dark:hover:text-gray-200"
                    >
                        <CalendarDays className="h-4 w-4" />
                    </button>
                )}
            </div>
            
            {inputError && (
                <p className="mt-1 text-xs text-red-500">
                    {inputError}
                </p>
            )}

            {isOpen && !readOnly && calendarPosition && createPortal(
                <div
                    className="date-input-calendar"
                    style={{
                        top: calendarPosition.top,
                        left: calendarPosition.left,
                    }}
                >
                    <DayPicker
                        mode="single"
                        selected={selectedDate}
                        onSelect={handleCalendarSelect}
                        defaultMonth={selectedDate ?? today}
                        startMonth={calendarStartMonth}
                        endMonth={calendarEndMonth}
                        captionLayout="dropdown"
                        disabled={[
                            ...(minDate
                                ? [{ before: minDate }]
                                : []),
                            ...(maxDate
                                ? [{ after: maxDate }]
                                : []),
                        ]}
                        className="p-3 text-gray-900 dark:text-white"
                        animate={false}
                    />
                </div>,
                document.body
            )}
        </div>
    );
};