export type UserRole = 'user' | 'admin';

export interface User {
    id: string;
    email?: string;
    phone?: string;
    firstName: string;
    lastName: string;
    role: UserRole;
}

export type AuthMethod = 'phone' | 'email';

export interface EmailRegisterPayload {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
}

export interface PhoneRegisterPayload {
    phone: string;
    code: string;
    firstName: string;
    lastName: string;
}
