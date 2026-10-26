import type { CustomerType } from '@/entities/customer/customer.type';
import api from '@/shared/api/axios';

export interface PersonalDataConsentDocument {
    id: string;
    version: string;
    title: string;
    content: string;
    contentHash: string;
    publishedAt: string;
}

export type OtpChannel = 'email' | 'phone';

export type OtpAuthenticationStatus =
    | 'Authenticated'
    | 'RegistrationRequired';

export interface SendOtpResponse {
    challengeId: string;
    expiresIn: number;
    retryAfter: number;
}

export interface VerifyOtpResponse {
    success: boolean;
    status: OtpAuthenticationStatus;
    token: string | null;
    registrationTicket: string | null;
}

export interface RegisterRequest {
    registrationTicket: string;
    customerType: CustomerType;
    email?: string;
    phone: string;
    firstName?: string;
    lastName?: string;
    inn?: string;
    consentDocumentId: string;
    personalDataConsentAccepted: boolean;
}

export interface RegisterResponse {
    success: boolean;
    token: string;
}

export interface MeResponse {
    userId: string;
    firstName: string;
    lastName: string;
    email?: string;
    phone?: string;
    role: 'user' | 'admin';
    customerId?: string;
}

function persistToken(token: string) {
    localStorage.setItem('token', token);
}

export const authApi = {
    async sendOtp(
        channel: OtpChannel,
        destination: string
    ): Promise<SendOtpResponse> {
        const { data } = await api.post<SendOtpResponse>(
            '/auth/otp/send',
            {
                channel,
                destination,
            }
        );

        return data;
    },

    async verifyOtp(
        challengeId: string,
        channel: OtpChannel,
        destination: string,
        code: string
    ): Promise<VerifyOtpResponse> {
        const { data } = await api.post<VerifyOtpResponse>(
            '/auth/otp/verify',
            {
                challengeId,
                channel,
                destination,
                code,
            }
        );

        if (data.token) {
            persistToken(data.token);
        }

        return data;
    },

    async register(request: RegisterRequest): Promise<RegisterResponse> {
        const { data } = await api.post<RegisterResponse>(
            '/auth/register',
            request
        );

        persistToken(data.token);

        return data;
    },

    async getMe(): Promise<MeResponse> {
        const { data } = await api.get<MeResponse>('/auth/me');

        return data;
    },

    async getPersonalDataProcessingConsent(): Promise<PersonalDataConsentDocument> {
        const { data } = await api.get<PersonalDataConsentDocument>(
            '/consent-documents/personal-data-processing'
        );

        return data;
    },
};