import { BookingListItemDto } from "../booking/booking.types";

export enum CustomerType {
    Individual = "Individual",
    IndividualEntrepreneur = "IndividualEntrepreneur",
    Organization = "Organization",
}

export enum DocumentType {
    PassportFront = 0,
    PassportRegistration = 1,
    DriverLicenseFront = 2,
    DriverLicenseBack = 3
}

export enum VerificationStatus {
    NotSubmitted = "NotSubmitted",
    Pending = "Pending",
    Approved = "Approved",
    Rejected = "Rejected"
}

export interface CustomerLookup {
    id: string;
    type: CustomerType;
    displayName: string;
    phone: string;
    email?: string;
    inn?: string | null;
    passport?: string | null;
    verificationStatus: VerificationStatus;
}

export interface CustomerShortDto {
    id: string;
    type: CustomerType;
    displayName: string | null;
    email: string | null;
    phone: string;
    verificationStatus: VerificationStatus;
}

export interface CustomerDetailsDto {
    id: string;
    type: CustomerType;

    firstName?: string;
    lastName?: string;
    middleName?: string;

    shortName?: string;
    fullName?: string;
    legalForm?: string;

    email: string | null;
    phone: string;

    dateOfBirth?: string;

    verificationStatus: VerificationStatus;

    passport?: PassportDto;

    driverLicense?: DriverLicenseDto;


    inn?: string;
    ogrnip?: string;
    ogrn?: string;
    kpp?: string;


    organizationAddress?: string;
    factAddress?: string;


    bankInfo?: BankInfoDto;

    documents: CustomerDocumentDto[];
    bookings: BookingListItemDto[];
}

export interface CreateCustomerRequest {
    type: CustomerType;

    email?: string;
    phone: string;

    firstName?: string;
    lastName?: string;
    middleName?: string;
    shortName?: string;
    fullName?: string;

    dateOfBirth?: string;

    passport?: PassportDto;

    inn?: string;
    ogrnip?: string;
    ogrn?: string;
    kpp?: string;
    legalForm?: string;

    organizationAddress?: string;
    factAddress?: string;

    bankInfo?: BankInfoDto;

    driverLicense?: DriverLicenseDto;
}

export interface UpdateCustomerRequest{
    type: CustomerType;

    email?: string;
    phone: string;

    firstName?: string;
    lastName?: string;
    middleName?: string;
    shortName?: string;
    fullName?: string;

    dateOfBirth?: string;

    passport?: PassportDto;

    inn?: string;
    ogrnip?: string;
    ogrn?: string;
    kpp?: string;
    legalForm?: string;

    organizationAddress?: string;
    factAddress?: string;

    bankInfo?: BankInfoDto;

    driverLicense?: DriverLicenseDto;
}

export interface CustomerFormProps {
    type: CustomerType;
    customer: CustomerDetailsDto;
    readOnly: boolean;
    onUpdate?: (data: UpdateCustomerRequest) => Promise<void>;
    onCreate?: (data: CreateCustomerRequest) => Promise<void>;
}

export interface CustomerDocumentDto {
    id: string;
    type: DocumentType;
    uploadedAt: string;
    url: string;
}

export interface PassportDto{
    serial: string;
    number: string;
    issuedBy: string;
    issuedDate: string;
    registrationAddress: string;
}

export interface DriverLicenseDto{
    number: string;
    categories: string[];
    issuedDate: string;
    expirationDate: string;
    experienceFrom: number;
}

export interface BankInfoDto{
    currentAccount: string;
    correspondentAccount: string;
    bik: string;
    bankName: string;
}

