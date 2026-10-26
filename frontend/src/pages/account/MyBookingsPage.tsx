import { useEffect, useState } from 'react';
import { bookingApi } from '@/entities/booking/booking.api';
import type { BookingListItemDto } from '@/entities/booking/booking.types';
import { BookingCard } from '@/features/booking/components/BookingCard';

export const MyBookingsPage = () => {
    const [bookings, setBookings] = useState<BookingListItemDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const loadBookings = async () => {
            try {
                const data = await bookingApi.getMyBookings();
                setBookings(data);
            } catch (err) {
                console.error(err);
                setError('Не удалось загрузить бронирования');
            } finally {
                setLoading(false);
            }
        };

        loadBookings();
    }, []);

    return (
        <div className="min-h-screen bg-gray-50 pt-24 pb-16 dark:bg-gray-900">
            <div className="container mx-auto px-4">
                <h1 className="mb-8 text-3xl font-bold text-gray-900 dark:text-white">
                    Мои бронирования
                </h1>

                {loading && (
                    <div className="py-12 text-center text-gray-500">
                        Загрузка...
                    </div>
                )}

                {error && (
                    <div className="rounded-xl bg-red-50 p-4 text-red-700">
                        {error}
                    </div>
                )}

                {!loading && !error && bookings.length === 0 && (
                    <div className="rounded-2xl border border-gray-200 bg-white p-10 text-center dark:border-gray-700 dark:bg-gray-800">
                        <p className="text-gray-600 dark:text-gray-300">
                            У вас пока нет бронирований.
                        </p>
                    </div>
                )}

                <div className="space-y-5">
                    {bookings.map(booking => (
                        <BookingCard
                            key={booking.id}
                            booking={booking}
                        />
                    ))}
                </div>
            </div>
        </div>
    );
};