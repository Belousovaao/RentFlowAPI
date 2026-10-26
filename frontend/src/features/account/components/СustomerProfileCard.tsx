import { Mail, Pencil, Phone, User } from 'lucide-react';
import { CustomerDetailsDto, CustomerType } from '@/entities/customer/customer.type';

interface Props {
    customer: CustomerDetailsDto;
    onEdit: () => void;
}

export const CustomerProfileCard = ({ customer, onEdit }: Props) => {
    const displayName = getDisplayName(customer);
    const customerType = getCustomerTypeLabel(customer.type);

    return (
        <section className="rounded-2xl border border-gray-200 bg-white p-6 shadow-sm dark:border-gray-700 dark:bg-gray-800">
            <div className="flex items-start justify-between gap-4">
                <div className="flex items-center gap-4">
                    <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-full bg-primary-100 text-primary-600 dark:bg-primary-900/30 dark:text-primary-400">
                        <User className="h-6 w-6" />
                    </div>

                    <div className="min-w-0">
                        <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
                            {displayName}
                        </h2>
                    </div>
                </div>

                <button
                    type="button"
                    onClick={onEdit}
                    aria-label="Редактировать профиль"
                    className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg text-gray-500 transition-colors hover:bg-gray-100 hover:text-gray-900 dark:text-gray-400 dark:hover:bg-gray-700 dark:hover:text-white"
                >
                    <Pencil className="h-4 w-4" />
                </button>
            </div>

            <div className="mt-6 space-y-3 border-t border-gray-100 pt-5 dark:border-gray-700">
                {customer.phone && (
                    <div className="flex items-center gap-3 text-sm text-gray-600 dark:text-gray-300">
                        <Phone className="h-4 w-4 shrink-0 text-gray-400" />
                        <span>{customer.phone}</span>
                    </div>
                )}

                {customer.email && (
                    <div className="flex items-center gap-3 text-sm text-gray-600 dark:text-gray-300">
                        <Mail className="h-4 w-4 shrink-0 text-gray-400" />
                        <span className="truncate">{customer.email}</span>
                    </div>
                )}
            </div>

            <button
                type="button"
                onClick={onEdit}
                className="mt-5 w-full text-left text-sm font-medium text-primary-600 transition-colors hover:text-primary-700 dark:text-primary-400 dark:hover:text-primary-300"
            >
                Изменить данные →
            </button>
        </section>
    );
};

const getDisplayName = (customer: CustomerDetailsDto): string => {
    switch (customer.type) {
        case CustomerType.Individual:
            return [
                customer.firstName,
                customer.middleName,
                customer.lastName,
            ]
                .filter(Boolean)
                .join(' ');

        case CustomerType.IndividualEntrepreneur:
            return [
                customer.firstName,
                customer.middleName,
                customer.lastName,
            ]
                .filter(Boolean)
                .join(' ');

        case CustomerType.Organization:
            return customer.shortName || customer.fullName || 'Организация';

        default:
            return 'Клиент';
    }
};

const getCustomerTypeLabel = (type: CustomerType): string => {
    switch (type) {
        case CustomerType.Individual:
            return 'Физическое лицо';

        case CustomerType.IndividualEntrepreneur:
            return 'Индивидуальный предприниматель';

        case CustomerType.Organization:
            return 'Организация';

        default:
            return 'Клиент';
    }
};