import { useEffect, useState } from 'react';
import type { InputHTMLAttributes } from 'react';

interface DriverLicenseCategoriesInputProps
    extends Omit<
        InputHTMLAttributes<HTMLInputElement>,
        'value' | 'onChange'
    > {
    value: string;
    invalid?: boolean;
    onChange: (value: string) => void;
}

export const DriverLicenseCategoriesInput = ({
    value,
    onChange,
    className = '',
    invalid = false,
    ...props
}: DriverLicenseCategoriesInputProps) => {
    const [invalidInput, setInvalidInput] = useState(false);

    useEffect(() => {
        if (!invalidInput) {
            return;
        }

        const timeout = window.setTimeout(() => {
            setInvalidInput(false);
        }, 1500);

        return () => window.clearTimeout(timeout);
    }, [invalidInput]);

    const handleChange = ( event: React.ChangeEvent<HTMLInputElement>) => {
        const inputValue = event.target.value;

        if (/[^a-zA-Z0-9,\s]/.test(inputValue)) {
            setInvalidInput(true);
        }

        const normalized = inputValue
            .replace(/[^a-zA-Z0-9,\s]/g, '')
            .toUpperCase();

        onChange(normalized);
    };

    return (
        <div>
            <input
                {...props}
                value={value}
                className={className}
                inputMode="text"
                autoComplete="off"
                onChange={handleChange}
            />

            <p
                className={`mt-1 text-xs transition-colors ${
                    invalidInput || invalid
                        ? 'text-red-500'
                        : 'text-gray-500'
                }`}
            >
                Латинские буквы, например: B1, C, D
            </p>
        </div>
    );
};