import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowLeft, } from 'lucide-react';
import { adminCustomersApi } from '@/features/admin/api/adminCustomers.api';
import { CustomerShortDto, CustomerType, VerificationStatus } from '@/entities/customer/customer.type';
import { CreateCustomerModal } from '@/features/customer/components/CreateCustomerModal';

const statusLabels: Record<VerificationStatus, string> = {
    NotSubmitted: "Не установлен",
    Pending: "На проверке",
    Approved: "Одобрен",
    Rejected: "Отказ"
};

const statusColors: Record<VerificationStatus, string> = {
    Pending: "bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400",
    Approved: "bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400",
    NotSubmitted: "bg-gray-100 text-gray-700 dark:bg-gray-700 dark:text-gray-300",
    Rejected: "bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400",
};

export const AdminCustomersPage = () => {

    const [customers, setCustomers] = useState<CustomerShortDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [showRejected, setShowRejected] = useState(false);
    const [page, setPage] = useState(1);
    const [total, setTotal] = useState(0);
    const [searchTerm, setSearchTerm] = useState('');
    const [customerType, setCustomerType] = useState<CustomerType | undefined>(undefined);
    const [createModalOpen, setCreateModalOpen] = useState(false);

    const navigate = useNavigate();
    const pageSize = 20;

    const fetchCustomers = async () => {
        setLoading(true);

        try {
            const res = await adminCustomersApi.getList({
                page,
                pageSize,
                search: searchTerm || undefined,
                type: customerType,
                showRejected
            });
            setCustomers(res.data.data);
            setTotal(res.data.totalCount);

        } catch (err) {
            console.error(err);
            alert('Ошибка загрузки клиентов');

        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchCustomers();
    }, [page, searchTerm, customerType, showRejected]);

    const totalPages = Math.ceil(total / pageSize);
    const getCustomerTypeLabel = (type: CustomerType) => {
        switch (type) {
            case CustomerType.Individual:
                return 'ФЛ';

            case CustomerType.IndividualEntrepreneur:
                return 'ИП';

            case CustomerType.Organization:
                return 'ЮЛ';

            default:
                return '';
        }
    };

    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-900 pt-24 pb-16">
            <div className="container mx-auto px-4">
                {/* Заголовок */}
                <div className="flex justify-between items-center mb-6">
                    <div className="flex items-center gap-4">
                        <button
                            onClick={() => navigate('/admin/')}
                            className="p-2 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-lg transition-colors"
                        >
                            <ArrowLeft className="w-5 h-5" />
                        </button>
                        <h1 className="text-2xl font-bold text-gray-900 dark:text-white">
                            Управление клиентами
                        </h1>
                    </div>
                    <button
                        onClick={() => setCreateModalOpen(true)}
                        className="mt-2 px-3 py-1 bg-blue-600 text-white rounded-lg">
                        Добавить клиента
                    </button>
                </div>

                {/* Поиск и фильтры */}
                <div className="bg-white dark:bg-gray-800 rounded-lg shadow-sm p-4 mb-6">
                    <div className="flex flex-col lg:flex-row gap-4">
                        {/* Поиск */}
                        <div className="flex-1">
                            <input
                                type="text"
                                placeholder="Поиск по имени, телефону или email..."
                                value={searchTerm}
                                onChange={(e) => {
                                    setSearchTerm(e.target.value);
                                    setPage(1);
                                }}
                                className="w-full px-4 py-2 border border-gray-300 dark:border-gray-600 rounded-lg bg-white dark:bg-gray-700 text-gray-900 dark:text-white focus:ring-2 focus:ring-primary-500 ocus:border-transparent"
                            />
                        </div>
                        <label className="inline-flex items-center">
                            <input
                                type="checkbox"
                                checked={showRejected}
                                onChange={(e) => {
                                    setShowRejected(e.target.checked);
                                    setPage(1);
                                }}
                                className="rounded border-gray-300 text-primary-600 focus:ring-primary-500"
                            />

                            <span className="ml-2 text-sm text-gray-700 dark:text-gray-300">
                                Показывать отклонённых
                            </span>
                        </label>

                        {/* Тип клиента */}
                        <div className="flex gap-2">
                            <button
                                type="button"
                                onClick={() => {
                                    setCustomerType(undefined);
                                    setPage(1);
                                }}
                                className={`px-4 py-2 rounded-lg border
                                    ${
                                        customerType === undefined
                                            ? 'bg-primary-600 text-white border-primary-600'
                                            : 'bg-white text-gray-700 border-gray-300 hover:bg-gray-100'
                                    }
                                `}
                            >
                                Все
                            </button>
                            <button
                                type="button"
                                onClick={() => {
                                    setCustomerType(CustomerType.Individual);
                                    setPage(1);
                                }}
                                className={`
                                    px-4 py-2 rounded-lg border
                                    ${
                                        customerType === CustomerType.Individual
                                            ? 'bg-primary-600 text-white border-primary-600'
                                            : 'bg-white text-gray-700 border-gray-300 hover:bg-gray-100'
                                    }
                                `}
                            >
                                ФЛ
                            </button>
                            <button
                                type="button"
                                onClick={() => {
                                    setCustomerType(CustomerType.IndividualEntrepreneur);
                                    setPage(1);
                                }}
                                className={`
                                    px-4 py-2 rounded-lg border
                                    ${
                                        customerType === CustomerType.IndividualEntrepreneur
                                            ? 'bg-primary-600 text-white border-primary-600'
                                            : 'bg-white text-gray-700 border-gray-300 hover:bg-gray-100'
                                    }
                                `}
                            >
                                ИП
                            </button>
                            <button
                                type="button"
                                onClick={() => {
                                    setCustomerType(CustomerType.Organization);
                                    setPage(1);
                                }}
                                className={`
                                    px-4 py-2 rounded-lg border
                                    ${
                                        customerType === CustomerType.Organization
                                            ? 'bg-primary-600 text-white border-primary-600'
                                            : 'bg-white text-gray-700 border-gray-300 hover:bg-gray-100'
                                    }
                                `}
                            >
                                ЮЛ
                            </button>
                        </div>
                    </div>
                </div>

                {/* Таблица */}
                {loading ? (
                    <div className="flex justify-center py-12">
                        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600" />
                    </div>
                ) : !customers || customers.length === 0 ? (
                    <div className="text-center py-12 bg-white dark:bg-gray-800 rounded-xl">
                        <p className="text-gray-700 dark:text-gray-300">
                            Клиенты не найдены
                        </p>
                    </div>
                ) : (
                    <div className="overflow-x-auto rounded-2xl border border-gray-200 bg-white dark:border-gray-700 dark:bg-gray-800">
                        <div className="grid min-w-max grid-cols-[auto_auto_auto_auto_auto]">
                            {/* Header */}
                            <div className="border-b border-gray-200 bg-gray-50 px-5 py-3 text-sm font-medium text-gray-700 dark:text-gray-300 dark:border-gray-700 dark:bg-gray-800/50">
                                Клиент
                            </div>
                            <div className="border-b border-gray-200 bg-gray-50 px-5 py-3 text-sm font-medium text-gray-700 dark:text-gray-300 dark:border-gray-700 dark:bg-gray-800/50">
                                Тип
                            </div>
                            <div className="border-b border-gray-200 bg-gray-50 px-5 py-3 text-sm font-medium text-gray-700 dark:text-gray-300 dark:border-gray-700 dark:bg-gray-800/50">
                                Телефон
                            </div>
                            <div className="border-b border-gray-200 bg-gray-50 px-5 py-3 text-sm font-medium text-gray-700 dark:text-gray-300 dark:border-gray-700 dark:bg-gray-800/50">
                                Email
                            </div>
                            <div className="border-b border-gray-200 bg-gray-50 px-5 py-3 text-sm font-medium text-gray-700 dark:text-gray-300 dark:border-gray-700 dark:bg-gray-800/50">
                                Статус
                            </div>

                            {/* Rows */}
                            {customers.map((customer) => (
                                <div
                                    key={customer.id}
                                    onClick={() => navigate(`/admin/customers/${customer.id}`)}
                                    className="grid grid-cols-subgrid col-span-5 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors cursor-pointer"
                                >
                                    <div className="px-5 py-5 text-gray-700 dark:text-gray-300">
                                        {customer.displayName}
                                    </div>
                                    <div className="px-5 py-5 text-gray-700 dark:text-gray-300">
                                        {getCustomerTypeLabel(customer.type)}
                                    </div>
                                    <div className="px-5 py-5 text-sm whitespace-nowrap text-gray-700 dark:text-gray-300">
                                        {customer.phone || '—'}
                                    </div>
                                    <div className="px-5 py-5 text-sm whitespace-nowrap text-gray-700 dark:text-gray-300">
                                        {customer.email || '—'}
                                    </div>
                                    <div className="px-5 py-5">
                                        <span
                                            className={`inline-flex items-center rounded-full px-3 py-1 text-xs font-medium whitespace-nowrap ${
                                                statusColors[customer.verificationStatus]
                                            }`}
                                        >
                                            {statusLabels[customer.verificationStatus]}
                                        </span>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                )}
                {/* Пагинация */}
                {totalPages > 1 && (
                    <div className="flex justify-center gap-2 mt-8">
                        <button
                            onClick={() =>
                                setPage(p => Math.max(1, p - 1))
                            }
                            disabled={page === 1}
                            className="px-4 py-2 border rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-100 dark:hover:bg-gray-700 text-gray-700 dark:text-gray-300"
                        >
                            ← Назад
                        </button>
                        <span className="px-4 py-2 text-gray-600 dark:text-gray-400">
                            Страница {page} из {totalPages}
                        </span>
                        <button
                            onClick={() =>
                                setPage(p => Math.min(totalPages, p + 1))
                            }
                            disabled={page === totalPages}
                            className="px-4 py-2 border rounded-lg disabled:opacity-50 disabled:cursor-not-allowed hover:bg-gray-100 dark:hover:bg-gray-700 text-gray-700 dark:text-gray-300"
                        >
                            Вперед →
                        </button>
                    </div>
                )}
            </div>
        <CreateCustomerModal
            open={createModalOpen}
            onClose={() => setCreateModalOpen(false)}
            onCreated={() => {setCreateModalOpen(false);}}
        />
        </div>
    );
};