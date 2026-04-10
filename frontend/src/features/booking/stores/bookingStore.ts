import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { Asset } from '../../../entities/asset/asset.types';
import { calculateTotalPrice, calculateDays, isValidDateRange } from '../utils/priceCalculator';

interface BookingDetails {
    asset: Asset | null;
    startDate: Date | null;
    endDate: Date | null;
    totalPrice: number;
    days: number;
    isValid: boolean;
}

interface BookingState {
    // Состояние
    currentBooking: BookingDetails;
    bookings: any[];
    isLoading: boolean;
    error: string | null;
    
    // Действия
    setBookingDetails: (details: Partial<BookingDetails>) => void;
    clearBooking: () => void;
    addBooking: (booking: any) => void;
    setLoading: (isLoading: boolean) => void;
    setError: (error: string | null) => void;
    recalculatePrice: () => void;
    validateBooking: () => boolean;
}

const initialState: BookingDetails = {
    asset: null,
    startDate: null,
    endDate: null,
    totalPrice: 0,
    days: 0,
    isValid: false,
};

export const useBookingStore = create<BookingState>()(
    persist(
        (set, get) => ({
            currentBooking: initialState,
            bookings: [],
            isLoading: false,
            error: null,
            
            setBookingDetails: (details) => {
                set((state) => {
                    const updatedBooking = { ...state.currentBooking, ...details };
                    
                    // Пересчитываем цену, если есть актив и даты
                    if (updatedBooking.asset && updatedBooking.startDate && updatedBooking.endDate) {
                        const days = calculateDays(updatedBooking.startDate, updatedBooking.endDate);
                        const totalPrice = calculateTotalPrice(
                            updatedBooking.asset.dailyPrice,
                            updatedBooking.startDate,
                            updatedBooking.endDate
                        );
                        const isValid = isValidDateRange(updatedBooking.startDate, updatedBooking.endDate);
                        
                        updatedBooking.days = days;
                        updatedBooking.totalPrice = totalPrice;
                        updatedBooking.isValid = isValid;
                    } else {
                        updatedBooking.days = 0;
                        updatedBooking.totalPrice = 0;
                        updatedBooking.isValid = false;
                    }
                    
                    return { currentBooking: updatedBooking };
                });
            },
            
            recalculatePrice: () => {
                const { currentBooking } = get();
                if (currentBooking.asset && currentBooking.startDate && currentBooking.endDate) {
                    const days = calculateDays(currentBooking.startDate, currentBooking.endDate);
                    const totalPrice = calculateTotalPrice(
                        currentBooking.asset.dailyPrice,
                        currentBooking.startDate,
                        currentBooking.endDate
                    );
                    const isValid = isValidDateRange(currentBooking.startDate, currentBooking.endDate);
                    
                    set({
                        currentBooking: {
                            ...currentBooking,
                            days,
                            totalPrice,
                            isValid,
                        },
                    });
                }
            },
            
            validateBooking: () => {
                const { currentBooking } = get();
                return currentBooking.isValid && currentBooking.asset !== null;
            },
            
            clearBooking: () => {
                set({ currentBooking: initialState });
            },
            
            addBooking: (booking) => {
                set((state) => ({
                    bookings: [...state.bookings, booking],
                    currentBooking: initialState,
                }));
            },
            
            setLoading: (isLoading) => {
                set({ isLoading });
            },
            
            setError: (error) => {
                set({ error });
            },
        }),
        {
            name: 'booking-storage',
            partialize: (state) => ({
                bookings: state.bookings,
            }),
        }
    )
);