import { differenceInDays, isAfter, isBefore, isValid } from 'date-fns';

export const calculateTotalPrice = (
    dailyPrice: number,
    startDate: Date,
    endDate: Date
): number => {
    const days = calculateDays(startDate, endDate);
    return dailyPrice * days;
};

export const calculateDays = (startDate: Date, endDate: Date): number => {
    return differenceInDays(endDate, startDate);
};

export const isValidDateRange = (startDate: Date | null, endDate: Date | null): boolean => {
    if (!startDate || !endDate) return false;
    
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    
    return (
        isValid(startDate) &&
        isValid(endDate) &&
        isAfter(startDate, today) && // или isSameOrAfter если нужно
        isAfter(endDate, startDate)
    );
};