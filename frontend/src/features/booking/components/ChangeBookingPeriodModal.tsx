import { useEffect, useState } from 'react';
import { AnimatePresence, motion } from 'framer-motion';
import { CalendarDays, Check, ChevronDown, ChevronUp, Info, Loader2, X, } from 'lucide-react';
import { toast } from 'react-hot-toast';
import { DateRangePicker } from '@/features/booking/components/DateRangePicker';
import { bookingApi } from '@/entities/booking/booking.api';
import type { AdminBookingDetailsDto, ChangeBookingPeriodPreviewDto, RentalPriceSegmentDto, } from '@/entities/booking/booking.types';
import { useI18n } from '@/app/providers/I18nProvider';
import { formatDateForBack, getDaysLabel } from '@/shared/utils/formatters';
import { useBusyPeriods } from '@/entities/availability/hooks/useBusyPeriods';

interface ChangeBookingPeriodModalProps {
    isOpen: boolean;
    booking: AdminBookingDetailsDto;
    onClose: () => void;
    onSuccess: () => Promise<void> | void;
}

export const ChangeBookingPeriodModal = ({
    isOpen,
    booking,
    onClose,
    onSuccess,
}: ChangeBookingPeriodModalProps) => {
    const [startDate, setStartDate] = useState<Date | null>(null);
    const [endDate, setEndDate] = useState<Date | null>(null);
    const [preview, setPreview] = useState<ChangeBookingPeriodPreviewDto | null>(null);
    const [previewLoading, setPreviewLoading] = useState(false);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [showCurrentDetails, setShowCurrentDetails] = useState(false);
    const [showNewDetails, setShowNewDetails] = useState(false);
    const [showPenaltyInfo, setShowPenaltyInfo] = useState(false);
    const { t } = useI18n();
    const { data: busyPeriods = [] } = useBusyPeriods(booking.asset.id,booking.id);

    useEffect(() => {
        if (!isOpen) return;

        setStartDate(new Date(booking.rentalPeriod.startDate));
        setEndDate(new Date(booking.rentalPeriod.endDate));
        setPreview(null);
        setPreviewLoading(false);
        setSaving(false);
        setError(null);
        setShowCurrentDetails(false);
        setShowNewDetails(false);
        setShowPenaltyInfo(false);
    }, [isOpen, booking]);

    useEffect(() => {
        if (!isOpen || !startDate || !endDate) {
            return;
        }

        const currentStart = new Date(booking.rentalPeriod.startDate);
        const currentEnd = new Date(booking.rentalPeriod.endDate);
        const isSamePeriod =
            startDate.toDateString() === currentStart.toDateString() &&
            endDate.toDateString() === currentEnd.toDateString();

        if (isSamePeriod) {
            setPreview(null);
            setError(null);
            setShowCurrentDetails(false);
            setShowNewDetails(false);
            return;
        }

        const timer = window.setTimeout(async () => {
            try {
                setPreviewLoading(true);
                setPreview(null);
                setError(null);
                setShowNewDetails(false);

                const result = await bookingApi.previewChangePeriod(
                    booking.id,
                    formatDateForBack(startDate),
                    formatDateForBack(endDate)
                );

                setPreview(result);
            } 
            catch (err) {
                console.error('Failed to preview booking period change:', err);
                setError('Не удалось рассчитать стоимость нового периода.');
            } 
            finally {
                setPreviewLoading(false);
            }
        }, 400);

        return () => window.clearTimeout(timer);
    }, [
        isOpen,
        startDate,
        endDate,
        booking.id,
        booking.rentalPeriod.startDate,
        booking.rentalPeriod.endDate,
    ]);

    useEffect(() => {
        if (!isOpen) return;

        const html = document.documentElement;
        const body = document.body;
        const previousHtmlOverflow = html.style.overflow;
        const previousBodyOverflow = body.style.overflow;
        const previousBodyPaddingRight = body.style.paddingRight;
        const scrollbarWidth = window.innerWidth - html.clientWidth;
        html.style.overflow = 'hidden';
        body.style.overflow = 'hidden';

        if (scrollbarWidth > 0) {
            body.style.paddingRight = `${scrollbarWidth}px`;
        }

        return () => {
            html.style.overflow = previousHtmlOverflow;
            body.style.overflow = previousBodyOverflow;
            body.style.paddingRight = previousBodyPaddingRight;
        };
    }, [isOpen]);

    const handleChange = (range: {
        start: Date | null;
        end: Date | null;
    }) => {
        setStartDate(range.start);
        setEndDate(range.end);
    };

    const handleSave = async () => {
        if (!startDate || !endDate || !preview) {
            return;
        }

        try {
            setSaving(true);
            setError(null);

            await bookingApi.changePeriod(
                booking.id,
                formatDateForBack(startDate),
                formatDateForBack(endDate)
            );
            toast.success('Срок бронирования изменён.');
            await onSuccess();
            onClose();
        } catch (err) {
            console.error('Failed to change booking period:', err);
            toast.error('Не удалось изменить срок бронирования.');
        } finally {
            setSaving(false);
        }
    };

    const formatPrice = (value: number) => value.toLocaleString('ru-RU') + ' ₽';

    const getCalendarDays = (start: Date, end: Date) => {
        const startDay = new Date(
            start.getFullYear(),
            start.getMonth(),
            start.getDate()
        );

        const endDay = new Date(
            end.getFullYear(),
            end.getMonth(),
            end.getDate()
        );

        return Math.round((endDay.getTime() - startDay.getTime()) / (1000 * 60 * 60 * 24));
    };

    const selectedRentalDays = startDate && endDate
            ? getCalendarDays(startDate, endDate)
            : 0;

    const currentRentalDays = preview?.currentRentalDays ??
        getCalendarDays(
            new Date(booking.rentalPeriod.startDate),
            new Date(booking.rentalPeriod.endDate)
        );

    const renderPriceSegments = (segments: RentalPriceSegmentDto[]) => (
        <div className="mt-3 space-y-2 border-t border-gray-200 pt-3 dark:border-gray-700">
            {segments.map((segment, index) => (
                <div
                    key={index}
                    className="flex items-center justify-between gap-4 rounded-lg bg-gray-100 px-3 py-2 text-sm dark:bg-gray-800"
                >
                    <span className="text-gray-900 dark:text-white">
                        {formatPrice(segment.discountedDailyPrice)} ×{' '}
                        {segment.days}
                    </span>

                    <span className="shrink-0 font-medium text-gray-900 dark:text-white">
                        {formatPrice(segment.total)}
                    </span>
                </div>
            ))}
        </div>
    );

    if (!isOpen) {
        return null;
    }

    return (
        <AnimatePresence>
            <motion.div
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                exit={{ opacity: 0 }}
                className="fixed inset-0 z-50 flex items-start justify-center overflow-y-auto bg-black/50 px-4 py-6 backdrop-blur-sm"
                onClick={onClose}
            >
                <motion.div
                    initial={{ opacity: 0, y: 16, scale: 0.98 }}
                    animate={{ opacity: 1, y: 0, scale: 1 }}
                    exit={{ opacity: 0, y: 16, scale: 0.98 }}
                    transition={{
                        type: 'spring',
                        damping: 28,
                        stiffness: 320,
                    }}
                    onClick={(event) => event.stopPropagation()}
                    className="relative w-full max-w-xl overflow-visible rounded-2xl bg-white shadow-2xl dark:bg-gray-800"
                >
                    {/* Header */}
                    <div className="flex items-center justify-between border-b border-gray-200 px-6 py-4 dark:border-gray-700">
                        <div className="flex items-center gap-3">
                            <CalendarDays className="h-5 w-5 dark:text-white" />
                            <h2 className="text-lg font-semibold dark:text-white">
                                Изменить срок аренды
                            </h2>
                        </div>

                        <button
                            type="button"
                            onClick={onClose}
                            disabled={saving}
                            className="rounded-full p-2 transition-colors hover:bg-gray-100 disabled:opacity-40 dark:hover:bg-gray-700"
                            aria-label="Закрыть"
                        >
                            <X className="h-5 w-5" />
                        </button>
                    </div>

                    <div className="space-y-5 p-6">
                        {/* Period comparison */}
                        <div className="grid grid-cols-2 gap-3">
                            {/* Current */}
                            <div className="rounded-xl border border-gray-200 bg-gray-50 p-4 dark:border-gray-700 dark:bg-gray-700/50">
                                <p className="text-xs font-medium uppercase tracking-wide text-gray-600 dark:text-gray-200">
                                    Текущий период
                                </p>

                                <p className="mt-2 font-semibold dark:text-gray-200">
                                    {new Date(booking.rentalPeriod.startDate).toLocaleDateString('ru-RU')}
                                    {' — '}
                                    {new Date(booking.rentalPeriod.endDate).toLocaleDateString('ru-RU')}
                                </p>

                                <p className="mt-1 text-sm text-gray-700 dark:text-gray-300">
                                    {currentRentalDays}{' '}
                                    {getDaysLabel(currentRentalDays, t)}
                                </p>

                                {preview && (
                                    <div className="mt-3 text-sm font-semibold text-gray-800 dark:text-gray-200">
                                        {formatPrice(preview.currentRentalPrice)}
                                    </div>
                                )}
                            </div>

                            {/* New */}
                            <div className="rounded-xl border border-primary-200 bg-primary-50/50 p-4 dark:border-primary-800 dark:bg-primary-900/20">
                                <p className="text-xs font-medium uppercase tracking-wide text-primary-600 dark:text-primary-400">
                                    Новый период
                                </p>

                                <p className="mt-2 font-semibold text-gray-800 dark:text-gray-200">
                                    {startDate
                                        ? startDate.toLocaleDateString('ru-RU')
                                        : '—'}
                                    {' — '}
                                    {endDate
                                        ? endDate.toLocaleDateString('ru-RU')
                                        : '—'}
                                </p>

                                <p className="mt-1 text-sm text-gray-700 dark:text-gray-300">
                                    {selectedRentalDays}{' '}
                                    {getDaysLabel(selectedRentalDays, t)}
                                </p>
                            </div>
                        </div>

                        {/* Calendar */}
                        <div>
                            <DateRangePicker
                                value={{
                                    start: startDate,
                                    end: endDate,
                                }}
                                onChange={handleChange}
                                busyPeriods={busyPeriods}
                            />
                        </div>

                        {/* Loading */}
                        {previewLoading && (
                            <div className="flex items-center gap-2 rounded-xl bg-gray-50 p-4 text-sm text-gray-700 dark:bg-gray-700/50 dark:text-gray-300">
                                <Loader2 className="h-4 w-4 animate-spin" />
                                Рассчитываем новую стоимость...
                            </div>
                        )}

                        {/* Error */}
                        {error && !previewLoading && (
                            <div className="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700 dark:border-red-900 dark:bg-red-950/30 dark:text-red-400">
                                {error}
                            </div>
                        )}

                        {/* Financial calculation */}
                        {preview && !previewLoading && (
                            <div className="rounded-xl border border-gray-200 bg-gray-50 p-5 dark:border-gray-700 dark:bg-gray-700/50">
                                <div className="mb-4 flex items-center justify-between">
                                    <h3 className="font-semibold dark:text-white">
                                        Расчёт стоимости
                                    </h3>
                                </div>

                                <div className="space-y-3">
                                    {/* Current details */}
                                    <div>
                                        <button
                                            type="button"
                                            onClick={() => setShowCurrentDetails((value) => !value)}
                                            className="flex w-full items-center justify-between gap-4 text-left"
                                        >
                                            <span className="text-sm text-gray-800 dark:text-gray-200">
                                                Текущая арендная плата
                                            </span>

                                            <span className="flex shrink-0 items-center gap-2 font-semibold text-gay-800 dark:text-gray-200">
                                                {formatPrice(preview.currentRentalPrice)}

                                                {showCurrentDetails ? (
                                                    <ChevronUp className="h-4 w-4 text-gray-400" />
                                                ) : (
                                                    <ChevronDown className="h-4 w-4 text-gray-400" />
                                                )}
                                            </span>
                                        </button>

                                        <AnimatePresence initial={false}>
                                            {showCurrentDetails && (
                                                <motion.div
                                                    initial={{
                                                        height: 0,
                                                        opacity: 0,
                                                    }}
                                                    animate={{
                                                        height: 'auto',
                                                        opacity: 1,
                                                    }}
                                                    exit={{
                                                        height: 0,
                                                        opacity: 0,
                                                    }}
                                                    className="overflow-hidden"
                                                >
                                                    {renderPriceSegments(preview.currentSegments)}
                                                </motion.div>
                                            )}
                                        </AnimatePresence>
                                    </div>

                                    {/* New rental price */}
                                    <div>
                                        <button
                                            type="button"
                                            onClick={() => setShowNewDetails((value) => !value)}
                                            className="flex w-full items-center justify-between gap-4 text-left"
                                        >
                                            <span className="text-sm text-gray-800 dark:text-gray-200">
                                                {preview.isExtension
                                                    ? 'Арендная плата за период продления'
                                                    : 'Арендная плата за новый период'}
                                            </span>

                                            <span className="flex shrink-0 items-center gap-2 font-semibold text-gray-800 dark:text-gray-200">
                                                {formatPrice(preview.newRentalPrice)}
                                                {showNewDetails ? (
                                                    <ChevronUp className="h-4 w-4 text-gray-400" />
                                                ) : (
                                                    <ChevronDown className="h-4 w-4 text-gray-400" />
                                                )}
                                            </span>
                                        </button>

                                        <AnimatePresence initial={false}>
                                            {showNewDetails && (
                                                <motion.div
                                                    initial={{
                                                        height: 0,
                                                        opacity: 0,
                                                    }}
                                                    animate={{
                                                        height: 'auto',
                                                        opacity: 1,
                                                    }}
                                                    exit={{
                                                        height: 0,
                                                        opacity: 0,
                                                    }}
                                                    className="overflow-hidden"
                                                >
                                                    {renderPriceSegments(preview.changedSegments)}
                                                </motion.div>
                                            )}
                                        </AnimatePresence>
                                    </div>

                                    {/* Penalty */}
                                    {preview.penalty > 0 && (
                                        <div className="flex items-center justify-between gap-4 border-t border-gray-200 pt-3 dark:border-gray-700">
                                            <div className="relative min-w-0">
                                                <div className="flex items-center gap-1.5">
                                                    <span className="text-sm text-gray-800 dark:text-gray-200">
                                                        Штраф за сокращение срока
                                                    </span>

                                                    <button
                                                        type="button"
                                                        onClick={() => setShowPenaltyInfo((value) => !value)}
                                                        className="inline-flex h-5 w-5 shrink-0 items-center justify-center rounded-full text-gray-400 transition-colors hover:bg-gray-200 hover:text-gray-600 dark:hover:bg-gray-700 dark:hover:text-gray-200"
                                                        aria-label="Информация о штрафе"
                                                    >
                                                        <Info className="h-4 w-4" />
                                                    </button>
                                                </div>

                                                <AnimatePresence>
                                                    {showPenaltyInfo && (
                                                        <motion.div
                                                            initial={{
                                                                opacity: 0,
                                                                y: -4,
                                                            }}
                                                            animate={{
                                                                opacity: 1,
                                                                y: 0,
                                                            }}
                                                            exit={{
                                                                opacity: 0,
                                                                y: -4,
                                                            }}
                                                            className="absolute left-0 top-7 z-20 w-72 rounded-xl border border-gray-200 bg-white p-4 text-sm text-gray-700 shadow-xl dark:border-gray-700 dark:bg-gray-800 dark:text-gray-300"
                                                        >
                                                            <p className="font-medium text-gray-900 dark:text-gray-100">
                                                                Штраф за сокращение срока
                                                            </p>

                                                            <p className="mt-2 leading-relaxed">
                                                                Согласно п. 4.12 Договора аренды, при сокращении срока аренды применяется штраф в размере стоимости 1 суток аренды.
                                                            </p>
                                                        </motion.div>
                                                    )}
                                                </AnimatePresence>
                                            </div>

                                            <span className="shrink-0 font-semibold text-red-500">
                                                {formatPrice(preview.penalty)}
                                            </span>
                                        </div>
                                    )}

                                    {/* Result */}
                                    {(preview.isRefund ||
                                        preview.isAdditionalPayment) && (
                                        <div className="mt-4 flex items-center justify-between gap-4 border-t border-gray-300 pt-4 dark:border-gray-600">
                                            <div>
                                                <p className="text-base font-semibold text-gray-800 dark:text-white">
                                                    {preview.isRefund
                                                        ? 'К возврату'
                                                        : 'Доплата'}
                                                </p>
                                            </div>

                                            <span className="shrink-0 text-xl font-bold text-gray-800 dark:text-white">
                                                {formatPrice(Math.abs(preview.difference))}
                                            </span>
                                        </div>
                                    )}

                                    {!preview.isRefund &&
                                        !preview.isAdditionalPayment && (
                                            <div className="mt-4 border-t border-gray-300 pt-4 dark:border-gray-600">
                                                <div className="flex items-center justify-between">
                                                    <span className="text-base font-semibold text-gray-800 dark:text-gray-200">
                                                        Стоимость не изменилась
                                                    </span>

                                                    <span className="font-semibold text-gray-800 dark:text-gray-200">
                                                        {formatPrice(preview.newRentalPrice)}
                                                    </span>
                                                </div>
                                            </div>
                                        )}
                                </div>
                            </div>
                        )}

                        {/* Actions */}
                        <div className="flex justify-end gap-3 pt-1">
                            <button
                                type="button"
                                onClick={onClose}
                                disabled={saving}
                                className="rounded-lg border border-gray-300 px-4 py-2 text-gray-700 dark:text-gray-300 transition-colors hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-40 dark:border-gray-600 dark:hover:bg-gray-700"
                            >
                                Отмена
                            </button>

                            <button
                                type="button"
                                onClick={handleSave}
                                disabled={
                                    saving ||
                                    previewLoading ||
                                    !startDate ||
                                    !endDate ||
                                    !preview
                                }
                                className="flex items-center gap-2 rounded-lg bg-primary-600 px-4 py-2 text-white transition-colors hover:bg-primary-700 disabled:cursor-not-allowed disabled:opacity-40"
                            >
                                {saving ? (
                                    <>
                                        <Loader2 className="h-4 w-4 animate-spin" />
                                        Сохраняем...
                                    </>
                                ) : (
                                    <>
                                        <Check className="h-4 w-4" />
                                        Подтвердить изменение
                                    </>
                                )}
                            </button>
                        </div>
                    </div>
                </motion.div>
            </motion.div>
        </AnimatePresence>
    );
};