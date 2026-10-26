import { ArrowRight, Calendar, Car, CreditCard } from 'lucide-react';
import { Link } from 'react-router-dom';
import type { BookingListItemDto } from '@/entities/booking/booking.types';
import { BookingStatusBadge } from '@/features/booking/components/BookingStatusBage';
import { getPhotoUrl } from '@/shared/utils/photoUrl';
import { formatPrice } from '@/shared/utils/formatters';

interface Props {
    booking: BookingListItemDto;
    compact?: boolean;
}

export const BookingCard = ({ booking, compact = false }: Props) => {
    const { asset, rentalPeriod, price, services, status } = booking;

    if (compact) {
        return (
            <Link
                to={`/account/bookings/${booking.id}`}
                className="group block rounded-2xl border border-gray-200 bg-white p-4 transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-800"
            >
                <div className="flex items-center gap-4">
                    <div className="h-20 w-24 shrink-0 overflow-hidden rounded-xl bg-gray-100 dark:bg-gray-700">
                        {asset.coverPhotoUrl ? (
                            <img
                                src={getPhotoUrl(asset.coverPhotoUrl)}
                                alt={`${asset.brandName} ${asset.model}`}
                                className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-105"
                            />
                        ) : (
                            <div className="flex h-full items-center justify-center">
                                <Car className="h-8 w-8 text-gray-400" />
                            </div>
                        )}
                    </div>

                    <div className="min-w-0 flex-1">
                        <div className="flex flex-wrap items-center gap-2">
                            <h3 className="truncate font-semibold text-gray-900 dark:text-white">
                                {asset.brandName} {asset.model}
                            </h3>

                            <BookingStatusBadge status={status} />
                        </div>

                        <div className="mt-2 flex flex-wrap items-center gap-x-4 gap-y-1 text-sm text-gray-500 dark:text-gray-400">
                            <span className="flex items-center gap-1.5">
                                <Calendar className="h-4 w-4" />
                                {formatDate(rentalPeriod.startDate)}
                                {' — '}
                                {formatDate(rentalPeriod.endDate)}
                            </span>

                            <span className="font-medium text-gray-700 dark:text-gray-200">
                                {formatPrice(price.totalToPay)}
                            </span>
                        </div>
                    </div>

                    <ArrowRight className="hidden h-5 w-5 shrink-0 text-gray-400 transition-transform group-hover:translate-x-1 sm:block" />
                </div>
            </Link>
        );
    }

    return (
        <Link
            to={`/account/bookings/${booking.id}`}
            className="group block rounded-2xl border border-gray-200 bg-white p-6 shadow-sm transition-shadow hover:shadow-md dark:border-gray-700 dark:bg-gray-800"
        >
            <div className="flex flex-col gap-5 md:flex-row">
                <div className="h-32 w-full overflow-hidden rounded-xl bg-gray-100 md:w-48">
                    {asset.coverPhotoUrl ? (
                        <img
                            src={getPhotoUrl(asset.coverPhotoUrl)}
                            alt={`${asset.brandName} ${asset.model}`}
                            className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-105"
                        />
                    ) : (
                        <div className="flex h-full items-center justify-center">
                            <Car className="h-10 w-10 text-gray-400" />
                        </div>
                    )}
                </div>

                <div className="flex-1">
                    <div className="flex flex-col justify-between gap-3 sm:flex-row">
                        <div>
                            <h2 className="text-xl font-semibold text-gray-900 dark:text-white">
                                {asset.brandName} {asset.model}
                            </h2>

                            <p className="text-sm text-gray-500">
                                {asset.year}
                            </p>
                        </div>

                        <BookingStatusBadge status={status} />
                    </div>

                    <div className="mt-5 grid gap-3 text-sm sm:grid-cols-2">
                        <div className="flex items-center gap-2 text-gray-600 dark:text-gray-300">
                            <Calendar className="h-4 w-4" />
                            <span>
                                {formatDate(rentalPeriod.startDate)}
                                {' — '}
                                {formatDate(rentalPeriod.endDate)}
                            </span>
                        </div>

                        <div className="flex items-center gap-2 text-gray-600 dark:text-gray-300">
                            <CreditCard className="h-4 w-4" />
                            <span>
                                Итого: {formatPrice(price.totalToPay)}
                            </span>
                        </div>
                    </div>

                    {services.length > 0 && (
                        <div className="mt-4">
                            <p className="mb-2 text-sm font-medium text-gray-700 dark:text-gray-200">
                                Дополнительные услуги
                            </p>

                            <ul className="space-y-1 text-sm text-gray-500">
                                {services.map(service => (
                                    <li key={service.additionalServiceId}>
                                        {service.name} — {formatPrice(service.price)}
                                    </li>
                                ))}
                            </ul>
                        </div>
                    )}
                </div>

                <ArrowRight className="hidden h-5 w-5 shrink-0 self-center text-gray-400 transition-transform group-hover:translate-x-1 md:block" />
            </div>
        </Link>
    );
};
const formatDate = (value: string) => new Intl.DateTimeFormat('ru-RU').format(new Date(value));
