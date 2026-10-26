import { create } from 'zustand';

interface AuthModalState {
    isOpen: boolean;
    onSuccess: (() => void) | null;
    open: (onSuccess?: () => void) => void;
    close: () => void;
    completeAuth: () => void;
}

export const useAuthModalStore = create<AuthModalState>((set, get) => ({
    isOpen: false,
    onSuccess: null,
    open: (onSuccess) => {
        console.log('AUTH STORE: OPEN');
        set({ isOpen: true, onSuccess: onSuccess ?? null });
    },
    close: () => {
        console.log('AUTH STORE: CLOSE');
        set({ isOpen: false, onSuccess: null });
    },
    completeAuth: () => {
        console.log('AUTH STORE: COMPLETE AUTH');
        const { onSuccess } = get();
        set({ isOpen: false, onSuccess: null });
        onSuccess?.();
    },
}));
