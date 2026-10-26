import { useEffect, useMemo, useState, type InputHTMLAttributes, } from 'react';
import { ArrowLeft, LogIn, Mail, Phone, ShieldCheck, UserRound, X, } from 'lucide-react';
import { useAuthStore } from '../stores/authStore';
import { useAuthModalStore } from '../stores/authModalStore';
import { authApi, type OtpChannel, type PersonalDataConsentDocument } from '../api/auth.api';
import type { RegisterFormData } from '../types';
import { CustomerType } from '@/entities/customer/customer.type';
import { Button } from '@/features/ui/components/Button';
import { PhoneInput } from '@/features/ui/components/PhoneInput';
import { EmailInput } from '@/features/ui/components/EmailInput';
import PersonalDataConsentCheckbox from '@/features/ui/components/PersonalDataConsentCheckbox';

type Step =
    | 'contact'
    | 'otp'
    | 'registration';

export const AuthModal = () => {
    const {
        isOpen,
        completeAuth,
        close,
    } = useAuthModalStore();

    const {
        otpSession,
        pendingRegistration,
        isLoading,
        sendOtp,
        verifyOtp,
        register,
        resetOtpFlow,
    } = useAuthStore();

    const [step, setStep] = useState<Step>('contact');
    const [channel, setChannel] = useState<OtpChannel>('phone');
    const [destination, setDestination] = useState('');
    const [otpCode, setOtpCode] = useState('');
    const [error, setError] = useState<string | null>(null);
    const [registerData, setRegisterData] = useState<RegisterFormData>({
            customerType: CustomerType.Individual,
            phone: '',
            email: '',
            firstName: '',
            lastName: '',
            inn: '',
            consentDocumentId: '',
            personalDataConsentAccepted: false,
        });
    const [now, setNow] = useState(() => Date.now());
    const [personalDataConsent, setPersonalDataConsent] = useState(false);
    const [consentError, setConsentError] = useState(false);
    const [consentDocument, setConsentDocument] = useState<PersonalDataConsentDocument | null>(null);
    const [consentDocumentLoading, setConsentDocumentLoading] = useState(false);

    useEffect(() => {
        if (!isOpen) {
            return;
        }

        const timer = window.setInterval(() => {
            setNow(Date.now());
        }, 1000);

        return () => {
            window.clearInterval(timer);
        };
    }, [isOpen]);

    useEffect(() => {
        if (!isOpen) {
            return;
        }

        if (pendingRegistration) {
            setStep('registration');

            setRegisterData((current) => ({
                ...current,

                phone:
                    pendingRegistration.channel === 'phone'
                        ? pendingRegistration.destination
                        : current.phone ?? '',

                email:
                    pendingRegistration.channel === 'email'
                        ? pendingRegistration.destination
                        : current.email ?? '',
            }));

            return;
        }

        if (otpSession) {
            setStep('otp');
            setChannel(otpSession.channel);
            setDestination(otpSession.destination);

            return;
        }
        setStep('contact');
    }, [isOpen, otpSession, pendingRegistration,]);

    useEffect(() => {
        if (isOpen) {
            return;
        }

        setStep('contact');
        setChannel('phone');
        setDestination('');
        setOtpCode('');
        setError(null);
        setRegisterData({
            customerType: CustomerType.Individual,
            phone: '',
            email: '',
            firstName: '',
            lastName: '',
            inn: '',
            consentDocumentId: '',
            personalDataConsentAccepted: false,
        });
        setPersonalDataConsent(false);
        setConsentError(false);
        setConsentDocument(null);
        setConsentDocumentLoading(false);
        resetOtpFlow();
    }, [isOpen, resetOtpFlow]);

    useEffect(() => {
        if (!isOpen || step !== 'registration' || consentDocument) {
            return;
        }

        let cancelled = false;

        const loadConsentDocument = async () => {
            setConsentDocumentLoading(true);
            setError(null);

            try {
                const document = await authApi.getPersonalDataProcessingConsent();

                if (!cancelled) {
                    setConsentDocument(document);
                }
            } catch {
                if (!cancelled) {
                    setError('Не удалось загрузить текст согласия. Попробуйте позже.');
                }
            } finally {
                if (!cancelled) {
                    setConsentDocumentLoading(false);
                }
            }
        };

        void loadConsentDocument();

        return () => {
            cancelled = true;
        };
    }, [isOpen, step, consentDocument]);

    const otpExpired = useMemo(() => {
        if (!otpSession) {
            return false;
        }
        return now >= otpSession.expiresAt;
    }, [now, otpSession]);

    const resendAvailable = useMemo(() => {
        if (!otpSession) {
            return false;
        }

        if (otpSession.retryAvailableAt === null) {
            return true;
        }

        return now >= otpSession.retryAvailableAt;
    }, [now, otpSession]);

    const expiresIn = useMemo(() => {
        if (!otpSession) {
            return 0;
        }

        return Math.max(
            0,
            Math.ceil((otpSession.expiresAt - now) / 1000)
        );
    }, [now, otpSession]);

    const retryAfter = useMemo(() => {
        if (!otpSession || otpSession.retryAvailableAt === null) {
            return 0;
        }

        return Math.max(0, Math.ceil((otpSession.retryAvailableAt - now) / 1000));
    }, [now, otpSession]);

    const isRegistrationValid = !!registerData.phone?.trim() &&
    (
        registerData.customerType === CustomerType.Organization ||
        (
            !!registerData.firstName?.trim() && !!registerData.lastName?.trim()
        )
    ) &&
    (
        registerData.customerType === CustomerType.Individual || !!registerData.inn?.trim()
    );

    if (!isOpen) {
        return null;
    }

    const handleSendOtp = async () => {
        setError(null);

        const value = destination.trim();

        if (!value) {
            setError( channel === 'phone'
                ? 'Введите номер телефона.'
                : 'Введите email.'
            );

            return;
        }

        try {
            const normalizedDestination = destination.trim().toLowerCase();
            await sendOtp(channel, normalizedDestination);
            setOtpCode('');
            setStep('otp');
        } catch (err) {
            setError(getErrorMessage(err));
        }
    };

    const handleVerifyOtp = async () => {
        setError(null);
        const code = otpCode.trim();
        if (!/^\d{6}$/.test(code)) {
            setError('Введите 6-значный код.');
            return;
        }

        try {
            const result = await verifyOtp(code);
            if (result === 'authenticated') {
                completeAuth();
                return;
            }
            setStep('registration');
        } catch (err) {
            setError(getErrorMessage(err));
        }
    };

    const handleResend = async () => {
        if (!resendAvailable) {
            return;
        }

        setError(null);

        try {
            await sendOtp(channel, destination);
            setOtpCode('');
        } catch (err) {
            setError(getErrorMessage(err));
        }
    };

    const handleRegistration = async () => {
        setError(null);

        if (!personalDataConsent) {
            setConsentError(true);
            return;
        }

        if (!consentDocument || consentDocumentLoading) {
            setError('Текст согласия ещё не загружен. Попробуйте позже.');
            return;
        }

        setConsentError(false);

        const data = {
            ...registerData,
            phone: registerData.phone?.trim() || undefined,
            email: registerData.email?.trim().toLowerCase() || undefined,
            firstName: registerData.firstName?.trim() || undefined,
            lastName: registerData.lastName?.trim() || undefined,
            inn: registerData.inn?.trim() || undefined,
            consentDocumentId: consentDocument.id,
            personalDataConsentAccepted: personalDataConsent,
        };

        if (!data.phone) {
            setError('Введите номер телефона.');
            return;
        }

        if (data.customerType === CustomerType.Individual || data.customerType === CustomerType.IndividualEntrepreneur) {
            if (!data.firstName) {
                setError('Введите имя.');
                return;
            }

            if (!data.lastName) {
                setError('Введите фамилию.');
                return;
            }
        }

        if (data.customerType === CustomerType.IndividualEntrepreneur || data.customerType === CustomerType.Organization) {
            if (!data.inn) {
                setError('Введите ИНН.');
                return;
            }
        }

        try {
            await register(data);
            completeAuth();
        } catch (err) {
            setError(getErrorMessage(err));
        }
    };

    const updateRegisterField = <K extends keyof RegisterFormData>(
        field: K,
        value: RegisterFormData[K]
    ) => {
        setRegisterData((current) => ({
            ...current,
            [field]: value,
        }));
    };

    const handleBack = () => {
        setError(null);

        if (step === 'otp') {
            resetOtpFlow();
            setOtpCode('');
            setStep('contact');
            return;
        }

        if (step === 'registration') {
            resetOtpFlow();
            setStep('contact');
            setOtpCode('');
        }
    };

    const title = step === 'contact'
        ? 'Вход / регистрация'
        : step === 'otp'
            ? 'Подтверждение контакта'
            : 'Создание аккаунта';
    return (
        <div
            className="fixed inset-0 z-[9999] flex items-center justify-center bg-black/60 p-4 backdrop-blur-sm"
            onMouseDown={(event) => {
                if (event.target === event.currentTarget && !isLoading) {
                    close();
                }
            }}
        >
            <div className="flex max-h-[90vh] w-full max-w-lg flex-col overflow-hidden rounded-2xl bg-white shadow-2xl dark:bg-gray-900">
                {/* Header */}
                <div className="border-b border-gray-100 px-6 py-5 dark:border-gray-800">
                    <div className="flex items-start justify-between gap-4">
                        <div className="flex items-start gap-3">
                            <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl bg-primary-50 text-primary-600 dark:bg-primary-950/40">
                                {step === 'contact' && (
                                    <LogIn className="h-5 w-5" />
                                )}
                                {step === 'otp' && (
                                    <ShieldCheck className="h-5 w-5" />
                                )}
                                {step === 'registration' && (
                                    <UserRound className="h-5 w-5" />
                                )}
                            </div>
                            <div>
                                <h2 className="text-xl font-semibold text-gray-900 dark:text-white">
                                    {title}
                                </h2>
                            </div>
                        </div>

                        <button
                            type="button"
                            onClick={close}
                            disabled={isLoading}
                            className="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg text-gray-400 transition-colors hover:bg-gray-100 hover:text-gray-700 disabled:cursor-not-allowed disabled:opacity-50 dark:hover:bg-gray-800 dark:hover:text-gray-200"
                            aria-label="Закрыть"
                        >
                            <X className="h-5 w-5" />
                        </button>
                    </div>
                </div>

                {/* Content */}
                <div className="flex-1 overflow-y-auto px-6 py-6">
                    {step === 'contact' && (
                        <section className="space-y-5">
                            <div>
                                <div className="grid grid-cols-2 gap-2 rounded-xl bg-gray-100 p-1 dark:bg-gray-800">
                                    <button
                                        type="button"
                                        onClick={() => {
                                            setChannel('phone');
                                            setDestination('');
                                            setError(null);
                                        }}
                                        disabled={isLoading}
                                        className={`flex items-center justify-center gap-2 rounded-lg px-3 py-2.5 text-sm font-medium transition-all ${
                                            channel === 'phone'
                                                ? 'bg-white text-primary-600 shadow-sm dark:bg-gray-700 dark:text-primary-400'
                                                : 'text-gray-500 hover:text-gray-800 dark:text-gray-400 dark:hover:text-gray-200'
                                        }`}
                                    >
                                        <Phone className="h-4 w-4" />
                                        Телефон
                                    </button>

                                    <button
                                        type="button"
                                        onClick={() => {
                                            setChannel('email');
                                            setDestination('');
                                            setError(null);
                                        }}
                                        disabled={isLoading}
                                        className={`flex items-center justify-center gap-2 rounded-lg px-3 py-2.5 text-sm font-medium transition-all ${
                                            channel === 'email'
                                                ? 'bg-white text-primary-600 shadow-sm dark:bg-gray-700 dark:text-primary-400'
                                                : 'text-gray-500 hover:text-gray-800 dark:text-gray-400 dark:hover:text-gray-200'
                                        }`}
                                    >
                                        <Mail className="h-4 w-4" />
                                        Email
                                    </button>
                                </div>
                            </div>

                            <div>
                                <label className="mb-2 block text-sm font-medium text-gray-700 dark:text-gray-200">
                                    {channel === 'phone'
                                        ? 'Номер телефона'
                                        : 'Email'}
                                </label>

                                <div className="relative">
                                    {channel === 'phone' ? (
                                        <Phone className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" />
                                    ) : (
                                        <Mail className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-gray-400" />
                                    )}

                                    {channel === 'phone' ? (
                                        <PhoneInput
                                            value={destination}
                                            onChange={setDestination}
                                            autoComplete="tel"
                                            disabled={isLoading}
                                            className="w-full rounded-xl border border-gray-200 bg-white py-3 pl-10 pr-3 text-sm text-gray-900 outline-none transition focus:border-primary-500 focus:ring-2 focus:ring-primary-500/20 disabled:cursor-not-allowed disabled:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-white dark:focus:border-primary-500 dark:disabled:bg-gray-800/60"
                                        />
                                    ) : (
                                        <EmailInput
                                            value={destination}
                                            onChange={setDestination}
                                            placeholder="example@mail.ru"
                                            className="w-full rounded-xl border border-gray-200 bg-white py-3 pl-10 pr-3 text-sm text-gray-900 outline-none transition focus:border-primary-500 focus:ring-2 focus:ring-primary-500/20 disabled:cursor-not-allowed disabled:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-white dark:disabled:bg-gray-800/60"
                                        />
                                    )}
                                </div>
                            </div>

                            {error && (
                                <ErrorMessage message={error} />
                            )}

                            <div className="flex items-center justify-between gap-3 border-t border-gray-100 pt-5 dark:border-gray-800">
                                <Button
                                    onClick={handleSendOtp}
                                    disabled={
                                        isLoading ||
                                        !destination.trim()
                                    }
                                >
                                    {isLoading
                                        ? 'Отправка...'
                                        : 'Получить код'}
                                </Button>
                            </div>
                        </section>
                    )}

                    {step === 'otp' && (
                        <section className="space-y-5">
                            <div className="rounded-xl bg-primary-50 p-4 dark:bg-primary-950/20">
                                <div className="flex gap-3">
                                    <ShieldCheck className="mt-0.5 h-5 w-5 shrink-0 text-primary-600" />

                                    <div>
                                        <p className="text-sm font-medium text-gray-800">
                                            Код подтверждения
                                        </p>

                                        <p className="mt-1 text-xs leading-5 text-gray-600">
                                            Мы отправили код на{' '}
                                            <span className="font-medium text-gray-700">
                                                {destination}
                                            </span>
                                        </p>
                                    </div>
                                </div>
                            </div>

                            <div>
                                <label className="mb-2 block text-sm font-medium text-gray-700 dark:text-gray-200">
                                    Одноразовый код
                                </label>

                                <input
                                    type="text"
                                    inputMode="numeric"
                                    autoComplete="one-time-code"
                                    maxLength={6}
                                    value={otpCode}
                                    onChange={(event) => {
                                        const value = event.target.value
                                            .replace(/\D/g, '')
                                            .slice(0, 6);

                                        setOtpCode(value);
                                    }}
                                    placeholder="000000"
                                    disabled={
                                        isLoading ||
                                        otpExpired
                                    }
                                    className="w-full rounded-xl border border-gray-200 bg-white px-4 py-3 text-center text-2xl font-semibold tracking-[0.4em] text-gray-900 outline-none transition focus:border-primary-500 focus:ring-2 focus:ring-primary-500/20 disabled:cursor-not-allowed disabled:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-white dark:disabled:bg-gray-800/60"
                                />
                            </div>

                            <div className="flex items-center justify-between text-xs">
                                <span className="text-gray-500 dark:text-gray-400">
                                    {otpExpired
                                        ? 'Срок действия кода истёк.'
                                        : `Код действует ещё ${formatSeconds(expiresIn)}`}
                                </span>

                                <button
                                    type="button"
                                    onClick={handleResend}
                                    disabled={
                                        isLoading ||
                                        !resendAvailable
                                    }
                                    className="font-medium text-primary-600 transition hover:text-primary-700 disabled:cursor-not-allowed disabled:text-gray-400"
                                >
                                    {resendAvailable
                                        ? 'Отправить повторно'
                                        : `Повторно через ${retryAfter} с`}
                                </button>
                            </div>

                            {error && (
                                <ErrorMessage message={error} />
                            )}

                            <div className="flex items-center justify-between gap-3 border-t border-gray-100 pt-5 dark:border-gray-800">
                                <button
                                    type="button"
                                    onClick={handleBack}
                                    disabled={isLoading}
                                    className="flex items-center gap-2 text-sm font-medium text-gray-500 transition hover:text-gray-800 disabled:cursor-not-allowed disabled:opacity-50 dark:text-gray-400 dark:hover:text-gray-200"
                                >
                                    <ArrowLeft className="h-4 w-4" />
                                    Изменить контакт
                                </button>

                                <Button
                                    onClick={handleVerifyOtp}
                                    disabled={
                                        isLoading ||
                                        otpExpired ||
                                        otpCode.length !== 6
                                    }
                                >
                                    {isLoading
                                        ? 'Проверка...'
                                        : 'Продолжить'}
                                </Button>
                            </div>
                        </section>
                    )}

                    {step === 'registration' && (
                        <section className="space-y-5">
                            <div className="rounded-xl bg-primary-50 p-4 dark:bg-primary-950/20">
                                <div className="flex gap-3">
                                    <UserRound className="mt-0.5 h-5 w-5 shrink-0 text-primary-600" />

                                    <div>
                                        <p className="text-sm font-medium text-gray-800 dark:text-gray-700">
                                            Завершение регистрации
                                        </p>

                                        <p className="mt-1 text-xs leading-5 text-gray-500 dark:text-gray-600">
                                            Контакт уже подтверждён. Осталось заполнить данные аккаунта.
                                        </p>
                                    </div>
                                </div>
                            </div>

                            <div>
                                <label className="mb-2 block text-sm font-medium text-gray-700 dark:text-gray-200">
                                    Тип клиента
                                </label>

                                <select
                                    value={
                                        registerData.customerType
                                    }
                                    onChange={(event) =>
                                        updateRegisterField(
                                            'customerType',
                                            event.target
                                                .value as CustomerType
                                        )
                                    }
                                    disabled={isLoading}
                                    className="w-full rounded-xl border border-gray-200 bg-white px-3 py-3 text-sm text-gray-900 outline-none transition focus:border-primary-500 focus:ring-2 focus:ring-primary-500/20 disabled:cursor-not-allowed disabled:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-white dark:disabled:bg-gray-800/60"
                                >
                                    <option
                                        value={
                                            CustomerType.Individual
                                        }
                                    >
                                        Физическое лицо
                                    </option>

                                    <option
                                        value={
                                            CustomerType.IndividualEntrepreneur
                                        }
                                    >
                                        Индивидуальный предприниматель
                                    </option>

                                    <option
                                        value={
                                            CustomerType.Organization
                                        }
                                    >
                                        Организация
                                    </option>
                                </select>
                            </div>

                            <div className="grid gap-4 sm:grid-cols-2">
                                <div>
                                    <label className="mb-2 block text-sm font-medium text-gray-700 dark:text-gray-200">
                                        Телефон
                                        <span className="ml-1 text-red-500">*</span>
                                    </label>

                                    <PhoneInput
                                        value={registerData.phone ?? ''}
                                        onChange={(value) => updateRegisterField('phone', value)}
                                        disabled={isLoading || pendingRegistration?.channel === 'phone'}
                                        required
                                        autoComplete="tel"
                                        className="w-full rounded-xl border border-gray-200 bg-white px-3 py-3 text-sm text-gray-900 outline-none transition focus:border-primary-500 focus:ring-2 focus:ring-primary-500/20 disabled:cursor-not-allowed disabled:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-white dark:focus:border-primary-500 dark:disabled:bg-gray-800/60"
                                    />
                                </div>

                                <div>
                                    <label className="mb-2 block text-sm font-medium text-gray-700 dark:text-gray-200">
                                        Email
                                    </label>

                                    <EmailInput
                                        value={registerData.email ?? ''}
                                        onChange={(value) => updateRegisterField('email', value)}
                                        placeholder="example@mail.ru"
                                        className="w-full rounded-xl border border-gray-200 bg-white py-3 px-3 text-sm text-gray-900 outline-none transition focus:border-primary-500 focus:ring-2 focus:ring-primary-500/20 disabled:cursor-not-allowed disabled:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-white dark:disabled:bg-gray-800/60"
                                    />
                                </div>
                            </div>

                            {(
                                registerData.customerType === CustomerType.Individual ||
                                registerData.customerType === CustomerType.IndividualEntrepreneur
                            ) && (
                                <div className="grid gap-4 sm:grid-cols-2">
                                    <Field
                                        label="Имя"
                                        type="text"
                                        value={
                                            registerData.firstName ??
                                            ''
                                        }
                                        onChange={(value) =>
                                            updateRegisterField(
                                                'firstName',
                                                value
                                            )
                                        }
                                        disabled={isLoading}
                                        required
                                        autoComplete="given-name"
                                    />

                                    <Field
                                        label="Фамилия"
                                        type="text"
                                        value={registerData.lastName ?? ''}
                                        onChange={(value) => updateRegisterField( 'lastName', value)}
                                        disabled={isLoading}
                                        required
                                        autoComplete="family-name"
                                    />
                                </div>
                            )}

                            {(
                                registerData.customerType === CustomerType.IndividualEntrepreneur ||
                                registerData.customerType === CustomerType.Organization
                            ) && (
                                <Field
                                    label="ИНН"
                                    required
                                    type="text"
                                    value={registerData.inn ?? ''}
                                    onChange={(value) => updateRegisterField( 'inn', value.replace(/\D/g, ''))}
                                    disabled={isLoading}
                                    inputMode="numeric"
                                    autoComplete="off"
                                />
                            )}

                            <PersonalDataConsentCheckbox
                                checked={personalDataConsent}
                                document={consentDocument}
                                onChange={(checked) => {
                                    setPersonalDataConsent(checked);

                                    if (checked) {
                                        setConsentError(false);
                                    }
                                }}
                                error={
                                    consentError
                                        ? 'Для регистрации необходимо дать согласие на обработку персональных данных.'
                                        : undefined
                                }
                            />

                            {error && (
                                <ErrorMessage message={error} />
                            )}

                            <div className="flex items-center justify-between gap-3 border-t border-gray-100 pt-5 dark:border-gray-800">
                                <button
                                    type="button"
                                    onClick={handleBack}
                                    disabled={isLoading}
                                    className="flex items-center gap-2 text-sm font-medium text-gray-500 transition hover:text-gray-800 disabled:cursor-not-allowed disabled:opacity-50 dark:text-gray-400 dark:hover:text-gray-200"
                                >
                                    <ArrowLeft className="h-4 w-4" />
                                    Назад
                                </button>

                                <Button
                                    onClick={handleRegistration}
                                    disabled={isLoading || !isRegistrationValid}
                                >
                                    {isLoading
                                        ? 'Создание аккаунта...'
                                        : 'Зарегистрироваться'}
                                </Button>
                            </div>
                        </section>
                    )}
                </div>
            </div>
        </div>
    );
};

interface FieldProps {
    label: string;
    type: string;
    value: string;
    onChange: (value: string) => void;
    disabled?: boolean;
    required?: boolean;
    autoComplete?: string;
    inputMode?: InputHTMLAttributes<HTMLInputElement>['inputMode'];
}

const Field = ({
    label,
    type,
    value,
    onChange,
    disabled,
    required = false,
    autoComplete,
    inputMode,
}: FieldProps) => {
    return (
        <div>
            <label className="mb-2 block text-sm font-medium text-gray-700 dark:text-gray-200">
                {label}
                {required && (
                    <span className="ml-1 text-red-500">
                        *
                    </span>
                )}
            </label>

            <input
                type={type}
                value={value}
                onChange={(event) => onChange(event.target.value)}
                disabled={disabled}
                required={required}
                autoComplete={autoComplete}
                inputMode={inputMode}
                className="w-full rounded-xl border border-gray-200 bg-white px-3 py-3 text-sm text-gray-900 outline-none transition focus:border-primary-500 focus:ring-2 focus:ring-primary-500/20 disabled:cursor-not-allowed disabled:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-white dark:focus:border-primary-500 dark:disabled:bg-gray-800/60"
            />
        </div>
    );
};

const ErrorMessage = ({
    message,
}: {
    message: string;
}) => {
    return (
        <div className="rounded-xl border border-red-100 bg-red-50 px-4 py-3 text-sm text-red-700 dark:border-red-900/50 dark:bg-red-950/20 dark:text-red-300">
            {message}
        </div>
    );
};

function formatSeconds(seconds: number): string {
    const minutes = Math.floor(seconds / 60);
    const remainder = seconds % 60;

    if (minutes === 0) {
        return `${remainder} сек.`;
    }

    return `${minutes}:${remainder
        .toString()
        .padStart(2, '0')}`;
}

function getErrorMessage(error: unknown): string {
    if (
        typeof error === 'object' &&
        error !== null &&
        'response' in error
    ) {
        const response = (
            error as {
                response?: {
                    data?: {
                        message?: string;
                    };
                };
            }
        ).response;

        if (response?.data?.message) {
            return response.data.message;
        }
    }

    if (error instanceof Error) {
        return error.message;
    }

    return 'Произошла ошибка. Попробуйте ещё раз.';
}