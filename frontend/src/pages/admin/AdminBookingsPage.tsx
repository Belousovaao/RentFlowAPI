import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { ArrowLeft, Loader2 } from "lucide-react";
import { bookingApi } from "@/entities/booking/booking.api";
import type { AdminBookingListItemDto } from "@/entities/booking/booking.types";
import type { BookingStatus } from "@/entities/booking/booking.types";
import { formatPrice } from "@/shared/utils/formatters";

export const statusLabels: Record<BookingStatus, string> = {
    Pending: "На рассмотрении",
    Confirmed: "Подтверждено",
    Cancelled: "Отменено",
    Completed: "Завершено",
    Active: "Активно",
};

const statusColors: Record<BookingStatus, string> = {
    Pending:
        "bg-amber-100 text-amber-800 dark:bg-amber-900/30 dark:text-amber-400",
    Confirmed:
        "bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400",
    Active:
        "bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400",
    Completed:
        "bg-gray-100 text-gray-700 dark:bg-gray-700 dark:text-gray-300",
    Cancelled:
        "bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400",
};

export const AdminBookingsPage = () => {
    const [bookings, setBookings] = useState<AdminBookingListItemDto[]>([]);
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        const load = async () => {
            try {
                const response = await bookingApi.getAdminBookings();
                setBookings(response);
            } catch (error) {
                console.error("Failed to load admin bookings:", error);
            } finally {
                setLoading(false);
            }
        };

        load();
    }, []);

    if (loading) {
        return (
            <div className="flex justify-center py-16">
                <Loader2 className="h-8 w-8 animate-spin" />
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50 pt-24 pb-16 dark:bg-gray-900">
            <div className="container mx-auto px-4">
                <div className="flex items-center gap-4">
                    <button
                        onClick={() => navigate('/admin/')}
                        className="p-2 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg transition-colors"
                    >
                        <ArrowLeft className="w-5 h-5" />
                    </button>
                    <h1 className="text-2xl font-bold text-gray-700 dark:text-gray-300">
                        Список бронирований
                    </h1>
                </div>
                <div className="overflow-x-auto rounded-2xl border border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-800">
                    <div
                        className="grid min-w-max grid-cols-[auto_auto_auto_auto_auto]"
                    >
                        {/* Header */}
                        <div className="border-b border-gray-200 bg-gray-50 px-5 py-3 text-sm font-medium text-gray-700 dark:text-gray-300 dark:border-gray-700 dark:bg-gray-800/50">
                            Автомобиль
                        </div>
                        <div className="border-b border-gray-200 bg-gray-50 px-5 py-3 text-sm font-medium text-gray-700 dark:text-gray-300 dark:border-gray-700 dark:bg-gray-800/50">
                            Клиент
                        </div>
                        <div className="border-b border-gray-200 bg-gray-50 px-5 py-3 text-sm font-medium text-gray-700 dark:text-gray-300 dark:border-gray-700 dark:bg-gray-800/50">
                            Период
                        </div>
                        <div className="border-b border-gray-200 bg-gray-50 px-5 py-3 text-sm font-medium text-gray-700 dark:text-gray-300 dark:border-gray-700 dark:bg-gray-800/50">
                            Сумма
                        </div>
                        <div className="border-b border-gray-200 bg-gray-50 px-5 py-3 text-sm font-medium text-gray-700 dark:text-gray-300 dark:border-gray-700 dark:bg-gray-800/50">
                            Статус
                        </div>

                        {/* Rows */}
                        {bookings.map((booking) => (
                            <div
                                key={booking.id}
                                onClick={() => navigate(`/admin/bookings/${booking.id}`)}
                                className="grid grid-cols-subgrid col-span-5 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors cursor-pointer"
                            >
                                <div className="px-5 py-5">
                                    <p className="font-semibold whitespace-nowrap text-gray-700 dark:text-gray-300">
                                        {booking.asset.brandName} {booking.asset.model}
                                    </p>

                                    <p className="text-sm text-gray-700 dark:text-gray-300">
                                        {booking.asset.year}
                                    </p>
                                </div>

                                <div className="px-5 py-5">
                                    <p className="font-medium whitespace-nowrap text-gray-700 dark:text-gray-300">
                                        {booking.customer.displayName ?? "Без имени"}
                                    </p>

                                    <p className="text-sm text-gray-700 dark:text-gray-300 whitespace-nowrap">
                                        {booking.customer.phone}
                                    </p>

                                    <p className="text-sm text-gray-700 dark:text-gray-300  whitespace-nowrap">
                                        {booking.customer.email}
                                    </p>
                                </div>

                                <div className="px-5 py-5 text-sm whitespace-nowrap text-gray-700 dark:text-gray-300">
                                    {new Date(booking.rentalPeriod.startDate).toLocaleDateString()}
                                    {" — "}
                                    {new Date(booking.rentalPeriod.endDate).toLocaleDateString()}
                                </div>
                                <div className="px-5 py-5 font-semibold whitespace-nowrap text-gray-700 dark:text-gray-300">
                                    {formatPrice(booking.price.total)}
                                </div>

                                <div className="px-5 py-5">
                                    <span
                                        className={`inline-flex items-center rounded-full px-3 py-1 text-xs font-medium whitespace-nowrap ${statusColors[booking.status]}`}
                                    >
                                        {statusLabels[booking.status]}
                                    </span>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
             
            </div>
        </div>
    );
};