import { useAuthStore } from '@/features/auth/stores/authStore';
import { useI18n } from '@/app/providers/I18nProvider';
import { useEffect, useState } from 'react';
import { customerApi } from '@/features/customer/api/customerApi';
import { CustomerDetailsDto, UpdateCustomerRequest } from '@/entities/customer/customer.type';
import { CustomerForm } from '@/features/customer/components/forms/CustomerForm';
import { CustomerProfileCard } from '@/features/account/components/СustomerProfileCard';
import { BookingListItemDto } from '@/entities/booking/booking.types';
import { bookingApi } from '@/entities/booking/booking.api';
import { UpcomingBookingCard } from '@/features/account/components/UpcomingBookingCard';
import { MyBookingsSection } from '@/features/account/components/MyBookingsSection';
import { BookingHistory } from '@/features/account/components/BookingHistory';
import { CustomerStats } from '@/features/account/components/CustomerStats';
import axios from 'axios';

export const AccountPage = () => {
    const { isAuthenticated } = useAuthStore();
    const { t } = useI18n();
    const [customer, setCustomer] = useState<CustomerDetailsDto | null>(null) 
    const [editing, setEditing] = useState(false);
    const [saving, setSaving] = useState(false);
    const [bookings, setBookings] = useState<BookingListItemDto[]>([]);

    useEffect(() => {
        const loadData = async () => {
            try {
                const [customerResponse, bookingsData] = await Promise.all([
                    customerApi.getMe(),
                    bookingApi.getMyBookings(),
                ]);

                setCustomer(customerResponse.data);
                setBookings(bookingsData);
            } catch (error) {
                console.error('Ошибка загрузки личного кабинета:', error);
                if (axios.isAxiosError(error))
                {
                    console.error('URL:', error.config?.url);
                    console.error('HTTP status:', error.response?.status);
                    console.error('Response:', error.response?.data);
                }
            }
        };

        loadData();
    }, []);

    const handleUpdate = async(data: UpdateCustomerRequest)=>{
        if (!customer)
            return;
        const result = await customerApi.update(data);

        setCustomer(result.data);
        setEditing(false);
        setSaving(false);
    };

    if (!isAuthenticated || !customer) {
        return (
            <div className="container mx-auto px-4 pt-28 pb-16">
                <div className="mx-auto max-w-lg rounded-2xl border border-gray-200 bg-white p-8 text-center dark:border-gray-700 dark:bg-gray-800">
                    <p className="text-gray-600 dark:text-gray-300">{t('auth.notSignedIn')}</p>
                    <p className="mt-2 text-sm text-gray-500">{t('auth.useSignIn')}</p>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50 pt-24 pb-16 dark:bg-gray-900">
            <div className="container mx-auto px-4">
                <div className="mb-8 flex items-center justify-between gap-4">
                    <h1 className="text-3xl font-bold text-gray-900 dark:text-white">
                        {t('account.title')}
                    </h1>
                </div>

                {editing ? (
                    <div className="rounded-2xl border border-gray-200 bg-white p-6 shadow-sm dark:border-gray-700 dark:bg-gray-800">
                        <CustomerForm
                            type={customer.type}
                            customer={customer}
                            readOnly={false}
                            onUpdate={handleUpdate}
                        />
                    </div>
                ) : (
                    <div className="space-y-10">
                        <div className="grid gap-6 lg:grid-cols-[320px_minmax(0,1fr)]">
                            <CustomerProfileCard
                                customer={customer}
                                onEdit={() => setEditing(true)}
                            />

                            <UpcomingBookingCard
                                bookings={bookings}
                            />
                        </div>

                        <MyBookingsSection
                            bookings={bookings}
                            limit={3}
                        />

                        <BookingHistory bookings={bookings} />

                        <CustomerStats bookings={bookings} />
                    </div>
                )}
            </div>
        </div>
    );
}