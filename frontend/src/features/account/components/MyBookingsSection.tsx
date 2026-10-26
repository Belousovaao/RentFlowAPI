import { Link } from 'react-router-dom';
import { ArrowRight, CalendarDays } from 'lucide-react';
import type { BookingListItemDto } from '@/entities/booking/booking.types';
import { BookingCard } from '@/features/booking/components/BookingCard';

interface Props {
    bookings: BookingListItemDto[];
    limit?: number;
}

export const MyBookingsSection = ({
    bookings,
    limit = 3,
}: Props) => {
    const visibleBookings = bookings.slice(0, limit);

    return (
        <section>
            <div className="mb-5 flex items-center justify-between gap-4">
                <div>
                    <h2 className="text-2xl font-bold text-gray-900 dark:text-white">
                        Мои бронирования
                    </h2>

                    {bookings.length > 0 && (
                        <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                            История и предстоящие поездки
                        </p>
                    )}
                </div>

                {bookings.length > limit && (
                    <Link
                        to="/account/bookings"
                        className="hidden shrink-0 items-center gap-1.5 text-sm font-medium text-primary-600 transition-colors hover:text-primary-700 sm:flex dark:text-primary-400 dark:hover:text-primary-300"
                    >
                        Все бронирования
                        <ArrowRight className="h-4 w-4" />
                    </Link>
                )}
            </div>

            {visibleBookings.length > 0 ? (
                <div className="space-y-3">
                    {visibleBookings.map(booking => (
                        <BookingCard
                            key={booking.id}
                            booking={booking}
                            compact
                        />
                    ))}
                </div>
            ) : (
                <EmptyBookings />
            )}

            {bookings.length > limit && (
                <Link
                    to="/account/bookings"
                    className="mt-4 flex items-center justify-center gap-2 rounded-xl border border-gray-200 bg-white px-4 py-3 text-sm font-medium text-gray-700 transition-colors hover:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-200 dark:hover:bg-gray-750 sm:hidden"
                >
                    Все бронирования
                    <ArrowRight className="h-4 w-4" />
                </Link>
            )}
        </section>
    );
};

const EmptyBookings = () => {
    return (
        <div className="rounded-2xl border border-dashed border-gray-300 bg-white p-8 text-center dark:border-gray-700 dark:bg-gray-800">
            <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-gray-100 dark:bg-gray-700">
                <CalendarDays className="h-6 w-6 text-gray-400" />
            </div>

            <h3 className="mt-4 font-semibold text-gray-900 dark:text-white">
                Пока нет поездок
            </h3>

            <p className="mx-auto mt-1 max-w-sm text-sm text-gray-500 dark:text-gray-400">
                Забронируйте автомобиль, чтобы ваша первая поездка появилась здесь.
            </p>

            <Link
                to="/assets"
                className="mt-5 inline-flex items-center gap-2 rounded-xl bg-primary-600 px-5 py-2.5 text-sm font-medium text-white transition-colors hover:bg-primary-700"
            >
                Выбрать автомобиль
                <ArrowRight className="h-4 w-4" />
            </Link>
        </div>
    );
};