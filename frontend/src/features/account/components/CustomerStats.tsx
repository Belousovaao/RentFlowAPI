import { CalendarDays, Car, Route } from 'lucide-react';
import type { BookingListItemDto } from '@/entities/booking/booking.types';

interface Props {
    bookings: BookingListItemDto[];
}

export const CustomerStats = ({ bookings }: Props) => {
    const totalTrips = bookings.length;
    const totalDays = bookings.reduce((sum, booking) => {
        const start = new Date(booking.rentalPeriod.startDate);
        const end = new Date(booking.rentalPeriod.endDate);
        const days = Math.ceil((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24),);

        return sum + Math.max(days, 0);
    }, 0);

    const uniqueCars = new Set(bookings.map(booking => booking.asset.id),).size;

    return (
        <section>
            <div className="grid gap-3 sm:grid-cols-3">
                <StatCard
                    icon={Route}
                    value={totalTrips}
                    label={getTripsLabel(totalTrips)}
                />

                <StatCard
                    icon={CalendarDays}
                    value={totalDays}
                    label={getDaysLabel(totalDays)}
                />

                <StatCard
                    icon={Car}
                    value={uniqueCars}
                    label={getCarsLabel(uniqueCars)}
                />
            </div>
        </section>
    );
};

interface StatCardProps {
    icon: typeof Route;
    value: number;
    label: string;
}

const StatCard = ({
    icon: Icon,
    value,
    label,
}: StatCardProps) => {
    return (
        <div className="rounded-2xl border border-gray-200 bg-white p-5 shadow-sm dark:border-gray-700 dark:bg-gray-800">
            <div className="flex items-center gap-3">
                <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-primary-100 text-primary-600 dark:bg-primary-900/30 dark:text-primary-400">
                    <Icon className="h-5 w-5" />
                </div>

                <div>
                    <div className="text-2xl font-bold text-gray-900 dark:text-white">
                        {value}
                    </div>

                    <div className="text-sm text-gray-500 dark:text-gray-400">
                        {label}
                    </div>
                </div>
            </div>
        </div>
    );
};

const getTripsLabel = (count: number): string => {
    if (count % 10 === 1 && count % 100 !== 11) {
        return 'поездка';
    }

    if (
        count % 10 >= 2 &&
        count % 10 <= 4 &&
        (count % 100 < 10 || count % 100 >= 20)
    ) {
        return 'поездки';
    }

    return 'поездок';
};

const getDaysLabel = (count: number): string => {
    if (count % 10 === 1 && count % 100 !== 11) {
        return 'день аренды';
    }

    if (
        count % 10 >= 2 &&
        count % 10 <= 4 &&
        (count % 100 < 10 || count % 100 >= 20)
    ) {
        return 'дня аренды';
    }

    return 'дней аренды';
};

const getCarsLabel = (count: number): string => {
    if (count % 10 === 1 && count % 100 !== 11) {
        return 'автомобиль';
    }

    if (
        count % 10 >= 2 &&
        count % 10 <= 4 &&
        (count % 100 < 10 || count % 100 >= 20)
    ) {
        return 'автомобиля';
    }

    return 'автомобилей';
};