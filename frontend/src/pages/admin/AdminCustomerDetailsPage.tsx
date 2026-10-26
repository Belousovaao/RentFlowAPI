import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { ArrowLeft, ExternalLink, FileText, Pencil, Trash2, UserCheck, X } from 'lucide-react';
import { VerificationStatus, CustomerDetailsDto, UpdateCustomerRequest } from '@/entities/customer/customer.type';
import { adminCustomersApi } from '@/features/admin/api/adminCustomers.api';
import { CustomerForm } from '@/features/customer/components/forms/CustomerForm';
import { BookingListItemDto } from '@/entities/booking/booking.types';
import { bookingApi } from '@/entities/booking/booking.api';
import { statusLabels } from './AdminBookingsPage';
import { StatusModal } from '@/features/admin/components/StatusModal';


export const AdminCustomerDetailsPage = () => {

    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();
    const [customer, setCustomer] = useState<CustomerDetailsDto | null>(null);
    const [loading, setLoading] = useState(true);
    const [editing, setEditing] = useState(false);
    const [saving, setSaving] = useState(false);
    const [statusUpdating, setStatusUpdating] = useState(false);
    const [bookings, setBookings] = useState<BookingListItemDto[]>([]);
    const [bookingsLoading, setBookingsLoading] = useState(true);
    const [statusModalOpen, setStatusModalOpen] = useState(false);
    const [deletingDocumentId, setDeletingDocumentId] = useState<string | null>(null);

    useEffect(() => {
        if (!id)
            return;
        const loadCustomer = async () => {
            setLoading(true);
            setBookingsLoading(true);
            try {
                const [customerResponse, bookingsResponse] = await Promise.all(
                    [adminCustomersApi.getById(id), bookingApi.getAdminCustomerBookings(id)
            ]);
                setCustomer(customerResponse.data);
                setBookings(bookingsResponse);
            } catch (error) {
                console.error(error);
                alert('Ошибка загрузки клиента');
                navigate('/admin/customers');
            } finally {
                setLoading(false);
                setBookingsLoading(false);
            }
        };
        loadCustomer();
    }, [id, navigate]);

    const handleUpdate = async (data: UpdateCustomerRequest) => {
        if (!id)
            return;
        setSaving(true);
        try {
            const response = await adminCustomersApi.update(id, data);
            setCustomer(response.data);
            setEditing(false);
        } catch (error) {
            console.error(error);
            alert('Ошибка сохранения клиента');
        } finally {
            setSaving(false);
        }
    };

    const handleStatusChange = async (status: VerificationStatus) => {
        if (!id || !customer)
            return;

        setStatusUpdating(true);
        try {
            await adminCustomersApi.updateStatus(id, status);
            setCustomer({
                ...customer,
                verificationStatus: status,
            });
            setStatusModalOpen(false);
        } catch (error) {
            console.error(error);
            alert("Ошибка изменения статуса");
        } finally {
            setStatusUpdating(false);
        }
    };

    const handleDeleteDocument = async (documentId: string) => {
        if (!id || !customer || deletingDocumentId) return;

        const document = customer.documents.find(x => x.id === documentId);

        if (!document) return;

        const confirmed = window.confirm(`Удалить документ «${document.type}»? Это действие нельзя отменить.`);

        if (!confirmed) return;

        setDeletingDocumentId(documentId);

        try {
            await adminCustomersApi.deleteDocument(id, documentId);

            setCustomer(prev => {
                if (!prev) return prev;

                return {
                    ...prev,
                    documents: prev.documents.filter(
                        x => x.id !== documentId
                    ),
                };
            });
        } catch (error) {
            console.error(error);
            alert('Не удалось удалить документ');
        } finally {
            setDeletingDocumentId(null);
        }
    };

    const verificationSteps = [
        {
            status: VerificationStatus.NotSubmitted,
            label: 'Не установлен',
        },
        {
            status: VerificationStatus.Pending,
            label: 'На проверке',
        },
        {
            status: VerificationStatus.Approved,
            label: 'Одобрен',
        }
    ];

    if (loading) {
        return (
            <div className="min-h-screen bg-gray-50 dark:bg-gray-900 pt-24">
                <div className="flex justify-center py-20">
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600" />
                </div>
            </div>
        );
    }

    if (!customer) {
        return (
            <div className="min-h-screen bg-gray-50 dark:bg-gray-900 pt-24">
                <div className="container mx-auto px-4">
                    <p className="text-gray-500">
                        Клиент не найден
                    </p>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-900 pt-24 pb-16">
            <div className="container mx-auto px-4 max-w-6xl">
                {/* HEADER */}
                <div className="flex items-center justify-between mb-6">
                    <div>
                        <button
                            onClick={() => navigate('/admin/users')}
                            className="flex items-center gap-2 p-2 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg transition-colors"
                        >
                        <ArrowLeft className="w-5 h-5" />
                            Назад
                        </button>
                    </div>
                    <div className="flex items-center gap-4 ml-auto">
                        <button
                            type="button"
                            onClick={() => setStatusModalOpen(true)}
                            className="flex items-center gap-2 px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700"
                        >
                            <UserCheck className="w-4 h-4" />
                            Изменить статус
                        </button>
                        {!editing ? (

                            <button
                                type="button"
                                onClick={() => setEditing(true)}
                                className="flex items-center gap-2 px-4 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700"
                            >
                                <Pencil className="w-4 h-4" />
                                Редактировать
                            </button>

                        ) : (

                            <button
                                type="button"
                                onClick={() => setEditing(false)}
                                disabled={saving}
                                className="flex items-center gap-2 px-4 py-2 border rounded-lg hover:bg-gray-100 dark:hover:bg-gray-800"
                            >
                                <X className="w-4 h-4" />
                                Отмена
                            </button>

                        )}
                    </div>
                </div>

                {/* STATUS */}
                <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6 mb-6">
                    <div className="flex items-center justify-between mb-6">
                        <div>
                            <h2 className="font-semibold text-gray-900 dark:text-white">
                                Статус клиента
                            </h2>
                        </div>

                        {customer.verificationStatus === VerificationStatus.Rejected && (
                            <span className="px-3 py-1 rounded-full text-sm font-medium bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400">
                                Отклонен
                            </span>
                        )}
                    </div>
                    {customer.verificationStatus === VerificationStatus.Rejected ? (
                        <div className="border border-red-200 dark:border-red-900/50 bg-red-50 dark:bg-red-900/10 rounded-lg p-4">
                            <div className="flex items-center gap-3">
                                <div className="w-3 h-3 rounded-full bg-red-500" />
                                <div>
                                    <div className="font-medium text-red-800 dark:text-red-300">
                                        Верификация отклонена
                                    </div>
                                    <div className="text-sm text-red-600 dark:text-red-400 mt-1">
                                        Клиент не прошел проверку документов
                                    </div>
                                </div>
                            </div>
                        </div>
                    ) : (
                        <div className="flex items-start">
                            {verificationSteps.map((step, index) => {
                                const currentIndex = verificationSteps.findIndex(
                                    x => x.status === customer.verificationStatus
                                );
                                const isActive = step.status === customer.verificationStatus;
                                const isCompleted = index < currentIndex;
                                return (
                                    <div
                                        key={step.status}
                                        className="flex items-start flex-1"
                                    >
                                        <div className="flex flex-col items-center">
                                            <div
                                                className={`w-8 h-8 rounded-full flex items-center justify-center text-sm font-medium transition-colors
                                                    ${
                                                        isActive
                                                            ? 'bg-primary-600 text-white'
                                                            : isCompleted
                                                                ? 'bg-blue-400 text-white'
                                                                : 'bg-gray-200 text-gray-500 dark:bg-gray-700'
                                                    }
                                                `}
                                            >
                                                {isCompleted ? '✓' : index + 1}
                                            </div>

                                            <span
                                                className={`mt-2 text-sm whitespace-nowrap
                                                    ${
                                                        isActive
                                                            ? 'font-semibold text-gray-900 dark:text-white'
                                                            : 'text-gray-500'
                                                    }
                                                `}
                                            >
                                                {step.label}
                                            </span>
                                        </div>
                                        {index < verificationSteps.length - 1 && (
                                            <div
                                                className={`h-0.5 flex-1 mt-4 mx-3
                                                    ${
                                                        index < currentIndex
                                                            ? 'bg-blue-400'
                                                            : 'bg-gray-200 dark:bg-gray-700'
                                                    }
                                                `}
                                            />
                                        )}

                                    </div>
                                );
                            })}

                        </div>

                    )}

                </div>

                {/* CUSTOMER DATA */}
                <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6">
                    <CustomerForm
                        type={customer.type}
                        customer={customer}
                        readOnly={!editing}
                        onUpdate={handleUpdate}
                    />
                </div>

                {/* BOOKINGS */}
                <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6 mt-6">
                    <div className="flex items-center justify-between mb-6">
                        <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
                            Бронирования клиента
                        </h2>

                        <span className="text-sm text-gray-500 dark:text-gray-400">
                            Всего: {bookings.length}
                        </span>
                    </div>

                    {bookingsLoading ? (
                        <div className="flex justify-center py-8">
                            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600" />
                        </div>
                    ) : bookings.length === 0 ? (
                        <p className="text-gray-500 dark:text-gray-400">
                            У клиента пока нет бронирований.
                        </p>
                    ) : (
                        <div className="overflow-x-auto">
                            <div className="min-w-[700px]">
                                <div className="grid grid-cols-[1.5fr_1fr_1fr_1fr] border-b border-gray-200 dark:border-gray-700 pb-3 px-3 text-sm font-medium text-gray-500">
                                    <span>Автомобиль</span>
                                    <span>Период</span>
                                    <span>Сумма</span>
                                    <span>Статус</span>
                                </div>

                                <div className="divide-y divide-gray-200 dark:divide-gray-700">
                                    {bookings.map((booking) => (
                                        <div
                                            key={booking.id}
                                            onClick={() =>
                                                navigate(`/admin/bookings/${booking.id}`)
                                            }
                                            className="grid grid-cols-[1.5fr_1fr_1fr_1fr] items-center px-3 py-4 cursor-pointer hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors"
                                        >
                                            <div>
                                                <p className="font-semibold text-gray-900 dark:text-white">
                                                    {booking.asset.brandName}{' '}
                                                    {booking.asset.model}
                                                </p>

                                                <p className="text-sm text-gray-500">
                                                    {booking.asset.year}
                                                </p>
                                            </div>

                                            <div className="text-sm text-gray-600 dark:text-gray-300">
                                                {new Date(
                                                    booking.rentalPeriod.startDate
                                                ).toLocaleDateString('ru-RU')}
                                                {' — '}
                                                {new Date(
                                                    booking.rentalPeriod.endDate
                                                ).toLocaleDateString('ru-RU')}
                                            </div>

                                            <div className="font-semibold text-gray-900 dark:text-white">
                                                {booking.price.totalToPay.toLocaleString(
                                                    'ru-RU'
                                                )}{' '}
                                                ₽
                                            </div>

                                            <div>
                                                <span
                                                    className={`inline-flex items-center rounded-full px-3 py-1 text-xs font-medium ${
                                                        booking.status === 'Pending'
                                                            ? 'bg-amber-100 text-amber-800 dark:bg-amber-900/30 dark:text-amber-400'
                                                            : booking.status === 'Confirmed'
                                                                ? 'bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400'
                                                                : booking.status === 'Active'
                                                                    ? 'bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400'
                                                                    : booking.status === 'Completed'
                                                                        ? 'bg-gray-100 text-gray-700 dark:bg-gray-700 dark:text-gray-300'
                                                                        : 'bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400'
                                                    }`}
                                                >
                                                    {statusLabels[booking.status]}
                                                </span>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        </div>
                    )}
                </div>

                {/* CUSTOMER DOCUMENTS */}
                <div className="bg-white dark:bg-gray-800 rounded-xl shadow-sm p-6 mt-6">
                    <div className="flex items-center justify-between mb-6">
                        <div>
                            <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
                                Документы клиента
                            </h2>
                            <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
                                Документы, отправленные клиентом для верификации
                            </p>
                        </div>

                        <span className="text-sm text-gray-500 dark:text-gray-400">
                            Всего: {customer.documents?.length ?? 0}
                        </span>
                    </div>

                    {!customer.documents?.length ? (
                        <div className="flex flex-col items-center justify-center py-10 text-center">
                            <FileText className="w-10 h-10 text-gray-400 mb-3" />

                            <p className="font-medium text-gray-700 dark:text-gray-300">
                                Документы не загружены
                            </p>

                            <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
                                Клиент ещё не отправлял документы для проверки.
                            </p>
                        </div>
                    ) : (
                        <div className="divide-y divide-gray-200 dark:divide-gray-700">
                            {customer.documents.map((document) => (
                                <div
                                    key={document.id}
                                    className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 py-4 first:pt-0 last:pb-0"
                                >
                                    <div className="flex items-start gap-3 min-w-0">
                                        <div className="p-2 rounded-lg bg-primary-50 dark:bg-primary-900/20">
                                            <FileText className="w-5 h-5 text-primary-600 dark:text-primary-400" />
                                        </div>

                                        <div className="min-w-0">
                                            <p className="font-medium text-gray-900 dark:text-white">
                                                {document.type}
                                            </p>

                                            <p className="text-sm text-gray-500 dark:text-gray-400 mt-1">
                                                Загружен{' '}
                                                {new Date(document.uploadedAt).toLocaleString(
                                                    'ru-RU',
                                                    {
                                                        day: '2-digit',
                                                        month: '2-digit',
                                                        year: 'numeric',
                                                        hour: '2-digit',
                                                        minute: '2-digit',
                                                    }
                                                )}
                                            </p>
                                        </div>
                                    </div>

                                    <a
                                        href={document.url}
                                        target="_blank"
                                        rel="noopener noreferrer"
                                        className="inline-flex items-center justify-center gap-2 px-4 py-2 border border-gray-200 dark:border-gray-700 rounded-lg text-sm font-medium text-gray-700 dark:text-gray-200 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors shrink-0"
                                    >
                                        <ExternalLink className="w-4 h-4" />
                                        Открыть документ
                                    </a>

                                    <button
                                        type="button"
                                        onClick={() => handleDeleteDocument(document.id)}
                                        disabled={deletingDocumentId !== null}
                                        className="inline-flex items-center justify-center gap-2 px-4 py-2 border border-red-200 dark:border-red-900 rounded-lg text-sm font-medium text-red-600 dark:text-red-400 hover:bg-red-50 dark:hover:bg-red-900/20 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                                    >
                                        <Trash2 className="w-4 h-4" />
                                        {deletingDocumentId === document.id
                                            ? 'Удаление…'
                                            : 'Удалить'}
                                    </button>
                                </div>
                            ))}
                        </div>
                    )}
                </div>

            </div>
            <StatusModal
                open={statusModalOpen}
                currentStatus={customer.verificationStatus}
                onClose={() => setStatusModalOpen(false)}
                onChange={handleStatusChange}
            />
        </div>
    );
};