import { ReactNode } from 'react';

interface CustomerFormFieldProps {
    label?: string;
    children: ReactNode;
    className?: string;
}

export const CustomerFormField = ({
    label,
    children,
    className = '',
}: CustomerFormFieldProps) => {
    return (
        <div className={className}>
            {label && (
                <label className="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-1.5">
                    {label}
                </label>
            )}

            {children}
        </div>
    );
};