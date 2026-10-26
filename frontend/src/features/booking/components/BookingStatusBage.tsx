import type { BookingStatus } from "@/entities/booking/booking.types";

interface Props {
    status: BookingStatus;
}

const statusConfig: Record<BookingStatus, { label: string; className: string }> = 
{
    Pending: {
        label: 'На рассмотрении',
        className: 'bg-yellow-100 text-yellow-800',
    },
    Confirmed: {
        label: 'Одобрено',
        className: 'bg-blue-100 text-blue-800',
    },
    Active: {
        label: 'Активно',
        className: 'bg-green-100 text-green-800',
    },
    Completed: {
        label: 'Завершено',
        className: 'bg-gray-100 text-gray-700',
    },
    Cancelled: {
        label: 'Отменено',
        className: 'bg-red-100 text-red-800',
    },
};

export const BookingStatusBadge = ({ status }: Props) => {
    const config = statusConfig[status];

    return (
        <span
            className={`inline-flex rounded-full px-3 py-1 text-xs font-semibold ${config.className}`}
        >
            {config.label}
        </span>
    );
};