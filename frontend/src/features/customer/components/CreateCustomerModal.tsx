import { useState } from 'react';
import { X, UserRound, Briefcase, Building2, UserPlus } from 'lucide-react';
import { CustomerType, type CreateCustomerRequest, type CustomerLookup } from '@/entities/customer/customer.type';
import { customerApi } from '../api/customerApi';
import { IndividualForm } from './forms/IndividualForm';
import { EntrepreneurForm } from './forms/EntrepreneurForm';
import { OrganizationForm } from './forms/OrganizationForm';
import { createPortal } from 'react-dom';
import { adminCustomersApi } from '@/features/admin/api/adminCustomers.api';
import axios from '@/shared/api/axios';

interface CreateCustomerModalProps {
    open: boolean;
    onClose: () => void;
    onCreated: (customer: CustomerLookup) => void;
}

const customerTypes = [
    {
        type: CustomerType.Individual,
        title: 'Физическое лицо',
        description: 'Частный клиент',
        icon: UserRound,
    },
    {
        type: CustomerType.IndividualEntrepreneur,
        title: 'ИП',
        description: 'Индивидуальный предприниматель',
        icon: Briefcase,
    },
    {
        type: CustomerType.Organization,
        title: 'Организация',
        description: 'Юридическое лицо',
        icon: Building2,
    },
];

export const CreateCustomerModal = ({
    open,
    onClose,
    onCreated,
}: CreateCustomerModalProps) => {

    const [type, setType] = useState<CustomerType>(CustomerType.Individual);

    if (!open) return null;

    const handleSubmit = async (data: CreateCustomerRequest) => {
        try {
            const response = await adminCustomersApi.create(data);
            onCreated(response.data);
        } catch (error: any) {
            console.error('Статус:', error?.response?.status);
            console.error('Ответ сервера:', error?.response?.data);
            console.error('Отправленные данные:', data);
            throw error;
        }
    };

    const modalRoot = document.getElementById('modal-root');

    if (!modalRoot) return null;

    const selectedType = customerTypes.find(x => x.type === type);

    return createPortal(
        <div className="fixed inset-0 z-[100] flex items-center justify-center bg-black/50 dark:bg-black/70 backdrop-blur-md p-4">
            <div className="relative w-full max-w-5xl max-h-[92vh] flex flex-col overflow-hidden rounded-3xl border border-white/40 dark:border-gray-700/60 bg-white/95 dark:bg-gray-900/95 shadow-2xl">

                {/* HEADER */}
                <div className="
                    flex items-center justify-between px-7 py-5 border-b border-gray-200 dark:border-gray-700">
                    <div className="flex items-center gap-4">

                        <div className="flex items-center justify-center w-11 h-11 rounded-xl bg-primary-100 dark:bg-primary-900/30 text-primary-600">
                            <UserPlus className="w-5 h-5" />
                        </div>

                        <div>
                            <h2 className="text-xl font-bold text-gray-900 dark:text-white">
                                Новый клиент
                            </h2>
                        </div>

                    </div>

                    <button
                        onClick={onClose}
                        className="flex items-center justify-center w-9 h-9 rounded-xl text-gray-500 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors"
                    >
                        <X className="w-5 h-5" />
                    </button>
                </div>

                {/* BODY */}
                <div className="overflow-y-auto px-7 py-6
                ">

                    {/* TYPE */}
                    <div className="mb-7">

                        <div className="mb-4">
                            <h3 className="font-semibold text-gray-900 dark:text-white">
                                Тип клиента
                            </h3>
                        </div>

                        <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
                            {customerTypes.map(item => {

                                const Icon = item.icon;
                                const selected = type === item.type;

                                return (
                                    <button
                                        key={item.type}
                                        type="button"
                                        onClick={() => setType(item.type)}
                                        className={`relative text-left rounded-2xl border p-4 transition-all duration-200
                                            ${selected
                                                    ? `border-primary-500 bg-primary-50 dark:bg-primary-900/20 shadow-md`
                                                    : `border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800/60 hover:border-primary-300 dark:hover:border-primary-700 hover:shadow-sm`
                                            }
                                        `}
                                    >
                                        {selected && (
                                            <div className="absolute top-3 right-3 w-2.5 h-2.5 rounded-full bg-primary-600"/>
                                        )}
                                        <div className={`flex items-center justify-center w-10 h-10 rounded-xl mb-3
                                            ${
                                                selected
                                                    ? `bg-primary-100 dark:bg-primary-900/40 text-primary-600`
                                                    : `bg-gray-100dark:bg-gray-700 text-gray-500 dark:text-gray-300`
                                            }
                                        `}>
                                            <Icon className="w-5 h-5" />
                                        </div>
                                        <div className="font-semibold text-gray-900 dark:text-white">
                                            {item.title}
                                        </div>
                                        <div className="text-xs text-gray-500 dark:text-gray-400 mt-1">
                                            {item.description}
                                        </div>
                                    </button>
                                );
                            })}
                        </div>
                    </div>

                    {/* FORM */}
                    <div className="border-t border-gray-200 dark:border-gray-700 pt-6">

                        {type === CustomerType.Individual && (
                            <IndividualForm
                                onCreate={handleSubmit}
                            />
                        )}
                        {type === CustomerType.IndividualEntrepreneur && (
                            <EntrepreneurForm
                                onCreate={handleSubmit}
                            />
                        )}
                        {type === CustomerType.Organization && (
                            <OrganizationForm
                                onCreate={handleSubmit}
                            />
                        )}
                    </div>
                </div>
            </div>
        </div>,
        modalRoot
    );
};