import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { User } from '../types';
import type { EmailRegisterPayload, PhoneRegisterPayload } from '../types';
import { normalizePhone } from '../validation';

interface AuthState {
    user: User | null;
    token: string | null;
    isAuthenticated: boolean;
    isLoading: boolean;

    /** Демо: «отправка» SMS — в проде заменить на API */
    loginWithEmail: (email: string, password: string) => Promise<void>;
    registerWithEmail: (data: EmailRegisterPayload) => Promise<void>;
    sendSmsCode: (phone: string) => Promise<void>;
    loginWithPhone: (phone: string, code: string) => Promise<void>;
    registerWithPhone: (data: PhoneRegisterPayload) => Promise<void>;

    logout: () => void;
    setUser: (user: User | null) => void;
    setToken: (token: string | null) => void;
}

const ADMIN_EMAIL = 'admin@rentflow.local';
const ADMIN_PASSWORD = 'admin123';
const DEMO_SMS_CODE = '000000';
const ADMIN_PHONE = '+79000000000';

function mockToken(): string {
    return `mock-jwt-${Date.now()}`;
}

function buildUser(partial: Partial<User> & Pick<User, 'id' | 'firstName' | 'lastName' | 'role'>): User {
    return {
        id: partial.id,
        email: partial.email,
        phone: partial.phone,
        firstName: partial.firstName,
        lastName: partial.lastName,
        role: partial.role,
    };
}

export const useAuthStore = create<AuthState>()(
    persist(
        (set) => ({
            user: null,
            token: null,
            isAuthenticated: false,
            isLoading: false,

            sendSmsCode: async (_phone: string) => {
                set({ isLoading: true });
                try {
                    await new Promise((r) => setTimeout(r, 600));
                    // В проде: POST /auth/sms/send { phone: _phone }
                } finally {
                    set({ isLoading: false });
                }
            },

            loginWithEmail: async (email: string, password: string) => {
                set({ isLoading: true });
                try {
                    await new Promise((r) => setTimeout(r, 500));
                    const normalizedEmail = email.trim().toLowerCase();
                    const isAdmin = normalizedEmail === ADMIN_EMAIL && password === ADMIN_PASSWORD;
                    const user = buildUser({
                        id: isAdmin ? 'admin-1' : 'user-email-1',
                        email: normalizedEmail,
                        firstName: isAdmin ? 'Admin' : 'Пользователь',
                        lastName: isAdmin ? 'RentFlow' : 'Тестовый',
                        role: isAdmin ? 'admin' : 'user',
                    });
                    set({
                        user,
                        token: mockToken(),
                        isAuthenticated: true,
                        isLoading: false,
                    });
                } catch (e) {
                    set({ isLoading: false });
                    throw e;
                }
            },

            registerWithEmail: async (data: EmailRegisterPayload) => {
                set({ isLoading: true });
                try {
                    await new Promise((r) => setTimeout(r, 500));
                    const user = buildUser({
                        id: `user-${Date.now()}`,
                        email: data.email.trim().toLowerCase(),
                        firstName: data.firstName.trim(),
                        lastName: data.lastName.trim(),
                        role: 'user',
                    });
                    set({
                        user,
                        token: mockToken(),
                        isAuthenticated: true,
                        isLoading: false,
                    });
                } catch (e) {
                    set({ isLoading: false });
                    throw e;
                }
            },

            loginWithPhone: async (phone: string, code: string) => {
                set({ isLoading: true });
                try {
                    await new Promise((r) => setTimeout(r, 500));
                    const n = normalizePhone(phone);
                    if (!n) throw new Error('Invalid phone');
                    if (code.trim() !== DEMO_SMS_CODE) {
                        set({ isLoading: false });
                        throw new Error('Invalid SMS code');
                    }
                    const isAdmin = n === ADMIN_PHONE;
                    const user = buildUser({
                        id: isAdmin ? 'admin-phone-1' : `user-phone-${n}`,
                        phone: n,
                        firstName: isAdmin ? 'Admin' : 'Пользователь',
                        lastName: isAdmin ? 'RentFlow' : 'Телефон',
                        role: isAdmin ? 'admin' : 'user',
                    });
                    set({
                        user,
                        token: mockToken(),
                        isAuthenticated: true,
                        isLoading: false,
                    });
                } catch (e) {
                    set({ isLoading: false });
                    throw e;
                }
            },

            registerWithPhone: async (data: PhoneRegisterPayload) => {
                set({ isLoading: true });
                try {
                    await new Promise((r) => setTimeout(r, 500));
                    const n = normalizePhone(data.phone);
                    if (!n) throw new Error('Invalid phone');
                    if (data.code.trim() !== DEMO_SMS_CODE) {
                        set({ isLoading: false });
                        throw new Error('Invalid SMS code');
                    }
                    const user = buildUser({
                        id: `user-phone-${Date.now()}`,
                        phone: n,
                        firstName: data.firstName.trim(),
                        lastName: data.lastName.trim(),
                        role: 'user',
                    });
                    set({
                        user,
                        token: mockToken(),
                        isAuthenticated: true,
                        isLoading: false,
                    });
                } catch (e) {
                    set({ isLoading: false });
                    throw e;
                }
            },

            logout: () => {
                set({
                    user: null,
                    token: null,
                    isAuthenticated: false,
                    isLoading: false,
                });
                localStorage.removeItem('auth-storage');
            },

            setUser: (user) => set({ user }),
            setToken: (token) => set({ token }),
        }),
        {
            name: 'auth-storage',
            partialize: (state) => ({
                user: state.user,
                token: state.token,
                isAuthenticated: state.isAuthenticated,
            }),
        }
    )
);
