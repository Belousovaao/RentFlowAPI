import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { authApi, RegisterRequest, type OtpChannel, } from '../api/auth.api';
import type { RegisterFormData, } from '../types';
import type { User } from '../types';

interface OtpSession {
    challengeId: string;
    channel: OtpChannel;
    destination: string;
    expiresAt: number;
    retryAvailableAt: number | null;
}

interface PendingRegistration {
    registrationTicket: string;
    channel: OtpChannel;
    destination: string;
}

interface AuthState {
    user: User | null;
    token: string | null;
    isAuthenticated: boolean;
    isLoading: boolean;
    otpSession: OtpSession | null;
    pendingRegistration: PendingRegistration | null;

    sendOtp: (
        channel: OtpChannel,
        destination: string
    ) => Promise<void>;

    verifyOtp: (
        code: string
    ) => Promise<'authenticated' | 'registration'>;

    register: (
        data: RegisterFormData
    ) => Promise<void>;

    loadCurrentUser: () => Promise<void>;

    logout: () => void;

    resetOtpFlow: () => void;
}

export const useAuthStore = create<AuthState>()(
    persist(
        (set, get) => ({
            user: null,
            token: null,
            isAuthenticated: false,
            isLoading: false,

            otpSession: null,
            pendingRegistration: null,

            sendOtp: async (channel, destination) => {
                set({ isLoading: true });

                try {
                    const response = await authApi.sendOtp(
                        channel,
                        destination
                    );

                    const now = Date.now();

                    set({
                        otpSession: {
                            challengeId: response.challengeId,
                            channel,
                            destination,
                            expiresAt: now + response.expiresIn * 1000,
                            retryAvailableAt: response.retryAfter > 0
                                ? now + response.retryAfter * 1000
                                : null,
                        },
                        pendingRegistration: null,
                    });
                } finally {
                    set({ isLoading: false });
                }
            },

            verifyOtp: async (code) => {
                const { otpSession, } = get();

                if (!otpSession) {
                    throw new Error(
                        'OTP session is not initialized.'
                    );
                }

                if (Date.now() >= otpSession.expiresAt) {
                    throw new Error(
                        'OTP challenge has expired.'
                    );
                }

                set({ isLoading: true });

                try {
                    const response =
                        await authApi.verifyOtp(
                            otpSession.challengeId,
                            otpSession.channel,
                            otpSession.destination,
                            code
                        );

                    if (
                        response.status === 'Authenticated'
                    ) {
                        if (!response.token)
                            throw new Error('Authentication token was not returned.');
                        set({
                            token: response.token,
                            isAuthenticated: true,
                            otpSession: null,
                            pendingRegistration: null,
                        });

                        await get().loadCurrentUser();

                        return 'authenticated';
                    }

                    if (
                        response.status ===
                        'RegistrationRequired'
                    ) {
                        if (
                            !response.registrationTicket
                        ) {
                            throw new Error(
                                'Registration ticket was not returned.'
                            );
                        }

                        set({
                            pendingRegistration: {
                                registrationTicket:
                                    response.registrationTicket,
                                channel:
                                    otpSession.channel,
                                destination:
                                    otpSession.destination,
                            },
                            otpSession: null,
                        });

                        return 'registration';
                    }

                    throw new Error(
                        'Unexpected OTP authentication status.'
                    );
                } finally {
                    set({ isLoading: false });
                }
            },

            register: async (data) => {
                const {pendingRegistration,} = get();

                if (!pendingRegistration)
                    throw new Error('Registration session is not initialized.');

                if (!data.phone?.trim())
                    throw new Error('Phone is required.');

                if (!data.consentDocumentId || !data.personalDataConsentAccepted) 
                {
                    throw new Error('Personal data processing consent is required.');
                }

                const request : RegisterRequest = {
                    registrationTicket: pendingRegistration.registrationTicket,
                    customerType: data.customerType,
                    email: data.email?.trim() || undefined,
                    phone: data.phone.trim(),
                    firstName: data.firstName?.trim() || undefined,
                    lastName: data.lastName?.trim() || undefined,
                    inn: data.inn?.trim() || undefined,
                    consentDocumentId: data.consentDocumentId,
                    personalDataConsentAccepted: data.personalDataConsentAccepted,
                };

                set({ isLoading: true });

                try {
                    const response = await authApi.register(request);
                    
                    if (!response.token) 
                        throw new Error('Authentication token was not returned.');

                    set({
                        token: response.token,
                        isAuthenticated: true,
                        pendingRegistration: null,
                        otpSession: null,
                    });

                    await get().loadCurrentUser();
                } finally {
                    set({ isLoading: false });
                }
            },

            loadCurrentUser: async () => {
                const {token,} = get();

                if (!token) {
                    set({
                        user: null,
                        isAuthenticated: false,
                    });
                    return;
                }

                try {
                    const response = await authApi.getMe();
                    const user: User = {
                        id: response.userId,
                        firstName: response.firstName,
                        lastName: response.lastName,
                        email: response.email,
                        phone: response.phone,
                        role: response.role,
                        customerId: response.customerId,
                    };

                    set({
                        user,
                        isAuthenticated: true,
                    });
                } catch {
                    get().logout();
                    throw new Error('Failed to load current user.');
                }
            },

            logout: () => {
                localStorage.removeItem('token');

                set({
                    user: null,
                    token: null,
                    isAuthenticated: false,
                    isLoading: false,
                    otpSession: null,
                    pendingRegistration: null,
                });
            },

            resetOtpFlow: () => {
                set({
                    otpSession: null,
                    pendingRegistration: null,
                });
            },
        }),

        {
            name: 'auth-storage',

            partialize: (state) => ({
                user: state.user,
                token: state.token,
                isAuthenticated:
                    state.isAuthenticated,
            }),
        }
    )
);