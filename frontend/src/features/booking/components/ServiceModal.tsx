import { useEffect, useState } from 'react';
import { AnimatePresence, motion } from 'framer-motion';
import { Check, Clock3, Loader2, Plus, X, } from 'lucide-react';
import { toast } from 'react-hot-toast';
import { bookingApi } from '@/entities/booking/booking.api';
import type { BookingServiceDto, BookingServiceRequest, BookingServicesChangePreviewDto, } from '@/entities/booking/booking.types';
import { AdditionalService, ServiceType, } from '@/entities/service/service.types';
import { useI18n } from '@/app/providers/I18nProvider';
import { formatPrice } from '@/shared/utils/formatters';
import { serviceApi } from '@/features/service/api/serviceApi';

interface ServiceModalProps {
    isOpen: boolean;
    bookingId: string;
    bookingServices: BookingServiceDto[];
    onClose: () => void;
    onSuccess: () => Promise<void> | void;
}

export const ServiceModal = ({
    isOpen,
    bookingId,
    bookingServices,
    onClose,
    onSuccess,
}: ServiceModalProps) => {
    const { t } = useI18n();
    const [availableServices, setAvailableServices] = useState<AdditionalService[]>([]);
    const [selectedServices, setSelectedServices] = useState<BookingServiceRequest[]>([]);
    const [serviceLoading, setServiceLoading] = useState(false);
    const [preview, setPreview] = useState<BookingServicesChangePreviewDto | null>(null);
    const [previewLoading, setPreviewLoading] = useState(false);
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [serviceTimes, setServiceTimes] = useState<Record<string, string>>({});

    useEffect(() => {
        if (!isOpen) 
        {
            return;
        }
        const loadServices = async () => {
            try 
            {
                setServiceLoading(true);
                setError(null);
                const result = await serviceApi.getAll();
                setAvailableServices(result.data);
                const initialTimes: Record<string, string> = {};
                result.data.forEach(service => {
                    if (requiresTime(service.type)) 
                    {
                        const defaultTime = getDefaultTime(service.type);
                        if (defaultTime) 
                            initialTimes[service.id] = defaultTime;
                    }
                });
                setServiceTimes(initialTimes);
            } 
            catch (err) 
            {
                console.error('Failed to load additional services:', err);
                setError(t('booking.servicesLoadError'));
            } 
            finally 
            {
                setServiceLoading(false);
            }
        };
        loadServices();
    }, [isOpen, t]);

    useEffect(() => {
        if (!isOpen)
            return;

        setSelectedServices(
            bookingServices.map(service => ({
                additionalServiceId: service.additionalServiceId,
                selectedTime: service.selectedTime,
            }))
        );
        setPreview(null);
        setError(null);
    }, [isOpen, bookingServices]);

    /*
     * Preview изменения.
     * Preview не нужен сразу после открытия:
     * пока selectedServices совпадает с bookingServices,
     * изменений нет.
     */
    useEffect(() => {
        if (!isOpen) {
            return;
        }
        const hasChanges = !isSameServices(
            bookingServices,
            selectedServices
        );

        if (!hasChanges) {
            setPreview(null);
            setPreviewLoading(false);
            setError(null);
            return;
        }
        const timer = window.setTimeout(async () => {
            try {
                setPreviewLoading(true);
                setPreview(null);
                setError(null);

                const result = await bookingApi.previewChangeServices(
                    bookingId,
                    selectedServices
                );
                setPreview(result);
            } 
            catch (err) 
            {
                console.error('Failed to preview booking services change:', err);
                setError(t('booking.servicesPreviewError'));
            } 
            finally 
            {
                setPreviewLoading(false);
            }
        }, 400);

        return () => 
            {window.clearTimeout(timer);};
    }, [
        isOpen,
        bookingId,
        bookingServices,
        selectedServices,
        t,
    ]);

    /*
     * Блокировка scroll страницы при открытой модалке.
     */
    useEffect(() => {
        if (!isOpen) {
            return;
        }
        const html = document.documentElement;
        const body = document.body;
        const previousHtmlOverflow = html.style.overflow;
        const previousBodyOverflow = body.style.overflow;
        const previousBodyPaddingRight = body.style.paddingRight;
        const scrollbarWidth = window.innerWidth - html.clientWidth;
        html.style.overflow = 'hidden';
        body.style.overflow = 'hidden';

        if (scrollbarWidth > 0) 
        {
            body.style.paddingRight =`${scrollbarWidth}px`;
        }

        return () => 
        {
            html.style.overflow = previousHtmlOverflow;
            body.style.overflow = previousBodyOverflow;
            body.style.paddingRight = previousBodyPaddingRight;
        };
    }, [isOpen]);

    /*
     * Добавление услуги.
     */
    const handleAddService = (service: AdditionalService) => 
    {
        setSelectedServices(prev => [
            ...prev,
            {
                additionalServiceId: service.id,
                selectedTime: requiresTime(service.type)
                    ? serviceTimes[service.id] ?? getDefaultTime(service.type)
                    : undefined,
            },
        ]);
    };

    /*
     * Удаление услуги.
     */
    const handleRemoveService = (serviceId: string
    ) => {
        setSelectedServices(prev =>
            prev.filter(
                service =>
                    service.additionalServiceId !==
                    serviceId
            )
        );
    };

    /*
     * Изменение времени услуги.
     */
    const handleTimeChange = (
        serviceId: string,
        time: string
    ) => {
        setSelectedServices(prev =>
            prev.map(service =>
                service.additionalServiceId ===
                serviceId
                    ? {
                        ...service,
                        selectedTime: time,
                    }
                    : service
            )
        );
    };

    /*
     * Сохранение.
     */
    const handleSave = async () => {
        if (!preview) {
            return;
        }

        try {
            setSaving(true);
            setError(null);
            await bookingApi.changeServices(
                bookingId,
                selectedServices
            );
            toast.success(t('booking.servicesChanged'));
            await onSuccess();
            onClose();
        } 
        catch (err) 
        {
            console.error('Failed to change booking services:', err);
            toast.error(t('booking.servicesChangeError'));
        } 
        finally 
        {
            setSaving(false);
        }
    };

    /*
     * Какие услуги уже выбраны.
     */
    const selectedServiceIds = new Set(selectedServices.map(service =>service.additionalServiceId));

    /*
     * Все услуги, которые ещё можно добавить.
     */
    const availableToAdd = availableServices.filter(service => !selectedServiceIds.has(service.id));

    const getServiceInfo = (serviceId: string ): AdditionalService | undefined => 
    { 
        return availableServices.find(service => service.id === serviceId );
    };

    /*
     * Изменения действительно есть?
     */
    const hasChanges = !isSameServices(
        bookingServices,
        selectedServices
    );

    /*
     * Не показываем саму модалку, если она закрыта.
     */
    if (!isOpen) {
        return null;
    }

    return (
        <AnimatePresence>
            <motion.div
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                exit={{ opacity: 0 }}
                className="fixed inset-0 z-50 flex items-center justify-center overflow-y-auto bg-black/50 px-4 py-6 backdrop-blur-sm"
                onClick={onClose}
            >
                <motion.div
                    initial={{
                        opacity: 0,
                        y: 16,
                        scale: 0.98,
                    }}
                    animate={{
                        opacity: 1,
                        y: 0,
                        scale: 1,
                    }}
                    exit={{
                        opacity: 0,
                        y: 16,
                        scale: 0.98,
                    }}
                    transition={{
                        type: 'spring',
                        damping: 28,
                        stiffness: 320,
                    }}
                    onClick={event => event.stopPropagation()}
                    className="relative w-full max-w-xl overflow-visible rounded-2xl bg-white shadow-2xl dark:bg-gray-800"
                >
                {/* Header */}
                <div
                    className="flex items-center justify-between border-b border-gray-200 px-6 py-4 dark:border-gray-700">
                        <div className="flex items-center gap-3">
                            <Plus className="h-5 w-5" />

                            <h2 className="text-lg font-semibold">
                                {t('booking.additionalServices')}
                            </h2>
                        </div>
                        <button
                            type="button"
                            onClick={onClose}
                            disabled={saving}
                            className="rounded-full p-2 transition-colors hover:bg-gray-100 disabled:opacity-40 dark:hover:bg-gray-700"
                            aria-label={t('common.close')}
                        >
                            <X className="h-5 w-5" />
                        </button>
                </div>

                <div
                    className="max-h-[calc(100vh-180px)] space-y-6 overflow-y-auto p-6">
                    {/* ========================= */}
                    {/* SELECTED SERVICES */}
                    {/* ========================= */}
                    <div>
                        <p className="mb-2 text-sm font-medium text-gray-500 dark:text-gray-400">
                            {t('booking.selectedServices')}
                        </p>

                        {selectedServices.length === 0 ? (
                            <div className="rounded-xl border border-dashed border-gray-300 bg-gray-50 px-4 py-5 text-center text-sm text-gray-500 dark:border-gray-600 dark:bg-gray-900 dark:text-gray-400">
                                {t('booking.noServicesSelected')}
                            </div>
                        ) : (
                            <div className="divide-y divide-gray-200 rounded-xl border border-gray-200 bg-gray-50 dark:divide-gray-700 dark:border-gray-700 dark:bg-gray-900">
                                {selectedServices.map(selected => {
                                    const bookingService = bookingServices.find(service =>
                                        service.additionalServiceId === selected.additionalServiceId
                                    );
                                    const service = getServiceInfo(selected.additionalServiceId);
                                    if (!service)
                                        return null;
                                    const calculated = preview?.services.find(item =>
                                        item.additionalServiceId === selected.additionalServiceId
                                    );
                                    const price = calculated?.price ?? bookingService?.price ?? service.price;
                                    return (
                                        <div
                                            key={service.id}
                                            className="flex items-center gap-4 px-4 py-3 first:rounded-t-xl last:rounded-b-xl"
                                        >
                                            {/* Service name + time */}
                                            <div className="min-w-0 flex-1">
                                                <p className="truncate font-medium text-gray-900 dark:text-white">
                                                    {service.name}
                                                </p>

                                                {selected.selectedTime && (
                                                    <p className="mt-0.5 text-sm text-gray-500 dark:text-gray-400">
                                                        {selected.selectedTime}
                                                    </p>
                                                )}
                                            </div>

                                            {/* Price */}
                                            <span className="shrink-0 font-medium text-primary-600">
                                                {formatPrice(price)}
                                            </span>

                                            {/* Remove */}
                                            <button
                                                type="button"
                                                onClick={() =>
                                                    handleRemoveService(
                                                        selected.additionalServiceId
                                                    )
                                                }
                                                disabled={saving}
                                                className="shrink-0 rounded-full p-1.5 text-gray-400 transition-colors hover:bg-red-50 hover:text-red-500 disabled:cursor-not-allowed disabled:opacity-40 dark:hover:bg-red-950/30"
                                                aria-label={t('booking.removeService')}
                                            >
                                                <X className="h-4 w-4" />
                                            </button>
                                        </div>
                                    );
                                })}
                            </div>
                        )}
                    </div>

                    {/* ========================= */}
                    {/* ADD SERVICES */}
                    {/* ========================= */}

                    <div>
                        <p className="mb-2 text-sm font-medium text-gray-500 dark:text-gray-400">
                            {t('booking.addService')}
                        </p>

                        {availableToAdd.length === 0 ? (
                            <div
                                className="rounded-xl border border-dashed border-gray-300 bg-gray-50 px-4 py-5 text-center text-sm text-gray-500 dark:border-gray-600 dark:bg-gray-900 dark:text-gray-400"
                            >
                                {t('booking.allServicesSelected')}
                            </div>
                        ) : (
                            <div className="space-y-2">
                                {availableToAdd.map(service => {
                                    const dynamicPrice = requiresTime(service.type);

                                    return (
                                        <div
                                            key={service.id}
                                            className="flex items-center justify-between gap-4 rounded-xl border border-gray-200 bg-white px-4 py-3 transition-colors hover:border-primary-400 hover:bg-primary-50/50 dark:border-gray-700 dark:bg-gray-800 dark:hover:border-primary-500 dark:hover:bg-primary-900/10"
                                        >
                                            <div className="min-w-0">
                                                <p className="font-medium text-gray-900 dark:text-white">
                                                    {service.name}
                                                </p>

                                                {dynamicPrice && (
                                                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                                                        {t('booking.priceDependsOnTime')}
                                                    </p>
                                                )}
                                            </div>

                                            <div className="flex shrink-0 items-center gap-3">
                                                {dynamicPrice ? (
                                                    <select
                                                        value={
                                                            serviceTimes[service.id] ??
                                                            getDefaultTime(service.type) ??
                                                            ''
                                                        }
                                                        onChange={event =>
                                                            setServiceTimes(prev => ({
                                                                ...prev,
                                                                [service.id]: event.target.value,
                                                            }))
                                                        }
                                                        disabled={saving}
                                                        className="rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm text-gray-900 outline-none transition-colors focus:border-primary-500 focus:ring-2 focus:ring-primary-500/20 dark:border-gray-600 dark:bg-gray-700 dark:text-white"
                                                    >
                                                        {generateTimeOptions(service.type).map(time => (
                                                            <option key={time} value={time}>
                                                                {time}
                                                            </option>
                                                        ))}
                                                    </select>
                                                ) : (
                                                    <span className="font-medium text-primary-600">
                                                        {formatPrice(service.price)}
                                                    </span>
                                                )}

                                                <button
                                                    type="button"
                                                    onClick={() => handleAddService(service)}
                                                    disabled={saving}
                                                    className="flex h-8 w-8 shrink-0 items-center justify-center rounded-full bg-gray-100 text-gray-500 transition-colors hover:bg-primary-100 hover:text-primary-600 disabled:cursor-not-allowed disabled:opacity-40 dark:bg-gray-700 dark:text-gray-300 dark:hover:bg-primary-900/30 dark:hover:text-primary-400"
                                                    aria-label={t('booking.addService')}
                                                >
                                                    <Plus className="h-4 w-4" />
                                                </button>
                                            </div>
                                        </div>
                                    );
                                })}
                            </div>
                        )}
                    </div>

                    {/* ========================= */}
                    {/* PREVIEW LOADING */}
                    {/* ========================= */}
                    {previewLoading && (
                        <div
                            className="flex items-center gap-2 rounded-lg bg-gray-50 p-4 text-sm text-gray-500 dark:bg-gray-900 dark:text-gray-400">
                            <Loader2 className="h-4 w-4 animate-spin" />
                            {t('booking.calculatingServices')}
                        </div>
                    )}

                    {/* ========================= */}
                    {/* ERROR */}
                    {/* ========================= */}
                    {error && !previewLoading && (
                        <div
                            className="rounded-lg border border-red-200 bg-red-50 p-4 text-sm text-red-700 dark:border-red-900 dark:bg-red-950/30 dark:text-red-400">
                            {error}
                        </div>
                    )}

                    {/* ========================= */}
                    {/* PRICE PREVIEW */}
                    {/* ========================= */}
                    {preview && !previewLoading && hasChanges && (
                        <div
                            className="rounded-xl border border-gray-200 bg-gray-50 p-5 dark:border-gray-700 dark:bg-gray-900">
                            <p className="mb-5 text-base font-semibold">
                                {t('booking.newPriceCalculation')}
                            </p>

                            <div className="space-y-4">
                                {/* Services current */}
                                <div
                                    className="flex items-center justify-between">
                                    <span
                                        className="text-sm text-gray-500 dark:text-gray-400">
                                        {t('booking.servicesPriceCurrent')}
                                    </span>
                                    <span className="font-medium">
                                        {formatPrice(preview.currentServicesPrice)}
                                    </span>
                                </div>
                                {/* Services new */}
                                <div
                                    className="flex items-center justify-between">
                                    <span
                                        className="text-sm text-gray-500 dark:text-gray-400">
                                        {t('booking.servicesPriceNew')}
                                    </span>
                                    <span className="font-medium">
                                        {formatPrice(preview.newServicesPrice)}
                                    </span>
                                </div>
                                {/* Difference */}
                                <div
                                    className="border-t border-gray-200 pt-4 dark:border-gray-700">
                                    <div
                                        className="flex items-center justify-between">
                                        <span className="text-base font-semibold">
                                            {preview.difference < 0 
                                                ? t('booking.refund')
                                                : preview.difference > 0
                                                ? t('booking.additionalPayment')
                                                : t('booking.noPriceChange')}
                                        </span>
                                        <span className="text-xl font-bold">
                                            {formatPrice(Math.abs(preview.difference))}
                                        </span>
                                    </div>
                                </div>
                                {/* Totals */}
                                <div
                                    className="border-t border-gray-200 pt-4 dark:border-gray-700">
                                    <div
                                        className="flex items-center justify-between">
                                        <span
                                            className="text-sm text-gray-500 dark:text-gray-400">
                                            {t('booking.totalCurrent')}
                                        </span>
                                        <span className="font-medium">
                                            {formatPrice(preview.currentTotal)}
                                        </span>
                                    </div>
                                    <div
                                        className="mt-2 flex items-center justify-between">
                                        <span className="text-base font-semibold">
                                            {t('booking.totalNew')}
                                        </span>
                                        <span className="text-lg font-bold">
                                            {formatPrice(preview.newTotal)}
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    )}

                    {/* ========================= */}
                    {/* ACTIONS */}
                    {/* ========================= */}
                    <div
                        className="flex justify-end gap-3 pt-2">
                        <button
                            type="button"
                            onClick={onClose}
                            disabled={saving}
                            className="rounded-lg border border-gray-300 px-4 py-2 transition-colors hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-40 dark:border-gray-600 dark:hover:bg-gray-700">
                            {t('common.cancel')}
                        </button>

                        <button
                            type="button"
                            onClick={handleSave}
                            disabled={saving || previewLoading || !hasChanges || !preview }
                            className="flex items-center gap-2 rounded-lg bg-primary-600 px-4 py-2 text-white transition-colors hover:bg-primary-700 disabled:cursor-not-allowed disabled:opacity-40">
                            {saving ? (
                                <>
                                    <Loader2 className="h-4 w-4 animate-spin" />
                                    {t('common.saving')}
                                </>
                            ) : (
                                <>
                                    <Check className="h-4 w-4" />
                                    {t('booking.saveServices')}
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

function isSameServices(
    bookingServices: BookingServiceDto[],
    selectedServices: BookingServiceRequest[]
): boolean {
    if (bookingServices.length !== selectedServices.length) 
    {
        return false;
    }
    return bookingServices.every(current => {
        const selected = selectedServices.find(service =>
            service.additionalServiceId ===
            current.additionalServiceId
        );

        if (!selected) {
            return false;
        }
        return (
            selected.selectedTime === current.selectedTime
        );
    });
}

function requiresTime(type: ServiceType): boolean 
{
    return (
        type === ServiceType.EarlyPickup ||
        type === ServiceType.LateReturn
    );
}

function generateTimeOptions(type: ServiceType): string[] 
{
    const result: string[] = [];
    const startHour = type === ServiceType.LateReturn ? 12 : 0;
    const endHour = type === ServiceType.EarlyPickup ? 11 : 23;
    for (let hour = startHour; hour <= endHour; hour++) 
    {
        if (type === ServiceType.LateReturn && hour === 12) 
        {
            result.push('12:30');
            continue;
        }

        result.push(`${hour
            .toString()
            .padStart(2, '0')}:00`
        );

        result.push(`${hour
            .toString()
            .padStart(2, '0')}:30`
        );
    }
    return result;
}

function getDefaultTime(type: ServiceType): string | undefined 
{
    switch (type) {
        case ServiceType.EarlyPickup:
            return '11:30';

        case ServiceType.LateReturn:
            return '12:30';

        default:
            return undefined;
    }
}