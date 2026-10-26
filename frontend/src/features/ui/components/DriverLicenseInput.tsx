import type { InputHTMLAttributes } from 'react';

interface DriverLicenseInputProps extends Omit<InputHTMLAttributes<HTMLInputElement>, 'value' | 'onChange'> {
    value: string;
    onChange: (value: string) => void;
    invalid?: boolean;
}

const DRIVER_LICENSE_LENGTH = 10;

function formatDriverLicense(value: string): string {
    const digits = value
        .replace(/\D/g, '')
        .slice(0, DRIVER_LICENSE_LENGTH);

    if (digits.length <= 2) {
        return digits;
    }

    if (digits.length <= 4) {
        return `${digits.slice(0, 2)} ${digits.slice(2)}`;
    }

    return `${digits.slice(0, 2)} ${digits.slice(2, 4)} ${digits.slice(4)}`;
}

export const DriverLicenseInput = ({
    value,
    onChange,
    invalid = false,
    className = '',
    ...props
}: DriverLicenseInputProps) => {
    const handleChange = (
        event: React.ChangeEvent<HTMLInputElement>
    ) => {
        const digits = event.target.value
            .replace(/\D/g, '')
            .slice(0, DRIVER_LICENSE_LENGTH);

        onChange(digits);
    };

    return (
        <input
            {...props}
            value={formatDriverLicense(value)}
            inputMode="numeric"
            autoComplete="off"
            maxLength={13}
            className={`${className} ${
                invalid
                    ? 'border-red-500 focus:border-red-500 focus:ring-red-500'
                    : ''
            }`}
            onChange={handleChange}
        />
    );
};