import { useState } from 'react';
import { customerApi } from '../api/customerApi';
import { CustomerType, type CustomerLookup } from '@/entities/customer/customer.type';
import { CreateCustomerModal } from './CreateCustomerModal';
import { useI18n } from '@/app/providers/I18nProvider';

interface CustomerSearchProps {
    onSelect: (customer: CustomerLookup) => void;
}

export const CustomerSearch = ({ 
    onSelect, 
}: CustomerSearchProps) => {
    const { t } = useI18n();
    const [type, setType] = useState<CustomerType>(CustomerType.Individual);
    const [query, setQuery] = useState('');
    const [results, setResults] = useState<CustomerLookup[]>([]);
    const [createModalOpen, setCreateModalOpen] = useState(false);

    const search = async () => {
        try {
            const customers = await customerApi.search(type, query);
            setResults(customers);
        } catch (error) {
            console.error('Не удалось выполнить поиск клиентов:', error);
        }
    };

    return (
        <div className="space-y-3"> 
            <div className="flex gap-2">
                <select
                    value={type}
                    onChange={(e) => setType(e.target.value as CustomerType)}
                    className='border rounded-lg text-gray-800 dark:text-gray-200'
                >
                    <option value={CustomerType.Individual}>
                        {t("customer.type.individualShort")}
                    </option>
                    <option value={CustomerType.IndividualEntrepreneur}>
                        {t("customer.type.entrepreneurShort")}
                    </option>
                    <option value={CustomerType.Organization}>
                        {t("customer.type.organizationShort")}
                    </option>
                </select>

                <input
                    value={query}
                    onChange={(e) => setQuery(e.target.value)}
                    placeholder={t("customer.search.placeholder")}
                    className='border rounded-lg text-gray-800 dark:text-gray-200'
                />
            </div>
            <div className='flex gap-2'>
                <button 
                    onClick={search}
                    className="mt-2 px-3 py-1 bg-blue-600 text-white rounded">  
                    {t("customer.search.find")}
                </button>
                <button
                    onClick={() => setCreateModalOpen(true)}
                    className="mt-2 px-3 py-1 bg-blue-600 text-white rounded">
                    {t("customer.search.add")}
                </button>
            </div>
            <div>
                {results.map(customer => (
                <div
                    key={customer.id}
                    className="border rounded-lg p-2"
                >
                    <div className="font-medium text-gray-800 dark:text-gray-200">
                        {customer.displayName}
                    </div>

                    <div className="text-sm text-gray-700 dark:text-gray-300">
                        {customer.phone}
                    </div>

                    {customer.passport && (
                        <div className="text-sm text-gray-700 dark:text-gray-300">
                            {t("customer.search.passport")}: {customer.passport}
                        </div>
                    )}

                    {customer.inn && (
                        <div className="text-sm text-gray-500">
                            {t("customer.search.inn")}: {customer.inn}
                        </div>
                    )}

                    <button
                        onClick={() => {
                            console.log("Button click");
                            console.log(customer);
                            onSelect(customer);}}
                        className="mt-2 px-3 py-1 bg-blue-600 text-white rounded"
                    >
                        {t("common.select")}
                    </button>
                </div>
                ))}
            </div>

        <CreateCustomerModal
            open={createModalOpen}
            onClose={() => setCreateModalOpen(false)}
            onCreated={(customer) => {
                onSelect(customer);
                setCreateModalOpen(false);
            }}
        />
        </div>
    );
};