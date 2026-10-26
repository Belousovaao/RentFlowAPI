/** Нормализация российского номера к виду +7XXXXXXXXXX */
export function normalizePhone(input: string): string | null {
    const digits = input.replace(/\D/g, '');
    if (digits.length === 11 && digits.startsWith('7')) {
        return `+7${digits.slice(1)}`;
    }
    if (digits.length === 11 && digits.startsWith('8')) {
        return `+7${digits.slice(1)}`;
    }
    if (digits.length === 10) {
        return `+7${digits}`;
    }
    return null;
}

export function isValidPhone(input: string): boolean {
    return normalizePhone(input) !== null;
}

const EMAIL_RE = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export function isValidEmail(email: string): boolean {
    return EMAIL_RE.test(email.trim());
}

export function isValidOtpCode(code: string): boolean {
    return /^\d{6}$/.test(code.trim());
}
