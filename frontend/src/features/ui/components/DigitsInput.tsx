import type { InputHTMLAttributes } from 'react';

interface DigitsInputProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'value' | 'onChange'> {
    value: string;
    onChange: (value: string) => void;
    maxLength: number;
    invalid?: boolean;
}

export const DigitsInput = ({
    value,
    onChange,
    maxLength,
    invalid = false,
    className = '',
    ...props
}: DigitsInputProps) => {
    const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const digits = event.target.value
            .replace(/\D/g, '')
            .slice(0, maxLength);

        onChange(digits);
    };

    return (
        <input
            {...props}
            value={value}
            maxLength={maxLength}
            inputMode="numeric"
            autoComplete="off"
            className={`${className} ${
                invalid
                    ? 'border-red-500 focus:border-red-500 focus:ring-red-500'
                    : ''
            }`}
            onChange={handleChange}
        />
    );
};