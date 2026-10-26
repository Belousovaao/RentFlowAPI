import { type ChangeEvent, type InputHTMLAttributes, type KeyboardEvent, useRef, } from 'react';

interface PhoneInputProps extends Omit< InputHTMLAttributes<HTMLInputElement>, 'value' | 'onChange'> 
{
    value: string;
    onChange: (value: string) => void;
}

const PHONE_DIGITS_COUNT = 10;
const PHONE_PREFIX_LENGTH = 4; // "+7 ("

function extractDigits(value: string): string {
    let digits = value.replace(/\D/g, '');

    if (digits.startsWith('7') || digits.startsWith('8')) {
        digits = digits.slice(1);
    }

    return digits.slice(0, PHONE_DIGITS_COUNT);
}

function formatPhone(digits: string): string {
    let result = '+7 (';

    result += digits.slice(0, 3);

    if (digits.length >= 3) {
        result += ')';
    }

    if (digits.length > 3) {
        result += ` ${digits.slice(3, 6)}`;
    }

    if (digits.length > 6) {
        result += `-${digits.slice(6, 8)}`;
    }

    if (digits.length > 8) {
        result += `-${digits.slice(8, 10)}`;
    }

    return result;
}

function toCanonicalPhone(digits: string): string {
    return digits ? `+7${digits}` : '';
}

/**
 * Сколько пользовательских цифр находится слева от позиции курсора.
 *
 * Фиксированная "+7 (" не считается.
 */
function getDigitIndex(
    value: string,
    position: number
): number {
    if (position <= PHONE_PREFIX_LENGTH) {
        return 0;
    }

    return value
        .slice(PHONE_PREFIX_LENGTH, position)
        .replace(/\D/g, '')
        .length;
}

/**
 * Позиция курсора после указанного количества
 * пользовательских цифр.
 */
function getPositionForDigitIndex(
    value: string,
    digitIndex: number
): number {
    if (digitIndex <= 0) {
        return PHONE_PREFIX_LENGTH;
    }

    let count = 0;

    for (
        let i = PHONE_PREFIX_LENGTH;
        i < value.length;
        i++
    ) {
        if (/\d/.test(value[i])) {
            count++;

            if (count === digitIndex) {
                return i + 1;
            }
        }
    }

    return value.length;
}

export const PhoneInput = ({
    value,
    onChange,
    placeholder = '+7 (___) ___-__',
    ...props
}: PhoneInputProps) => {
    const inputRef = useRef<HTMLInputElement>(null);

    const digits = extractDigits(value);
    const formattedValue = formatPhone(digits);

    const updateValue = (
        nextDigits: string,
        cursorDigitIndex?: number
    ) => {
        onChange(toCanonicalPhone(nextDigits));

        if (cursorDigitIndex === undefined) {
            return;
        }

        requestAnimationFrame(() => {
            const input = inputRef.current;

            if (!input) {
                return;
            }

            const formatted = formatPhone(nextDigits);

            const position = getPositionForDigitIndex(
                formatted,
                cursorDigitIndex
            );

            input.setSelectionRange(position, position);
        });
    };

    const handleChange = (
        event: ChangeEvent<HTMLInputElement>
    ) => {
        const nextDigits = extractDigits(
            event.target.value
        );

        onChange(toCanonicalPhone(nextDigits));
    };

    const handleKeyDown = (
        event: KeyboardEvent<HTMLInputElement>
    ) => {
        if (
            event.key !== 'Backspace' &&
            event.key !== 'Delete'
        ) {
            return;
        }

        const input = inputRef.current;

        if (!input) {
            return;
        }

        const start = input.selectionStart ?? 0;
        const end = input.selectionEnd ?? 0;

        event.preventDefault();

        const startDigitIndex = getDigitIndex(
            input.value,
            start
        );

        const endDigitIndex = getDigitIndex(
            input.value,
            end
        );

        let nextDigits = digits;
        let cursorDigitIndex = startDigitIndex;

        /*
         * Удаление выделенного диапазона.
         */
        if (start !== end) {
            nextDigits =
                digits.slice(0, startDigitIndex) +
                digits.slice(endDigitIndex);

            cursorDigitIndex = startDigitIndex;
        }

        /*
         * Backspace — удалить цифру слева от курсора.
         */
        else if (event.key === 'Backspace') {
            if (startDigitIndex === 0) {
                return;
            }

            const deleteIndex =
                startDigitIndex - 1;

            nextDigits =
                digits.slice(0, deleteIndex) +
                digits.slice(deleteIndex + 1);

            cursorDigitIndex = deleteIndex;
        }

        /*
         * Delete — удалить цифру справа от курсора.
         */
        else {
            if (startDigitIndex >= digits.length) {
                return;
            }

            const deleteIndex = startDigitIndex;

            nextDigits =
                digits.slice(0, deleteIndex) +
                digits.slice(deleteIndex + 1);

            cursorDigitIndex = startDigitIndex;
        }

        updateValue(
            nextDigits,
            cursorDigitIndex
        );
    };

    return (
        <input
            {...props}
            ref={inputRef}
            type="tel"
            value={formattedValue}
            onChange={handleChange}
            onKeyDown={handleKeyDown}
            placeholder={placeholder}
        />
    );
};