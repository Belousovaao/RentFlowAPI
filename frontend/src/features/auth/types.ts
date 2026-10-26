import { CustomerType, } from "@/entities/customer/customer.type";

export type UserRole = 'user' | 'admin';

export interface User {
    id: string;
    email?: string;
    phone?: string;
    firstName: string;
    lastName: string;
    role: UserRole;
    customerId?: string;
}

export interface RegisterFormData {
    customerType: CustomerType;
    email?: string;
    phone?: string;
    firstName?: string;
    lastName?: string;
    inn?: string;
    consentDocumentId: string;
    personalDataConsentAccepted: boolean;
}

export type AuthMethod = 'phone' | 'email';