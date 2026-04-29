import { useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { X, Mail, Smartphone } from 'lucide-react';
import { useI18n } from '@/app/providers/I18nProvider';
import { useAuthStore } from '../stores/authStore';
import {
    isValidEmail,
    isValidPassword,
    isValidPhone,
    isValidSmsCode,
    normalizePhone,
} from '../validation';

type Mode = 'login' | 'register';
type Method = 'phone' | 'email';

interface AuthModalProps {
    isOpen: boolean;
    onClose: () => void;
}

export const AuthModal = ({ isOpen, onClose }: AuthModalProps) => {
    const { t } = useI18n();
    const {
        loginWithEmail,
        registerWithEmail,
        sendSmsCode,
        loginWithPhone,
        registerWithPhone,
        isLoading,
    } = useAuthStore();

    const [mode, setMode] = useState<Mode>('login');
    const [method, setMethod] = useState<Method>('email');
    const [smsSent, setSmsSent] = useState(false);

    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [passwordConfirm, setPasswordConfirm] = useState('');
    const [phone, setPhone] = useState('');
    const [smsCode, setSmsCode] = useState('');
    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');

    const [errors, setErrors] = useState<Record<string, string>>({});
    const [formError, setFormError] = useState('');

    const resetFields = () => {
        setEmail('');
        setPassword('');
        setPasswordConfirm('');
        setPhone('');
        setSmsCode('');
        setFirstName('');
        setLastName('');
        setSmsSent(false);
        setErrors({});
        setFormError('');
    };

    const handleClose = () => {
        resetFields();
        setMode('login');
        setMethod('email');
        onClose();
    };

    const validateEmailForm = (): boolean => {
        const e: Record<string, string> = {};
        if (!isValidEmail(email)) e.email = t('auth.error.email');
        if (!isValidPassword(password)) e.password = t('auth.error.password');
        if (mode === 'register') {
            if (password !== passwordConfirm) e.passwordConfirm = t('auth.error.passwordMismatch');
            if (!firstName.trim()) e.firstName = t('auth.error.firstName');
            if (!lastName.trim()) e.lastName = t('auth.error.lastName');
        }
        setErrors(e);
        return Object.keys(e).length === 0;
    };

    const validatePhoneForm = (): boolean => {
        const e: Record<string, string> = {};
        if (!isValidPhone(phone)) e.phone = t('auth.error.phone');
        if (smsSent) {
            if (!isValidSmsCode(smsCode)) e.smsCode = t('auth.error.smsCode');
        }
        if (mode === 'register' && smsSent) {
            if (!firstName.trim()) e.firstName = t('auth.error.firstName');
            if (!lastName.trim()) e.lastName = t('auth.error.lastName');
        }
        setErrors(e);
        return Object.keys(e).length === 0;
    };

    const handleSendSms = async () => {
        setFormError('');
        if (!isValidPhone(phone)) {
            setErrors({ phone: t('auth.error.phone') });
            return;
        }
        try {
            await sendSmsCode(normalizePhone(phone)!);
            setSmsSent(true);
        } catch {
            setFormError(t('auth.error.generic'));
        }
    };

    const handleEmailSubmit = async (ev: React.FormEvent) => {
        ev.preventDefault();
        setFormError('');
        if (!validateEmailForm()) return;
        try {
            if (mode === 'login') {
                await loginWithEmail(email, password);
            } else {
                await registerWithEmail({
                    email: email.trim(),
                    password,
                    firstName: firstName.trim(),
                    lastName: lastName.trim(),
                });
            }
            handleClose();
        } catch {
            setFormError(t('auth.error.generic'));
        }
    };

    const handlePhoneSubmit = async (ev: React.FormEvent) => {
        ev.preventDefault();
        setFormError('');
        if (!validatePhoneForm()) return;
        const n = normalizePhone(phone);
        if (!n) {
            setErrors({ phone: t('auth.error.phone') });
            return;
        }
        try {
            if (mode === 'login') {
                if (!smsSent) {
                    await handleSendSms();
                    return;
                }
                await loginWithPhone(n, smsCode);
            } else {
                if (!smsSent) {
                    await handleSendSms();
                    return;
                }
                await registerWithPhone({
                    phone: n,
                    code: smsCode,
                    firstName: firstName.trim(),
                    lastName: lastName.trim(),
                });
            }
            handleClose();
        } catch {
            setFormError(t('auth.error.generic'));
        }
    };

    return (
        <AnimatePresence>
            {isOpen && (
                <div className="fixed inset-0 z-[200] flex items-center justify-center">
                    {/* Оверлей */}
                    <motion.div
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        exit={{ opacity: 0 }}
                        onClick={handleClose}
                        className="absolute inset-0 bg-black/50 backdrop-blur-sm"
                    />
                    
                    {/* Модалка */}
                    <motion.div
                        initial={{ opacity: 0, scale: 0.95 }}
                        animate={{ opacity: 1, scale: 1 }}
                        exit={{ opacity: 0, scale: 0.95 }}
                        transition={{ type: 'spring', damping: 25, stiffness: 300 }}
                        className="relative w-full max-w-md mx-4 bg-white dark:bg-gray-900 rounded-2xl shadow-2xl"
                        onClick={(e) => e.stopPropagation()}
                    >
                        <div className="relative rounded-2xl border border-gray-200 bg-white p-6 shadow-2xl dark:border-gray-700 dark:bg-gray-900">
                            <button
                                type="button"
                                onClick={handleClose}
                                className="absolute right-4 top-4 rounded-full p-1.5 text-gray-500 hover:bg-gray-100 dark:hover:bg-gray-800"
                                aria-label={t('common.cancel')}
                            >
                                <X className="h-5 w-5" />
                            </button>

                            <h2 className="text-xl font-bold text-gray-900 dark:text-white pr-8">
                                {mode === 'login' ? t('auth.loginTitle') : t('auth.registerTitle')}
                            </h2>

                            <div className="mt-4 flex rounded-lg bg-gray-100 p-1 dark:bg-gray-800">
                                <button
                                    type="button"
                                    onClick={() => {
                                        setMode('login');
                                        setSmsSent(false);
                                        setErrors({});
                                        setFormError('');
                                    }}
                                    className={`flex-1 rounded-md py-2 text-sm font-medium transition-colors ${
                                        mode === 'login'
                                            ? 'bg-white text-primary-600 shadow dark:bg-gray-700'
                                            : 'text-gray-600 dark:text-gray-400'
                                    }`}
                                >
                                    {t('auth.tabLogin')}
                                </button>
                                <button
                                    type="button"
                                    onClick={() => {
                                        setMode('register');
                                        setSmsSent(false);
                                        setErrors({});
                                        setFormError('');
                                    }}
                                    className={`flex-1 rounded-md py-2 text-sm font-medium transition-colors ${
                                        mode === 'register'
                                            ? 'bg-white text-primary-600 shadow dark:bg-gray-700'
                                            : 'text-gray-600 dark:text-gray-400'
                                    }`}
                                >
                                    {t('auth.tabRegister')}
                                </button>
                            </div>

                            <div className="mt-4 flex gap-2">
                                <button
                                    type="button"
                                    onClick={() => {
                                        setMethod('email');
                                        setSmsSent(false);
                                        setErrors({});
                                        setFormError('');
                                    }}
                                    className={`flex flex-1 items-center justify-center gap-2 rounded-lg border py-2.5 text-sm font-medium transition-colors ${
                                        method === 'email'
                                            ? 'border-primary-600 bg-primary-50 text-primary-700 dark:bg-primary-900/20'
                                            : 'border-gray-200 dark:border-gray-600'
                                    }`}
                                >
                                    <Mail className="h-4 w-4" />
                                    {t('auth.methodEmail')}
                                </button>
                                <button
                                    type="button"
                                    onClick={() => {
                                        setMethod('phone');
                                        setSmsSent(false);
                                        setErrors({});
                                        setFormError('');
                                    }}
                                    className={`flex flex-1 items-center justify-center gap-2 rounded-lg border py-2.5 text-sm font-medium transition-colors ${
                                        method === 'phone'
                                            ? 'border-primary-600 bg-primary-50 text-primary-700 dark:bg-primary-900/20'
                                            : 'border-gray-200 dark:border-gray-600'
                                    }`}
                                >
                                    <Smartphone className="h-4 w-4" />
                                    {t('auth.methodPhone')}
                                </button>
                            </div>

                            <p className="mt-3 text-xs text-gray-500 dark:text-gray-400">
                                {method === 'phone' ? t('auth.demoSmsHint') : t('auth.demoAdminHint')}
                            </p>

                            {formError && (
                                <p className="mt-3 text-sm text-red-600 dark:text-red-400">{formError}</p>
                            )}

                            {method === 'email' ? (
                                <form onSubmit={handleEmailSubmit} className="mt-4 space-y-3">
                                    <div>
                                        <label className="block text-xs font-medium text-gray-600 dark:text-gray-400">
                                            {t('auth.email')}
                                        </label>
                                        <input
                                            type="email"
                                            autoComplete="email"
                                            value={email}
                                            onChange={(e) => setEmail(e.target.value)}
                                            className="mt-1 w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm dark:border-gray-600 dark:bg-gray-800"
                                        />
                                        {errors.email && (
                                            <p className="mt-1 text-xs text-red-600">{errors.email}</p>
                                        )}
                                    </div>
                                    <div>
                                        <label className="block text-xs font-medium text-gray-600 dark:text-gray-400">
                                            {t('auth.password')}
                                        </label>
                                        <input
                                            type="password"
                                            autoComplete={mode === 'login' ? 'current-password' : 'new-password'}
                                            value={password}
                                            onChange={(e) => setPassword(e.target.value)}
                                            className="mt-1 w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm dark:border-gray-600 dark:bg-gray-800"
                                        />
                                        {errors.password && (
                                            <p className="mt-1 text-xs text-red-600">{errors.password}</p>
                                        )}
                                    </div>
                                    {mode === 'register' && (
                                        <>
                                            <div>
                                                <label className="block text-xs font-medium text-gray-600 dark:text-gray-400">
                                                    {t('auth.passwordConfirm')}
                                                </label>
                                                <input
                                                    type="password"
                                                    autoComplete="new-password"
                                                    value={passwordConfirm}
                                                    onChange={(e) => setPasswordConfirm(e.target.value)}
                                                    className="mt-1 w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm dark:border-gray-600 dark:bg-gray-800"
                                                />
                                                {errors.passwordConfirm && (
                                                    <p className="mt-1 text-xs text-red-600">
                                                        {errors.passwordConfirm}
                                                    </p>
                                                )}
                                            </div>
                                            <div className="grid grid-cols-2 gap-2">
                                                <div>
                                                    <label className="block text-xs font-medium text-gray-600 dark:text-gray-400">
                                                        {t('auth.firstName')}
                                                    </label>
                                                    <input
                                                        value={firstName}
                                                        onChange={(e) => setFirstName(e.target.value)}
                                                        className="mt-1 w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm dark:border-gray-600 dark:bg-gray-800"
                                                    />
                                                    {errors.firstName && (
                                                        <p className="mt-1 text-xs text-red-600">
                                                            {errors.firstName}
                                                        </p>
                                                    )}
                                                </div>
                                                <div>
                                                    <label className="block text-xs font-medium text-gray-600 dark:text-gray-400">
                                                        {t('auth.lastName')}
                                                    </label>
                                                    <input
                                                        value={lastName}
                                                        onChange={(e) => setLastName(e.target.value)}
                                                        className="mt-1 w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm dark:border-gray-600 dark:bg-gray-800"
                                                    />
                                                    {errors.lastName && (
                                                        <p className="mt-1 text-xs text-red-600">
                                                            {errors.lastName}
                                                        </p>
                                                    )}
                                                </div>
                                            </div>
                                        </>
                                    )}
                                    <button
                                        type="submit"
                                        disabled={isLoading}
                                        className="w-full rounded-lg bg-primary-600 py-2.5 text-sm font-semibold text-white hover:bg-primary-700 disabled:opacity-50"
                                    >
                                        {isLoading ? t('common.loading') : mode === 'login' ? t('auth.submitLogin') : t('auth.submitRegister')}
                                    </button>
                                </form>
                            ) : (
                                <form onSubmit={handlePhoneSubmit} className="mt-4 space-y-3">
                                    <div>
                                        <label className="block text-xs font-medium text-gray-600 dark:text-gray-400">
                                            {t('auth.phone')}
                                        </label>
                                        <input
                                            type="tel"
                                            value={phone}
                                            onChange={(e) => setPhone(e.target.value)}
                                            placeholder="+7 900 123-45-67"
                                            className="mt-1 w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm dark:border-gray-600 dark:bg-gray-800"
                                        />
                                        {errors.phone && (
                                            <p className="mt-1 text-xs text-red-600">{errors.phone}</p>
                                        )}
                                    </div>
                                    {!smsSent && (
                                        <button
                                            type="button"
                                            onClick={handleSendSms}
                                            disabled={isLoading}
                                            className="w-full rounded-lg border border-primary-600 py-2 text-sm font-medium text-primary-600 hover:bg-primary-50 dark:hover:bg-primary-900/20 disabled:opacity-50"
                                        >
                                            {t('auth.sendSms')}
                                        </button>
                                    )}
                                    {smsSent && (
                                        <>
                                            <div>
                                                <label className="block text-xs font-medium text-gray-600 dark:text-gray-400">
                                                    {t('auth.smsCode')}
                                                </label>
                                                <input
                                                    type="text"
                                                    inputMode="numeric"
                                                    maxLength={6}
                                                    value={smsCode}
                                                    onChange={(e) => setSmsCode(e.target.value.replace(/\D/g, ''))}
                                                    className="mt-1 w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm tracking-widest dark:border-gray-600 dark:bg-gray-800"
                                                />
                                                {errors.smsCode && (
                                                    <p className="mt-1 text-xs text-red-600">{errors.smsCode}</p>
                                                )}
                                            </div>
                                            {mode === 'register' && (
                                                <div className="grid grid-cols-2 gap-2">
                                                    <div>
                                                        <label className="block text-xs font-medium text-gray-600 dark:text-gray-400">
                                                            {t('auth.firstName')}
                                                        </label>
                                                        <input
                                                            value={firstName}
                                                            onChange={(e) => setFirstName(e.target.value)}
                                                            className="mt-1 w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm dark:border-gray-600 dark:bg-gray-800"
                                                        />
                                                        {errors.firstName && (
                                                            <p className="mt-1 text-xs text-red-600">
                                                                {errors.firstName}
                                                            </p>
                                                        )}
                                                    </div>
                                                    <div>
                                                        <label className="block text-xs font-medium text-gray-600 dark:text-gray-400">
                                                            {t('auth.lastName')}
                                                        </label>
                                                        <input
                                                            value={lastName}
                                                            onChange={(e) => setLastName(e.target.value)}
                                                            className="mt-1 w-full rounded-lg border border-gray-300 bg-white px-3 py-2 text-sm dark:border-gray-600 dark:bg-gray-800"
                                                        />
                                                        {errors.lastName && (
                                                            <p className="mt-1 text-xs text-red-600">
                                                                {errors.lastName}
                                                            </p>
                                                        )}
                                                    </div>
                                                </div>
                                            )}
                                            <button
                                                type="submit"
                                                disabled={isLoading}
                                                className="w-full rounded-lg bg-primary-600 py-2.5 text-sm font-semibold text-white hover:bg-primary-700 disabled:opacity-50"
                                            >
                                                {isLoading
                                                    ? t('common.loading')
                                                    : mode === 'login'
                                                      ? t('auth.submitLogin')
                                                      : t('auth.submitRegister')}
                                            </button>
                                        </>
                                    )}
                                </form>
                            )}
                        </div>
                    </motion.div>
                </div>
            )}
        </AnimatePresence>
    );
};