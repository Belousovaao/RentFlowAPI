export type CreateBookingCommand = {
    assetId: string;
    customerId: string;
    startDate: string;
    endDate: string;
    driver?: {
        firstName: string;
        lastName: string;
        middleName?: string;
        lisenceNumber: string;
        categories: string[];
        issueDate: string;
        expirationDate: string;
        phone: string;
    };
};