import { CalendarDays, Car } from 'lucide-react';
import type { BookingListItemDto } from '@/entities/booking/booking.types';
import { getPhotoUrl } from '@/shared/utils/photoUrl';

interface Props {
    bookings: BookingListItemDto[];
}

export const BookingHistory = ({ bookings }: Props) => {
    const history = bookings
        .filter(
            booking =>
                booking.status === 'Completed' ||
                booking.status === 'Active',
        )
        .sort(
            (a, b) =>
                new Date(a.rentalPeriod.startDate).getTime() -
                new Date(b.rentalPeriod.startDate).getTime(),
        );

    if (history.length === 0) {
        return null;
    }

    return (
        <section>
            <div className="mb-5">
                <h2 className="text-2xl font-bold text-gray-900 dark:text-white">
                    Ваша история поездок
                </h2>

                <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                    Автомобили, на которых вы уже ездили
                </p>
            </div>

            <div className="overflow-x-auto pb-2">
                <div className="flex min-w-max items-start">
                    {history.map((booking, index) => (
                        <HistoryItem
                            key={booking.id}
                            booking={booking}
                            isLast={index === history.length - 1}
                        />
                    ))}
                </div>
            </div>
        </section>
    );
};

interface HistoryItemProps {
    booking: BookingListItemDto;
    isLast: boolean;
}

const HistoryItem = ({
    booking,
    isLast,
}: HistoryItemProps) => {
    return (
        <div className="flex w-[220px] items-start">
            <div className="w-full">
                <div className="relative flex items-center">
                    <div className="h-3 w-3 shrink-0 rounded-full bg-primary-600 ring-4 ring-primary-100 dark:bg-primary-400 dark:ring-primary-900/30" />

                    {!isLast && (
                        <div className="h-px flex-1 bg-gray-200 dark:bg-gray-700" />
                    )}
                </div>

                <div className="mt-4 pr-5">
                    <div className="relative h-28 overflow-hidden rounded-xl bg-gray-100 dark:bg-gray-700">
                        {booking.asset.coverPhotoUrl ? (
                            <img
                                src={getPhotoUrl(
                                    booking.asset.coverPhotoUrl,
                                )}
                                alt={`${booking.asset.brandName} ${booking.asset.model}`}
                                className="h-full w-full object-cover"
                            />
                        ) : (
                            <div className="flex h-full items-center justify-center">
                                <Car className="h-8 w-8 text-gray-400" />
                            </div>
                        )}
                    </div>

                    <h3 className="mt-3 font-semibold text-gray-900 dark:text-white">
                        {booking.asset.brandName}{' '}
                        {booking.asset.model}
                    </h3>

                    <div className="mt-1 flex items-center gap-1.5 text-sm text-gray-500 dark:text-gray-400">
                        <CalendarDays className="h-4 w-4 shrink-0" />

                        <span>
                            {formatDate(
                                booking.rentalPeriod.startDate,
                            )}
                            {' — '}
                            {formatDate(
                                booking.rentalPeriod.endDate,
                            )}
                        </span>
                    </div>
                </div>
            </div>
        </div>
    );
};

const formatDate = (value: string): string =>
    new Intl.DateTimeFormat('ru-RU', {
        day: 'numeric',
        month: 'short',
    }).format(new Date(value));