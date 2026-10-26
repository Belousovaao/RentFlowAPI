import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useBookingStore } from '@/features/booking/stores/bookingStore';
import { useAuthStore } from '@/features/auth/stores/authStore';
import { useI18n } from '@/app/providers/I18nProvider';
import { formatDateForBack, formatPrice, getDaysLabel } from '@/shared/utils/formatters';
import { getPhotoUrl } from '@/shared/utils/photoUrl';
import { Button } from '@/features/ui/components/Button';
import api from '@/shared/api/axios';
import { AdditionalService } from '@/entities/service/service.types';
import { serviceApi } from '@/features/service/api/serviceApi';
import { AdditionalServicesSelector } from '@/features/service/components/AdditionalServicesSelector';
import { calculateDays } from '@/features/booking/utils/priceCalculator';
import { BookingCalculationResultDto } from '@/entities/booking/booking.types';
import { CustomerSearch } from '@/features/customer/components/CustomerSearch';
import { CustomerDetailsDto, CustomerLookup, VerificationStatus } from '@/entities/customer/customer.type';
import { CustomerInfo } from '@/features/customer/components/CustomerInfo';
import { BookingButton } from '@/features/booking/components/BookingButton';
import { UploadDocumentsModal } from '@/features/customer/components/UploadDocumentsModal';
import { GlassCard } from '@/features/ui/components/GlassCard';
import { DestinationDto, DestinationType } from '@/entities/destination/destination.type';
import { customerApi } from '@/features/customer/api/customerApi';
import { useAuthModalStore } from '@/features/auth/stores/authModalStore';
import { destinationApi } from '@/entities/destination/destinationApi';
import { travelZones } from "@/entities/destination/destination.constants";

export const CheckoutPage = () => {
    const { t } = useI18n();
    const navigate = useNavigate();
    const { currentBooking, clearBooking, addBooking, setBookingDetails } = useBookingStore();
    const { user, isAuthenticated } = useAuthStore();
    const { open: openAuthModal} = useAuthModalStore();
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const { asset, startDate, endDate } = currentBooking;
    const [services, setServices] = useState<AdditionalService[]>([]);
    const [price, setPrice] = useState<BookingCalculationResultDto | null>(null);
    const [selectedCustomer, setSelectedCustomer] = useState<CustomerLookup | null>(null);
    const [currentCustomer, setCurrentCustomer] = useState<CustomerDetailsDto | null>(null);
    const [uploadModalOpen, setUploadModalOpen] = useState(false);
    const [type, setType] = useState<DestinationType>(DestinationType.Local);
    const [destinationId, setDestinationId] = useState('');
    const [customRouteDescription, setCustomRouteDescription] = useState('');
    const [destinations, setDestinations] = useState<DestinationDto[]>([]);
    const [destinationsLoading, setDestinationsLoading] = useState(false);
    const [destinationError, setDestinationError] = useState<string | null>(null);
    const isAdmin = user?.role === "admin";

    const requiresCatalogDestination = type === DestinationType.Regional || type === DestinationType.LongDistance;
    const selectedDestination = destinations.find(destination => destination.id === destinationId) ?? null;
    const routeIsValid = type === DestinationType.Local || (requiresCatalogDestination && selectedDestination !== null) || (type === DestinationType.Other && customRouteDescription.trim().length > 0);

    useEffect(() => {
            serviceApi.getAll().then(response => setServices(response.data)).catch(console.error);
        }, []);

        useEffect(() => {
        if (!requiresCatalogDestination) {
            setDestinations([]);
            setDestinationId('');
            setDestinationError(null);
            return;
        }

        let cancelled = false;

        const loadDestinations = async () => {
            setDestinationsLoading(true);
            setDestinationError(null);
            setDestinationId('');

            try {
                const { data } = await destinationApi.getActive(type);

                if (!cancelled) {
                    setDestinations(data);
                }
            } catch (err) {
                console.error(err);

                if (!cancelled) {
                    setDestinations([]);
                    setDestinationError('Не удалось загрузить направления. Попробуйте ещё раз.');
                }
            } finally {
                if (!cancelled) {
                    setDestinationsLoading(false);
                }
            }
        };

        void loadDestinations();

        return () => {
            cancelled = true;
        };
    }, [type, requiresCatalogDestination]);

    
    if (!asset || !startDate || !endDate) {
        return (
            <div className="mx-auto max-w-2xl px-4 py-16 text-center">
                <h1 className="text-xl font-semibold">
                    Данные бронирования не найдены
                </h1>

                <p className="mt-2 text-gray-600">
                    Выберите автомобиль и период аренды, чтобы продолжить.
                </p>

                <button
                    type="button"
                    onClick={() => navigate('/')}
                    className="mt-6 rounded-lg bg-blue-600 px-5 py-3 text-white"
                >
                    Вернуться к автомобилям
                </button>
            </div>
        );
    }


    const coverPhoto = asset.photos?.find(p => p.type === 'Exterior') ?? asset.photos?.[0];
    const days = calculateDays(startDate, endDate);
    const selectedServices = currentBooking.selectedServices;
    const rentalPrice = price?.price.rentalPrice;
    const depositAmount = price?.price.deposit ?? 0;
    const servicesPrice = price?.price.servicesPrice ?? 0;
    const total = price?.price.total ?? 0;
    const totalToPay = price?.price.totalToPay ?? 0;
    const rentalSegments = price?.price.rentalPrice.segments ?? [];
    const currentSegment = rentalSegments.length > 0
        ? rentalSegments[rentalSegments.length - 1]
        : null;

    const loadCurrentCustomer = async () => {
        if (isAdmin) {
            return null;
        }

        const { data } = await customerApi.getMe();
        setCurrentCustomer(data);
        return data;
    };
    
    useEffect(() => {
        if (!asset || !startDate || !endDate) return;

        const recalculate = async () => {
            if (!routeIsValid) {
                setPrice(null);
                return;
            }

            try {
                const { data } = await api.post("/booking/calculate-price",
                    {
                        assetId: asset.id,
                        pickupDate: formatDateForBack(startDate),
                        pickupTime: "12:00",
                        returnDate: formatDateForBack(endDate),
                        returnTime: "12:00",
                        destinationType: type,
                        destinationId: requiresCatalogDestination ? destinationId : null,
                        customRouteDescription:
                            type === DestinationType.Other
                                ? customRouteDescription.trim()
                                : null,
                        services: selectedServices.map(s => ({
                            additionalServiceId: s.id,
                            selectedTime: s.selectedTime,
                        }))
                    }
                );
                setPrice(data);
            }
            catch (e) {
                console.error(e);
            }
        };
        recalculate();
    }, [asset, startDate, endDate, selectedServices, type, destinationId, customRouteDescription]);

    const handleDocumentsUploaded = async () => {
        try {
            const customer = await loadCurrentCustomer();

            if (!customer) {
                setError("Не удалось загрузить данные клиента.");
                return;
            }

            if (customer.verificationStatus !== VerificationStatus.Pending) {
                setError("Документы отправлены, но статус проверки не обновился.");
                return;
            }

            await onCreateBooking(customer.id);
        } catch (err: unknown) {
            console.error(err);
            setError("Не удалось обновить статус клиента после отправки документов.");
        }
    };
    const onCreateBooking = async (customerIdOverride?: string) => {
        if (!routeIsValid) {
            setError(
                requiresCatalogDestination
                    ? 'Выберите город назначения.'
                    : 'Укажите описание маршрута.'
            );
            return;
        }
        const currentUser = useAuthStore.getState().user;
        const currentIsAdmin = currentUser?.role === "admin";

        const bookingCustomerId = currentIsAdmin
            ? selectedCustomer?.id
            : customerIdOverride ?? currentCustomer?.id;

        if (!bookingCustomerId) {
            setError("Не удалось определить профиль клиента");
            return;
        }

        setSubmitting(true);
        setError(null);

        try {
            const { data } = await api.post('/booking', {
                assetId: asset.id,
                customerId: bookingCustomerId,
                pickupDate: formatDateForBack(startDate),
                pickupTime: "12:00",
                returnDate: formatDateForBack(endDate),
                returnTime: "12:00",
                driver: null,
                destinationType: type,
                destinationId: requiresCatalogDestination ? destinationId : null,
                customRouteDescription:
                    type === DestinationType.Other
                        ? customRouteDescription.trim()
                        : null,
                services: selectedServices.map(service => ({
                    additionalServiceId: service.id,
                    selectedTime: service.selectedTime,
                }))
            });

            addBooking(data);
            clearBooking();

            currentIsAdmin
                ? navigate('/admin/bookings')
                : navigate('/account/bookings', { replace: true });
        } catch (err: unknown) {
            const message =
                (err as {
                    response?: {
                        data?: {
                            title?: string;
                            message?: string;
                        };
                    };
                })?.response?.data?.title ??
                (err as {
                    response?: {
                        data?: {
                            message?: string;
                        };
                    };
                })?.response?.data?.message ??
                t('auth.error.generic');

            setError(message);
        } finally {
            setSubmitting(false);
        }
    };
    const continueBookingAfterAuth = async () => {
        const {
            isAuthenticated: authenticated,
            user: currentUser,
        } = useAuthStore.getState();

        if (!authenticated) {
            return;
        }

        setError(null);
        const currentIsAdmin = currentUser?.role === "admin";

        if (currentIsAdmin) {
            await onCreateBooking();
            return;
        }

        try {
            const customer = await customerApi.getMe()
                .then(response => response.data);

            setCurrentCustomer(customer);

            switch (customer.verificationStatus) {
                case VerificationStatus.Approved:
                case VerificationStatus.Pending:
                    await onCreateBooking(customer.id);
                    return;

                case VerificationStatus.NotSubmitted:
                case VerificationStatus.Rejected:
                    setUploadModalOpen(true);
                    return;

                default:
                    setError("Неизвестный статус проверки клиента.");
            }
        } catch (err: unknown) {
            console.error(err);
            setError("Не удалось загрузить данные клиента.");
        }
    };
    const handleBookingClick = async () => {
        setError(null);

        // Неавторизованный пользователь
        if (!isAuthenticated) {
            openAuthModal(() => {
                void continueBookingAfterAuth();
            });
            return;
        }

        // Администратор
        if (isAdmin) {
            await onCreateBooking();
            return;
        }

        // Обычный клиент — получаем актуальный Customer
        const customer = await loadCurrentCustomer();

        if (!customer) {
            setError("Не удалось определить профиль клиента.");
            return;
        }

        switch (customer.verificationStatus) {
            case VerificationStatus.Approved:
            case VerificationStatus.Pending:
                await onCreateBooking(customer.id);
                return;

            case VerificationStatus.NotSubmitted:
            case VerificationStatus.Rejected:
                setUploadModalOpen(true);
                return;

            default:
                setError("Неизвестный статус проверки клиента.");
        }
    };
    
    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-900 pt-24 pb-16">
            <div className="max-w-6xl mx-auto px-4">
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                    {/* ЛЕВАЯ КОЛОНКА */}
                    <div className="space-y-6">
                        {/* Автомобиль */}
                        <GlassCard>
                            <div>
                                <img
                                    src={getPhotoUrl(coverPhoto?.url) || '/car-placeholder.jpg'}
                                    alt={`${asset.brandName} ${asset.model}`}
                                    className="object-cover rounded-xl"
                                />
                                <div>
                                    <h2 className="text-2xl font-bold text-gray-900 dark:text-white">
                                        {asset.brandName} {asset.model}
                                    </h2>
                                    <p className="text-gray-500 dark:text-gray-400 mt-1">
                                        {asset.year}
                                    </p>
                                </div>
                            </div>
                        </GlassCard>
                        {/* Период */}
                        <GlassCard>
                            <h2 className="text-xl font-bold text-gray-900 dark:text-white mb-4">
                                {t('checkout.period')}
                            </h2>
                            <div className="flex justify-between">
                                <div>
                                    <p className="font-medium text-gray-900 dark:text-white">
                                        {startDate.toLocaleDateString()} - {endDate.toLocaleDateString()}
                                    </p>
                                </div>
                                <div>
                                    <p className="font-sm text-gray-700 dark:text-gray-300">
                                        {days} {getDaysLabel(days, t)}
                                    </p>
                                </div>
                            </div>
                        </GlassCard>
                        {/* Клиент */}
                        <GlassCard>
                            <h2 className="text-xl font-bold text-gray-900 dark:text-white mb-4">
                                {t("checkout.renter")}
                            </h2>
                            {isAdmin ? (
                                selectedCustomer ? (
                                    <div className="space-y-3">
                                        <CustomerInfo
                                            customer={selectedCustomer}
                                        />
                                        <Button
                                            variant="outline"
                                            size="sm"
                                            onClick={() =>
                                                setSelectedCustomer(null)
                                            }
                                        >
                                            Сменить клиента
                                        </Button>
                                    </div>
                                ) : (
                                    <CustomerSearch
                                        onSelect={setSelectedCustomer}
                                    />
                                )
                            ) : (
                                <CustomerInfo customer={currentCustomer ?? undefined}/>
                            )}
                        </GlassCard>
                        {/* Маршрут поездки */}
                        <GlassCard>
                            <h2 className="text-xl font-bold text-gray-900 dark:text-white mb-5">
                                {t('checkout.whereToGo')}
                            </h2>
                            <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                                {travelZones.map(item => {

                                    const Icon = item.icon;
                                    const selected = type === item.type;

                                    return (
                                        <button
                                            key={item.type}
                                            type="button"
                                            onClick={() => {
                                                setType(item.type)
                                                setDestinationId('');
                                                setDestinationError(null);
                                                setError(null)
                                            }}
                                            className={`relative text-left rounded-2xl border p-4 transition-all duration-200
                                                ${selected
                                                        ? `border-primary-500 bg-primary-50 dark:bg-primary-900/20 shadow-md`
                                                        : `border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800/60 hover:border-primary-300 dark:hover:border-primary-700 hover:shadow-sm`
                                                }
                                            `}>
                                                {selected && (
                                                    <div className="absolute top-3 right-3 w-2.5 h-2.5 rounded-full bg-primary-600"/>
                                                )}
                                                <div className={`flex items-center justify-center w-10 h-10 rounded-xl mb-3
                                                    ${
                                                        selected
                                                            ? `bg-primary-100 dark:bg-primary-900/40 text-primary-600`
                                                            : `bg-gray-100dark:bg-gray-700 text-gray-500 dark:text-gray-300`
                                                    }
                                                `}>
                                                    <Icon className="w-5 h-5" />
                                                </div>
                                                <div className="font-semibold text-gray-900 dark:text-white">
                                                    {item.title}
                                                </div>
                                                <div className="text-xs text-gray-500 dark:text-gray-400 mt-1">
                                                    {item.description}
                                                </div>
                                        </button>
                                    );
                                })}
                            </div>
                            {requiresCatalogDestination && (
                                <div className="mt-4 space-y-2">
                                    <label
                                        htmlFor="destination"
                                        className="block font-semibold text-gray-900 dark:text-white"
                                    >
                                        Город назначения
                                    </label>

                                    <div className="relative mt-1">
                                        <select
                                            id="destination"
                                            value={destinationId}
                                            onChange={e => setDestinationId(e.target.value)}
                                            disabled={destinationsLoading || destinations.length === 0}
                                            className="w-full appearance-none rounded-xl border border-gray-200 bg-white px-4 py-3.5 pr-11 text-sm font-medium text-gray-900 shadow-sm outline-none transition-all duration-200 hover:border-gray-300 focus:border-primary-500 focus:ring-4 focus:ring-primary-500/10 disabled:cursor-not-allowed disabled:opacity-60 dark:border-gray-700 dark:bg-gray-800 dark:text-white dark:hover:border-gray-600"
                                        >
                                            <option value="" disabled>
                                                {destinationsLoading
                                                    ? 'Загружаем направления...'
                                                    : 'Выберите город'}
                                            </option>

                                            {destinations.map(destination => (
                                                <option key={destination.id} value={destination.id}>
                                                    {destination.name}
                                                </option>
                                            ))}
                                        </select>

                                        <svg
                                            className="pointer-events-none absolute right-4 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400"
                                            viewBox="0 0 20 20"
                                            fill="none"
                                            aria-hidden="true"
                                        >
                                            <path
                                                d="m5 7.5 5 5 5-5"
                                                stroke="currentColor"
                                                strokeWidth="1.7"
                                                strokeLinecap="round"
                                                strokeLinejoin="round"
                                            />
                                        </svg>
                                    </div>

                                    {selectedDestination && (
                                        <div className="text-sm text-gray-600 dark:text-gray-300">
                                            <p>
                                                Доплата за маршрут:{' '}
                                                {formatPrice(selectedDestination.routeSurcharge)}
                                            </p>
                                        </div>
                                    )}

                                    {destinationError && (
                                        <p className="text-sm text-red-600">{destinationError}</p>
                                    )}
                                </div>
                            )}

                            {type === DestinationType.Other && (
                                <div className="mt-4 space-y-2">
                                    <label
                                        htmlFor="customRoute"
                                        className="block text-sm font-medium text-gray-700 dark:text-gray-200"
                                    >
                                        Описание маршрута
                                    </label>

                                    <textarea
                                        id="customRoute"
                                        value={customRouteDescription}
                                        onChange={e => setCustomRouteDescription(e.target.value)}
                                        maxLength={2000}
                                        rows={3}
                                        placeholder="Например, Красноярск — Абакан"
                                        className="w-full rounded-xl border border-gray-300 dark:border-gray-700 bg-white dark:bg-gray-800 px-3 py-3 text-gray-900 dark:text-white"
                                    />
                                    <p className="text-xs text-gray-500 dark:text-gray-400">
                                        Окончательные условия аренды определяются после проверки маршрута менеджером.
                                    </p>
                                </div>
                            )}
                        </GlassCard>

                        {/* Ошибка */}
                        {error && (
                            <p className="text-sm text-red-600 dark:text-red-400">
                                {error}
                            </p>
                        )}
                    </div>

                    {/* ПРАВАЯ КОЛОНКА */}
                    <div className="space-y-6">
                        {/* Дополнительные услуги */}
                        <GlassCard>
                            <AdditionalServicesSelector
                                services={services}
                                selectedServices={selectedServices}
                                calculatedServices={price?.services ?? []}
                                onChange={(services) => setBookingDetails({selectedServices: services})}
                            />
                        </GlassCard>

                        {/* Стоимость */}
                        <GlassCard>
                            <h2 className="text-xl font-bold text-gray-900 dark:text-white mb-5">
                                {t('checkout.priceBreakdown')}
                            </h2>

                            <div className="space-y-4">
                                {currentSegment && (
                                    <>
                                        {/* Базовая аренда */}
                                        <div className="flex justify-between gap-4">
                                            <span className="text-gray-600 dark:text-gray-300">
                                                Базовый тариф {formatPrice(currentSegment.baseDailyPrice)} × {currentSegment.days}
                                            </span>

                                            <span className="font-medium text-gray-900 dark:text-white whitespace-nowrap">
                                                {formatPrice(rentalPrice?.baseRentalPrice ?? 0)}
                                            </span>
                                        </div>

                                        {/* Скидка */}
                                        {rentalPrice && rentalPrice.discountAmount > 0 && (
                                            <>
                                                <div className="flex justify-between gap-4">
                                                    <span className="text-gray-600 dark:text-gray-300">
                                                        Скидка {currentSegment.durationDiscountPercent}%
                                                    </span>

                                                    <span className="font-medium text-green-600 whitespace-nowrap">
                                                        − {formatPrice(rentalPrice.discountAmount)}
                                                    </span>
                                                </div>

                                                {/* Аренда после скидки */}
                                                <div className="flex justify-between gap-4">
                                                    <span className="text-gray-600 dark:text-gray-300">
                                                        Тариф для Вас {formatPrice(currentSegment.discountedDailyPrice)} × {currentSegment.days}
                                                    </span>

                                                    <span className="font-medium text-gray-900 dark:text-white whitespace-nowrap">
                                                        {formatPrice(rentalPrice?.rentalPriceWithDiscount ?? 0)}
                                                    </span>
                                                </div>
                                            </>
                                        )}
                                    </>
                                )}

                                {/* Дополнительные услуги */}
                                {servicesPrice > 0 && (
                                    <div className="flex justify-between gap-4">
                                        <span className="text-gray-600 dark:text-gray-300">
                                            Дополнительные услуги
                                        </span>

                                        <span className="font-medium text-gray-900 dark:text-white whitespace-nowrap">
                                            {formatPrice(servicesPrice)}
                                        </span>
                                    </div>
                                )}

                                {/* Разделитель */}
                                <div className="border-t border-gray-200 dark:border-gray-700" />

                                {/* Стоимость аренды + услуг */}
                                <div className="flex justify-between gap-4">
                                    <span className="font-medium text-gray-700 dark:text-gray-200">
                                        Стоимость аренды и услуг
                                    </span>

                                    <span className="font-semibold text-gray-900 dark:text-white whitespace-nowrap">
                                        {formatPrice(total)}
                                    </span>
                                </div>

                                {(price?.price.routeSurcharge ?? 0) > 0 && (
                                    <div className="flex justify-between gap-4">
                                        <span className="text-gray-600 dark:text-gray-300">
                                            Доплата за маршрут
                                        </span>

                                        <span className="font-medium text-gray-900 dark:text-white whitespace-nowrap">
                                            {formatPrice(price?.price.routeSurcharge ?? 0)}
                                        </span>
                                    </div>
                                )}

                                {/* Депозит */}
                                {depositAmount > 0 && (
                                    <div className="flex justify-between gap-4">
                                        <div>
                                            <span className="text-gray-600 dark:text-gray-300">
                                                Депозит
                                            </span>
                                            <p className="text-xs text-gray-400 mt-0.5">
                                                Возвращается после завершения аренды
                                            </p>
                                        </div>

                                        <span className="font-medium text-gray-900 dark:text-white whitespace-nowrap">
                                            {formatPrice(depositAmount)}
                                        </span>
                                    </div>
                                )}

                                {/* Итого к оплате */}
                                <div className="border-t border-gray-200 dark:border-gray-700 pt-4">
                                    <div className="flex justify-between items-center gap-4">
                                        <span className="text-lg font-bold text-gray-900 dark:text-white">
                                            Итого к оплате
                                        </span>

                                        <span className="text-xl font-bold text-primary-600 dark:text-primary-400 whitespace-nowrap">
                                            {formatPrice(totalToPay)}
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </GlassCard>

                        {/* Действия */}
                        <GlassCard>
                            <div className='flex gap-3'>
                                <BookingButton
                                    loading={submitting}
                                    onClick={handleBookingClick}
                                    disabled={!routeIsValid}
                                />
                                <Button
                                    variant="outline"
                                    onClick={() => navigate(-1)}
                                    disabled={submitting}
                                >
                                    {t('common.cancel')}
                                </Button>
                            </div>
                        </GlassCard>
                    </div>
                </div>
            </div>

            <UploadDocumentsModal
                open={uploadModalOpen}
                onClose={() => setUploadModalOpen(false)}
                onSuccess={handleDocumentsUploaded}
            />
        </div>
    );
};
