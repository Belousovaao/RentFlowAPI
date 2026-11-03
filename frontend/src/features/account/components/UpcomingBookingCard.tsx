import { ArrowRight, Calendar, Car, Clock } from 'lucide-react';
import { Link } from 'react-router-dom';
import type { BookingListItemDto } from '@/entities/booking/booking.types';
import { BookingStatusBadge } from '@/features/booking/components/BookingStatusBage';
import { getPhotoUrl } from '@/shared/utils/photoUrl';
import { useI18n } from '@/app/providers/I18nProvider';

interface Props {
    bookings: BookingListItemDto[];
}

export const UpcomingBookingCard = ({ bookings }: Props) => {
    const { t, language } = useI18n();
    const booking = getFeaturedBooking(bookings);

    if (!booking) {
        return (
            <section className="relative overflow-hidden rounded-2xl border border-gray-200 bg-white shadow-sm dark:border-gray-700 dark:bg-gray-800">
                <div className="flex min-h-[280px] flex-col justify-between p-6 md:p-8">
                    <div>
                        <p className="text-sm font-medium uppercase tracking-wider text-primary-600 dark:text-primary-400">
                            {t("account.upcoming.nextTrip")}
                        </p>

                        <h2 className="mt-3 text-2xl font-bold text-gray-900 dark:text-white">
                            {t("account.upcoming.whereNext")}
                        </h2>

                        <p className="mt-2 max-w-md text-gray-500 dark:text-gray-400">
                            {t("account.upcoming.description")}
                        </p>
                    </div>

                    <Link
                        to="/assets"
                        className="mt-8 inline-flex w-fit items-center gap-2 rounded-xl bg-primary-600 px-5 py-3 text-sm font-medium text-white transition-colors hover:bg-primary-700"
                    >
                        {t("account.bookings.chooseVehicle")}
                        <ArrowRight className="h-4 w-4" />
                    </Link>
                </div>
            </section>
        );
    }

    const isActive = booking.status === 'Active';

    return (
        <section className="overflow-hidden rounded-2xl border border-gray-200 bg-white shadow-sm dark:border-gray-700 dark:bg-gray-800">
            <div className="grid min-h-[320px] md:grid-cols-[0.9fr_1.1fr]">
                <div className="relative h-[320px] overflow-hidden bg-gray-100 dark:bg-gray-700 md:h-auto">
                    {booking.asset.coverPhotoUrl ? (
                        <img
                            src={getPhotoUrl(booking.asset.coverPhotoUrl)}
                            alt={`${booking.asset.brandName} ${booking.asset.model}`}
                            className="absolute inset-0 h-full w-full object-cover"
                        />
                    ) : (
                        <div className="flex h-full items-center justify-center">
                            <Car className="h-16 w-16 text-gray-400" />
                        </div>
                    )}

                    <div className="absolute inset-0 bg-gradient-to-t from-black/30 to-transparent" />

                    <div className="absolute bottom-1 left-1/2 w-full -translate-x-1/2 px-4 text-center">
                        <h2 className="text-2xl font-bold text-white">
                            {booking.asset.brandName} {booking.asset.model}
                        </h2>
                    </div>
                </div>

                <div className="flex flex-col justify-between p-6 md:p-8">
                    <div>
                        <div className="flex flex-wrap items-center justify-between gap-3">
                            <p className="text-sm font-medium uppercase tracking-wider text-primary-600 dark:text-primary-400">
                                {isActive ? t('account.upcoming.inTrip') : t('account.upcoming.yourTrip')}
                            </p>

                            <BookingStatusBadge status={booking.status} />
                        </div>

                        <div className="mt-6">
                            <div className="flex items-center gap-3 text-gray-700 dark:text-gray-200">
                                <Calendar className="h-5 w-5 text-gray-400" />

                                <span className="text-lg font-medium">
                                    {formatDate(booking.rentalPeriod.startDate, language)}
                                    {' — '}
                                    {formatDate(booking.rentalPeriod.endDate, language)}
                                </span>
                            </div>

                            <div className="mt-3 flex items-center gap-3 text-sm text-gray-500 dark:text-gray-400">
                                <Clock className="h-4 w-4" />

                                <span>
                                    {isActive
                                        ? t('account.upcoming.rentalStarted')
                                        : getDaysUntil(booking.rentalPeriod.startDate, t, language)}
                                </span>
                            </div>
                        </div>
                    </div>

                    <Link
                        to={`/account/bookings/${booking.id}`}
                        className="mt-8 inline-flex items-center gap-2 text-sm font-medium text-primary-600 transition-colors hover:text-primary-700 dark:text-primary-400 dark:hover:text-primary-300"
                    >
                        {t("common.details")}
                        <ArrowRight className="h-4 w-4 transition-transform group-hover:translate-x-1" />
                    </Link>
                </div>
            </div>
        </section>
    );
};

const getFeaturedBooking = (bookings: BookingListItemDto[],): BookingListItemDto | null => {
    const activeBooking = bookings.find(
        booking => booking.status === 'Active',
    );

    if (activeBooking) {
        return activeBooking;
    }

    const now = new Date();

    const upcomingBookings = bookings
        .filter(booking => {
            if (booking.status !== 'Confirmed' && booking.status !== 'Pending') {
                return false;
            }

            return new Date(booking.rentalPeriod.startDate) > now;
        })
        .sort(
            (a, b) =>
                new Date(a.rentalPeriod.startDate).getTime() -
                new Date(b.rentalPeriod.startDate).getTime(),
        );

    return upcomingBookings[0] ?? null;
};

const formatDate = (value: string, language: string) =>
    new Intl.DateTimeFormat(language === 'zh' ? 'zh-CN' : language === 'en' ? 'en-GB' : 'ru-RU', {
        day: 'numeric',
        month: 'long',
    }).format(new Date(value));

const getDaysUntil = (value: string, t: (key: string) => string, language: string): string => {
    const now = new Date();
    const start = new Date(value);

    const diff = start.getTime() - now.getTime();
    const days = Math.ceil(diff / (1000 * 60 * 60 * 24));

    if (days <= 0) {
        return t('account.upcoming.startsToday');
    }

    if (days === 1) {
        return t('account.upcoming.oneDay');
    }

    if (days < 5) {
        return `${t('account.upcoming.daysUntil')} ${days}`;
    }

    return `${t('account.upcoming.daysUntil')} ${days}`;
};