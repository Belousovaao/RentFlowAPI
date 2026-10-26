import { create } from 'zustand';
import { createJSONStorage, persist } from 'zustand/middleware';
import type { Asset } from '../../../entities/asset/asset.types';
import type { SelectedAdditionalService } from '@/entities/service/service.types';

interface BookingDetails {
    asset: Asset | null;
    startDate: Date | null;
    endDate: Date | null;
    selectedServices: SelectedAdditionalService[];
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

}

const initialState: BookingDetails = {
    asset: null,
    startDate: null,
    endDate: null,
    selectedServices: [],
};

export const useBookingStore = create<BookingState>()(
    persist(
        (set) => ({
            currentBooking: initialState,
            bookings: [],
            isLoading: false,
            error: null,
            
            setBookingDetails: (details) => {
                set(state => ({
                    currentBooking: {
                        ...state.currentBooking,
                        ...details
                    }
                }));
            },
            clearBooking: () => {
                set({
                    currentBooking: initialState
                });
            },
            addBooking: (booking) => {
                set(state => ({
                    bookings: [...state.bookings, booking],
                    currentBooking: initialState
                }));
            },
            setLoading: (isLoading) => {set({isLoading});},
            setError: (error) => {set({error});}
        }),
        {
            name: 'booking-storage',
            storage: createJSONStorage(() => localStorage, {
                reviver: (key, value) => {
                    if (
                        (key === 'startDate' || key === 'endDate') &&
                        typeof value === 'string'
                    ) {
                        const date = new Date(value);
                        return Number.isNaN(date.getTime()) ? null : date;
                    }

                    return value;
                },
            }),
            partialize: (state) => ({
                currentBooking: state.currentBooking,
                bookings: state.bookings,
            }),
        }
    )
);