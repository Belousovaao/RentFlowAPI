import { differenceInDays, isAfter, isBefore, isValid, startOfDay } from 'date-fns';

export const calculateDays = (startDate: Date, endDate: Date): number => {
    return differenceInDays(endDate, startDate);
};

export const isValidDateRange = (startDate: Date | null, endDate: Date | null): boolean => {
    if (!startDate || !endDate) 
    {
        console.log('Invalid date range: startDate or endDate is null');
        return false;
    }
    
    const today = startOfDay(new Date());
    today.setHours(0, 0, 0, 0);
    
    return (
        isValid(startDate) &&
        isValid(endDate) &&
        !isBefore(startDate, today) &&
        isAfter(endDate, startDate)
    );
};

export const calculateMaxDiscountDailyPrice = (
    dailyPrice: number
): number => {
    const discountPercent = 25;

    const discounted = dailyPrice * (1 - discountPercent / 100);

    return Math.round(discounted / 100) * 100;
};