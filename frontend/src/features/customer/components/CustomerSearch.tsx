import { useState } from 'react';
import { customerApi } from '../api/customerApi';
import { CustomerType, type CustomerLookup } from '@/entities/customer/customer.type';
import { CreateCustomerModal } from './CreateCustomerModal';

interface CustomerSearchProps {
    onSelect: (customer: CustomerLookup) => void;
}

export const CustomerSearch = ({ 
    onSelect, 
}: CustomerSearchProps) => {
    const [type, setType] = useState<CustomerType>(CustomerType.Individual);
    const [query, setQuery] = useState('');
    const [results, setResults] = useState<CustomerLookup[]>([]);
    const [createModalOpen, setCreateModalOpen] = useState(false);

    const search = async () => {
        const response = await customerApi.search(type, query);
        setResults(response.data);
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
                        ФЛ
                    </option>
                    <option value={CustomerType.IndividualEntrepreneur}>
                        ИП
                    </option>
                    <option value={CustomerType.Organization}>
                        ООО
                    </option>
                </select>

                <input
                    value={query}
                    onChange={(e) => setQuery(e.target.value)}
                    placeholder="Поиск"
                    className='border rounded-lg text-gray-800 dark:text-gray-200'
                />
            </div>
            <div className='flex gap-2'>
                <button 
                    onClick={search}
                    className="mt-2 px-3 py-1 bg-blue-600 text-white rounded">  
                    Найти
                </button>
                <button
                    onClick={() => setCreateModalOpen(true)}
                    className="mt-2 px-3 py-1 bg-blue-600 text-white rounded">
                    Добавить
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
                            Паспорт: {customer.passport}
                        </div>
                    )}

                    {customer.inn && (
                        <div className="text-sm text-gray-500">
                            ИНН: {customer.inn}
                        </div>
                    )}

                    <button
                        onClick={() => {
                            console.log("Button click");
                            console.log(customer);
                            onSelect(customer);}}
                        className="mt-2 px-3 py-1 bg-blue-600 text-white rounded"
                    >
                        Выбрать
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