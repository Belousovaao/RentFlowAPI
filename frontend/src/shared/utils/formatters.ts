import { TranslateFunction } from '@/shared/config/i18n';

export const formatPrice = (price: number, currency: string = '₽'): string => {
    return `${price.toLocaleString('ru-RU')} ${currency}`;
};

export const formatDate = (date: Date): string => {
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();
    return `${day}.${month}.${year}`;
};

export const formatDateTime = (date: Date): string => {
    return date.toLocaleString("ru-RU", {
        day: "2-digit",
        month: "2-digit",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
    });
};

export function formatDateForBack(date: Date): string {
    return [
        date.getFullYear(),
        String(date.getMonth() + 1).padStart(2, "0"),
        String(date.getDate()).padStart(2, "0"),
    ].join("-");
}

export const getDaysLabel = (
    days: number,
    t: TranslateFunction
): string => {
    const lastDigit = days % 10;
    const lastTwoDigits = days % 100;

    if (lastTwoDigits >= 11 && lastTwoDigits <= 19) {
        return t('booking.days');
    }

    if (lastDigit === 1) {
        return t('booking.day1');
    }

    return t('booking.days');
};

export const parseDateForInput = (value: string): Date | undefined => {
    if (!value) {
        return undefined;
    }

    const match = /^(\d{4})-(\d{2})-(\d{2})$/.exec(value);

    if (!match) {
        return undefined;
    }

    const [, year, month, day] = match;

    const date = new Date(
        Number(year),
        Number(month) - 1,
        Number(day)
    );

    if (
        date.getFullYear() !== Number(year) ||
        date.getMonth() !== Number(month) - 1 ||
        date.getDate() !== Number(day)
    ) {
        return undefined;
    }

    return date;
};

export const formatDateInput = (value: string): string => {
    const digits = value
        .replace(/\D/g, '')
        .slice(0, 8);

    if (digits.length <= 2) {
        return digits;
    }

    if (digits.length <= 4) {
        return `${digits.slice(0, 2)}.${digits.slice(2)}`;
    }

    return `${digits.slice(0, 2)}.${digits.slice(2, 4)}.${digits.slice(4)}`;
};

export const parseDateInput = (value: string): Date | undefined => {
    const digits = value
        .replace(/\D/g, '')
        .slice(0, 8);

    if (digits.length !== 8) {
        return undefined;
    }

    const day = Number(digits.slice(0, 2));
    const month = Number(digits.slice(2, 4));
    const year = Number(digits.slice(4, 8));

    const date = new Date(
        year,
        month - 1,
        day
    );

    if (
        date.getFullYear() !== year ||
        date.getMonth() !== month - 1 ||
        date.getDate() !== day
    ) {
        return undefined;
    }

    return date;
};