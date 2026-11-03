import type { BookingStatus } from "@/entities/booking/booking.types";
import { useI18n } from "@/app/providers/I18nProvider";

interface Props {
    status: BookingStatus;
}

const statusConfig: Record<BookingStatus, { labelKey: string; className: string }> = 
{
    Pending: {
        labelKey: 'booking.status.pending',
        className: 'bg-yellow-100 text-yellow-800',
    },
    Confirmed: {
        labelKey: 'booking.status.confirmed',
        className: 'bg-blue-100 text-blue-800',
    },
    Active: {
        labelKey: 'booking.status.active',
        className: 'bg-green-100 text-green-800',
    },
    Completed: {
        labelKey: 'booking.status.completed',
        className: 'bg-gray-100 text-gray-700',
    },
    Cancelled: {
        labelKey: 'booking.status.cancelled',
        className: 'bg-red-100 text-red-800',
    },
};

export const BookingStatusBadge = ({ status }: Props) => {
    const config = statusConfig[status];
    const { t } = useI18n();

    return (
        <span
            className={`inline-flex rounded-full px-3 py-1 text-xs font-semibold ${config.className}`}
        >
            {t(config.labelKey)}
        </span>
    );
};