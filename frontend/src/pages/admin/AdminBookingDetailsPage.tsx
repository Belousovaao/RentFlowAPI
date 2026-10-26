import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { ArrowLeft, CalendarDays, ChevronRight, Loader2, Plus, RefreshCw, XCircle,} from "lucide-react";
import { bookingApi } from "@/entities/booking/booking.api";
import type { AdminBookingDetailsDto, BookingStatus, } from "@/entities/booking/booking.types";
import { VerificationStatus } from "@/entities/customer/customer.type";
import { statusLabels } from "./AdminBookingsPage";
import { toast } from "react-hot-toast";
import { BookingStatusModal } from "@/features/admin/components/BookingStatusModal";
import { ChangeBookingPeriodModal } from "@/features/booking/components/ChangeBookingPeriodModal";
import { ServiceModal } from "@/features/booking/components/ServiceModal";
import { formatDate, formatDateTime, formatPrice, } from "@/shared/utils/formatters";
import { travelZones } from "@/entities/destination/destination.constants";
import { RouteReviewStatus } from "@/entities/destination/destination.type";
import { BookingFinancesSection } from "@/features/admin/components/BookingFinancesSection";

export const AdminBookingDetailsPage = () => {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();

    const [booking, setBooking] = useState<AdminBookingDetailsDto | null>(null);
    const [loading, setLoading] = useState(true);
    const [changingStatus, setChangingStatus] = useState(false);
    const [cancelLoading, setCancelLoading] = useState(false);
    const [statusModalOpen, setStatusModalOpen] = useState(false);
    const [periodModalOpen, setPeriodModalOpen] = useState(false);
    const [serviceModalOpen, setServiceModalOpen] = useState(false);

    const customerStatusLabels: Record<VerificationStatus, string> = {
        NotSubmitted: "Не установлен",
        Pending: "На проверке",
        Approved: "Одобрен",
        Rejected: "Отказ",
    };

    const requiresApprovedCustomer = (status: BookingStatus) =>
        status === "Confirmed" || status === "Active";

    const loadBooking = async () => {
        if (!id) return;

        try {
            setLoading(true);

            const response = await bookingApi.getAdminBooking(id);

            setBooking(response);
        } catch (error) {
            console.error("Failed to load booking details:", error);
            toast.error("Не удалось загрузить бронирование.");
        } finally {
            setLoading(false);
        }
    };

    const handleCancelBooking = async () => {
        if (!id || cancelLoading) return;

        const confirmed = window.confirm("Вы действительно хотите отменить бронирование?");

        if (!confirmed) return;

        try {
            setCancelLoading(true);

            await bookingApi.cancel(id);

            toast.success("Бронирование отменено.");

            await loadBooking();
        } catch (error) {
            console.error("Failed to cancel booking:", error);
            toast.error("Не удалось отменить бронирование.");
        } finally {
            setCancelLoading(false);
        }
    };
    useEffect(() => {
        loadBooking();
    }, [id]);

    const changeStatus = async (status: BookingStatus) => {
        if (!booking || status === booking.status) return;

        if (
            requiresApprovedCustomer(status) &&
            booking.customer.verificationStatus !== VerificationStatus.Approved
        ) {
            toast.error(
                "Клиент не одобрен. Подтвердить или активировать бронирование можно только после одобрения клиента."
            );

            return;
        }

        try {
            setChangingStatus(true);

            await bookingApi.changeStatus(booking.id, status);

            setBooking((prev) =>
                prev
                    ? {
                          ...prev,
                          status,
                      }
                    : prev
            );

            toast.success(
                `Статус изменён на «${statusLabels[status]}»`
            );
        } catch (error) {
            console.error("Failed to change booking status:", error);

            toast.error("Не удалось изменить статус бронирования.");
        } finally {
            setChangingStatus(false);
        }
    };

    if (loading) {
        return (
            <div className="flex min-h-screen items-center justify-center bg-gray-50 dark:bg-gray-900">
                <Loader2 className="h-8 w-8 animate-spin text-blue-600" />
            </div>
        );
    }

    if (!booking) {
        return (
            <div className="min-h-screen bg-gray-50 pt-24 dark:bg-gray-900">
                <div className="container mx-auto max-w-5xl px-4">
                    <div className="rounded-2xl bg-white p-8 text-center shadow-sm dark:bg-gray-800">
                        <p className="text-gray-600 dark:text-gray-300">
                            Бронирование не найдено.
                        </p>

                        <button
                            type="button"
                            onClick={() => navigate("/admin/bookings")}
                            className="mt-4 text-sm font-medium text-blue-600 hover:text-blue-700"
                        >
                            Вернуться к списку
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    const canEditBooking =
        booking.status === "Pending" ||
        booking.status === "Confirmed" ||
        booking.status === "Active";
    const destinationZone = travelZones.find((zone) => zone.type === booking.destination.type);
    const routeReviewStatusLabels: Record<RouteReviewStatus, string> = {
        NotRequired: "Проверка не требуется",
        Pending: "На проверке",
        Approved: "Одобрен",
        Rejected: "Отклонён",
    };
    const rentalSegments = booking.price.rentalPrice.segments ?? [];

    const rentalStart = new Date(booking.rentalPeriod.startDate);
    const rentalEnd = new Date(booking.rentalPeriod.endDate);

    const segmentTimeline = rentalSegments.map((segment, index) => {
        const daysBefore = rentalSegments
            .slice(0, index)
            .reduce((sum, item) => sum + item.days, 0);

        const start = new Date(
            rentalStart.getTime() +
                daysBefore * 24 * 60 * 60 * 1000
        );

        const calculatedEnd = new Date(
            start.getTime() +
                segment.days * 24 * 60 * 60 * 1000
        );

        const end =
            calculatedEnd > rentalEnd
                ? rentalEnd
                : calculatedEnd;

        return {
            ...segment,
            start,
            end,
        };
    });

    const rentalPrice =
        booking.price.rentalPrice.rentalPriceWithDiscount;

    const servicesPrice = booking.price.servicesPrice;
    const routeSurcharge = booking.price.routeSurcharge;

    const penaltiesPrice = booking.price.penaltiesPrice;

    const deposit = booking.price.deposit;

    const total = booking.price.total;
    const totalToPay = booking.price.totalToPay;

    return (
        <div className="min-h-screen bg-gray-50 pt-24 pb-16 dark:bg-gray-900">
            <div className="container mx-auto max-w-5xl px-4">
                {/* Header */}
                <div className="mb-8 flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
                    <button
                        type="button"
                        onClick={() => navigate("/admin/bookings")}
                        className="flex w-fit items-center gap-2 text-sm font-medium text-gray-600 transition hover:text-gray-900 dark:text-gray-300 dark:hover:text-white"
                    >
                        <ArrowLeft className="h-4 w-4" />
                        Список всех бронирований
                    </button>

                    <div className="flex flex-wrap items-center gap-3">
                        {(booking.status === "Pending" || booking.status === "Confirmed") && (
                            <button
                                type="button"
                                onClick={handleCancelBooking}
                                disabled={cancelLoading}
                                className="inline-flex items-center gap-2 rounded-xl border border-red-200 bg-white px-4 py-2.5 text-sm font-medium text-red-600 shadow-sm transition hover:bg-red-50 disabled:cursor-not-allowed disabled:opacity-50 dark:border-red-900 dark:bg-gray-800 dark:text-red-400 dark:hover:bg-red-950/30"
                            >
                                {cancelLoading ? (
                                    <Loader2 className="h-4 w-4 animate-spin" />
                                ) : (
                                    <XCircle className="h-4 w-4" />
                                )}

                                Отменить бронирование
                            </button>
                        )}
                        <button
                            type="button"
                            onClick={() => setStatusModalOpen(true)}
                            disabled={changingStatus}
                            className="inline-flex items-center gap-2 rounded-xl border border-gray-200 bg-white px-4 py-2.5 text-sm font-medium text-gray-700 shadow-sm transition hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-200 dark:hover:bg-gray-750"
                        >
                            {changingStatus ? (
                                <Loader2 className="h-4 w-4 animate-spin" />
                            ) : (
                                <RefreshCw className="h-4 w-4" />
                            )}

                            Изменить статус
                        </button>

                        <button
                            type="button"
                            disabled={!canEditBooking}
                            onClick={() => setPeriodModalOpen(true)}
                            className="inline-flex items-center gap-2 rounded-xl bg-blue-600 px-4 py-2.5 text-sm font-medium text-white shadow-sm transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-50"
                        >
                            <CalendarDays className="h-4 w-4" />
                            Изменить срок
                        </button>
                    </div>
                </div>

                <div className="space-y-6">
                    {/* Автомобиль + Клиент */}
                    <div className="grid gap-6 lg:grid-cols-2">
                        {/* Автомобиль */}
                        <button
                            type="button"
                            onClick={() =>
                                navigate(`/assets/${booking.asset.id}`)
                            }
                            className="group w-full text-left"
                        >
                            <section className="h-full rounded-2xl bg-white p-6 shadow-sm transition hover:shadow-md dark:bg-gray-800">
                                <div className="flex items-start justify-between">
                                    <div>
                                        <h2 className="text-sm font-medium text-gray-500 dark:text-gray-400">
                                            Автомобиль
                                        </h2>

                                        <p className="mt-2 text-xl font-semibold text-gray-900 dark:text-white">
                                            {booking.asset.brandName}{" "}
                                            {booking.asset.model}
                                        </p>

                                        <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                                            {booking.asset.year}
                                        </p>
                                    </div>

                                    <ChevronRight className="h-5 w-5 text-gray-400 transition group-hover:translate-x-1 group-hover:text-gray-600 dark:group-hover:text-gray-200" />
                                </div>
                            </section>
                        </button>

                        {/* Клиент */}
                        <button
                            type="button"
                            onClick={() =>
                                navigate(
                                    `/admin/customers/${booking.customer.id}`
                                )
                            }
                            className="group w-full text-left"
                        >
                            <section className="h-full rounded-2xl bg-white p-6 shadow-sm transition hover:shadow-md dark:bg-gray-800">
                                <div className="flex items-start justify-between">
                                    <div className="min-w-0">
                                        <h2 className="text-sm font-medium text-gray-500 dark:text-gray-400">
                                            Клиент
                                        </h2>

                                        <p className="mt-2 text-xl font-semibold text-gray-900 dark:text-white">
                                            {booking.customer.displayName ??
                                                "Без имени"}
                                        </p>

                                        <div className="mt-2 space-y-1 text-sm text-gray-500 dark:text-gray-400">
                                            <p>{booking.customer.phone}</p>

                                            {booking.customer.email && (
                                                <p>
                                                    {
                                                        booking.customer
                                                            .email
                                                    }
                                                </p>
                                            )}

                                            <p>
                                                Статус:{" "}
                                                <span className="font-medium text-gray-700 dark:text-gray-200">
                                                    {
                                                        customerStatusLabels[
                                                            booking.customer
                                                                .verificationStatus
                                                        ]
                                                    }
                                                </span>
                                            </p>
                                        </div>
                                    </div>

                                    <ChevronRight className="h-5 w-5 shrink-0 text-gray-400 transition group-hover:translate-x-1 group-hover:text-gray-600 dark:group-hover:text-gray-200" />
                                </div>
                            </section>
                        </button>
                    </div>

                    {/* Период аренды */}
                    <section className="rounded-2xl bg-white p-6 shadow-sm dark:bg-gray-800">
                        <div className="mb-6">
                            <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
                                Период аренды
                            </h2>
                        </div>

                        <div className="relative">
                            {/* Линия таймлайна */}
                            <div className="absolute left-[7px] top-2 bottom-2 w-px bg-gray-200 dark:bg-gray-700" />

                            {/* Начало */}
                            <div className="relative flex gap-4">
                                <div className="relative z-10 mt-1 h-4 w-4 shrink-0 rounded-full border-4 border-white bg-blue-600 dark:border-gray-800" />

                                <div className="pb-5">
                                    <p className="text-xs font-medium uppercase tracking-wide text-gray-400">
                                        Начало аренды
                                    </p>

                                    <p className="mt-1 font-medium text-gray-900 dark:text-white">
                                        {formatDateTime(rentalStart)}
                                    </p>
                                </div>
                            </div>

                            {/* Сегменты */}
                            <div className="space-y-4">
                                {segmentTimeline.map((segment, index) => (
                                    <div
                                        key={`${segment.start.toISOString()}-${index}`}
                                        className="relative flex gap-4"
                                    >
                                        <div className="relative z-10 mt-5 h-4 w-4 shrink-0 rounded-full border-4 border-white bg-gray-300 dark:border-gray-800 dark:bg-gray-600" />

                                        <div className="min-w-0 flex-1 rounded-xl border border-gray-100 bg-gray-50 p-4 dark:border-gray-700 dark:bg-gray-700/50">
                                            <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
                                                <div>
                                                    <div className="flex flex-wrap items-center gap-2">
                                                        <span className="font-semibold text-gray-900 dark:text-white">
                                                            {segment.days}{" "}
                                                            {segment.days ===
                                                            1
                                                                ? "день"
                                                                : segment.days <
                                                                    5
                                                                  ? "дня"
                                                                  : "дней"}
                                                        </span>

                                                        {segment.durationDiscountPercent >
                                                            0 && (
                                                            <span className="rounded-full bg-green-100 px-2.5 py-1 text-xs font-medium text-green-700 dark:bg-green-900/30 dark:text-green-400">
                                                                −
                                                                {
                                                                    segment.durationDiscountPercent
                                                                }
                                                                %
                                                            </span>
                                                        )}
                                                    </div>

                                                    <p className="mt-2 text-sm text-gray-500 dark:text-gray-400">
                                                        {formatDate(
                                                            segment.start
                                                        )}{" "}
                                                        —{" "}
                                                        {formatDate(
                                                            segment.end
                                                        )}
                                                    </p>
                                                </div>

                                                <div className="text-left sm:text-right">
                                                    <p className="text-sm text-gray-500 dark:text-gray-400">
                                                        {formatPrice(
                                                            segment.discountedDailyPrice
                                                        )}{" "}
                                                        / день
                                                    </p>

                                                    <p className="mt-1 font-semibold text-gray-900 dark:text-white">
                                                        {formatPrice(
                                                            segment.total
                                                        )}
                                                    </p>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>

                            {/* Конец */}
                            <div className="relative mt-4 flex gap-4">
                                <div className="relative z-10 mt-1 h-4 w-4 shrink-0 rounded-full border-4 border-white bg-blue-600 dark:border-gray-800" />

                                <div>
                                    <p className="text-xs font-medium uppercase tracking-wide text-gray-400">
                                        Конец аренды
                                    </p>

                                    <p className="mt-1 font-medium text-gray-900 dark:text-white">
                                        {formatDateTime(rentalEnd)}
                                    </p>
                                </div>
                            </div>
                        </div>
                    </section>

                    {/* Дополнительные услуги */}
                    <section className="rounded-2xl bg-white p-6 shadow-sm dark:bg-gray-800">
                        <div className="mb-5 flex items-center justify-between gap-4">
                            <div>
                                <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
                                    Дополнительные услуги
                                </h2>

                                {booking.services.length > 0 && (
                                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                                        {booking.services.length}{" "}
                                        {booking.services.length === 1
                                            ? "услуга"
                                            : booking.services.length < 5
                                              ? "услуги"
                                              : "услуг"}
                                    </p>
                                )}
                            </div>

                            <button
                                type="button"
                                disabled={!canEditBooking}
                                onClick={() => setServiceModalOpen(true)}
                                className="inline-flex shrink-0 items-center gap-2 rounded-xl border border-gray-200 bg-white px-3.5 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-200 dark:hover:bg-gray-750"
                            >
                                <Plus className="h-4 w-4" />
                                Добавить
                            </button>
                        </div>

                        {booking.services.length === 0 ? (
                            <div className="rounded-xl border border-dashed border-gray-200 px-4 py-8 text-center dark:border-gray-700">
                                <p className="text-sm text-gray-500 dark:text-gray-400">
                                    Дополнительные услуги не выбраны.
                                </p>
                            </div>
                        ) : (
                            <div className="divide-y divide-gray-100 dark:divide-gray-700">
                                {booking.services.map((service, index) => (
                                    <div
                                        key={`${service.name}-${service.selectedTime ?? ""}-${index}`}
                                        className="flex flex-col gap-2 py-4 first:pt-0 last:pb-0 sm:flex-row sm:items-center sm:justify-between"
                                    >
                                        <div>
                                            <p className="font-medium text-gray-900 dark:text-white">
                                                {service.name}
                                            </p>

                                            {service.selectedTime && (
                                                <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                                                    {service.selectedTime}
                                                </p>
                                            )}
                                        </div>

                                        <p className="font-medium text-gray-900 dark:text-white">
                                            {formatPrice(service.price)}
                                        </p>
                                    </div>
                                ))}
                            </div>
                        )}
                    </section>

                    {/* Маршрут + Стоимость */}
                    <div className="grid gap-6 lg:grid-cols-2">
                        {/* Маршрут */}
                        <section className="rounded-2xl bg-white p-6 shadow-sm dark:bg-gray-800">
                            <div className="mb-5 flex items-center gap-3">
                                {destinationZone && (
                                    <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-blue-50 text-blue-600 dark:bg-blue-900/30 dark:text-blue-400">
                                        <destinationZone.icon className="h-5 w-5" />
                                    </div>
                                )}

                                <div>
                                    <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
                                        Маршрут
                                    </h2>

                                    <p className="mt-0.5 text-sm text-gray-500 dark:text-gray-400">
                                        {destinationZone?.title ?? booking.destination.type}
                                    </p>
                                </div>
                            </div>

                            {booking.destination.destinationNameSnapshot && (
                                <div className="mb-4">
                                    <p className="text-xs font-medium uppercase tracking-wide text-gray-400">
                                        Направление
                                    </p>
                                    <p className="mt-1 font-medium text-gray-900 dark:text-white">
                                        {booking.destination.destinationNameSnapshot}
                                    </p>
                                </div>
                            )}

                            {booking.destinationDetails && (
                                <div className="mb-4">
                                    <p className="font-medium text-gray-900 dark:text-white">
                                        {booking.destinationDetails.region}
                                    </p>
                                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                                        Расстояние: {booking.destinationDetails.distanceKm} км
                                    </p>
                                </div>
                            )}

                            {booking.destination.customRouteDescription && (
                                <div className="mb-4">
                                    <p className="text-xs font-medium uppercase tracking-wide text-gray-400">
                                        Описание маршрута
                                    </p>
                                    <p className="mt-1 whitespace-pre-wrap break-words text-sm text-gray-700 dark:text-gray-300">
                                        {booking.destination.customRouteDescription}
                                    </p>
                                </div>
                            )}

                            <div className="border-t border-gray-100 pt-4 dark:border-gray-700">
                                <div className="flex items-center justify-between gap-3">
                                    <span className="text-sm text-gray-500 dark:text-gray-400">
                                        Статус проверки
                                    </span>
                                    <span className="text-right text-sm font-medium text-gray-900 dark:text-white">
                                        {routeReviewStatusLabels[booking.destination.reviewStatus]}
                                    </span>
                                </div>
                            </div>
                        </section>
                        {/* Единая финансовая история */}
                        <BookingFinancesSection bookingId={booking.id} price={booking.price} services={booking.services} />
                    </div>
                </div>

                {/* Модальные окна */}
                <BookingStatusModal
                    open={statusModalOpen}
                    currentStatus={booking.status}
                    onClose={() => setStatusModalOpen(false)}
                    onChange={changeStatus}
                />

                <ChangeBookingPeriodModal
                    isOpen={periodModalOpen}
                    booking={booking}
                    onClose={() => setPeriodModalOpen(false)}
                    onSuccess={loadBooking}
                />

                <ServiceModal
                    isOpen={serviceModalOpen}
                    bookingId={booking.id}
                    bookingServices={booking.services}
                    onClose={() => setServiceModalOpen(false)}
                    onSuccess={loadBooking}
                />
            </div>
        </div>
    );
};