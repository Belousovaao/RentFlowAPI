import { useState } from 'react';
import { useI18n } from '@/app/providers/I18nProvider';
import { CustomerDetailsDto, CustomerType, UpdateCustomerRequest, type CreateCustomerRequest } from '@/entities/customer/customer.type';
import toast from 'react-hot-toast';
import { Car, Save, User, SquareUser } from 'lucide-react';
import { FormSection } from './FormSection';
import { CustomerFormField } from './CustomerFormField';
import { customerInputClass } from '@/entities/customer/customer.formStyles';
import { DigitsInput } from '@/features/ui/components/DigitsInput';
import { DriverLicenseInput } from '@/features/ui/components/DriverLicenseInput';
import { PhoneInput } from '@/features/ui/components/PhoneInput';
import { EmailInput } from '@/features/ui/components/EmailInput';
import { DriverLicenseCategoriesInput } from '@/features/ui/components/DriverLicenseCategoriesInput';
import { DateInput } from '@/features/ui/components/DateInput';

interface IndividualFormProps {
    customer?: CustomerDetailsDto;
    readOnly?: boolean;
    onCreate?: (data: CreateCustomerRequest) => Promise<void>;
    onUpdate?: (data: UpdateCustomerRequest) => Promise<void>;
}

export const IndividualForm = ({
    customer,
    readOnly = false,
    onCreate,
    onUpdate }: IndividualFormProps) => {
        const { t } = useI18n();
        const isEditMode = !!customer;

        const [showPersonalInfo, setShowPersonalInfo] = useState(true);
        const [showPassport, setShowPassport] = useState(false);
        const [showDriverLicense, setShowDriverLicense] = useState(false);
        const [invalidFields, setInvalidFields] = useState<string[]>([]);

        const [firstName, setFirstName] = useState(customer?.firstName || '');
        const [lastName, setLastName] = useState(customer?.lastName || '');
        const [middleName, setMiddleName] = useState(customer?.middleName || '');

        const [phone, setPhone] = useState(customer?.phone || '');
        const [email, setEmail] = useState(customer?.email || '');

        const [birthDate, setBirthDate] = useState(customer?.dateOfBirth || '');
        const [passportSerial, setPassportSerial] = useState(customer?.passport?.serial || '');
        const [passportNumber, setPassportNumber] = useState(customer?.passport?.number || '');
        const [passportIssuedBy, setPassportIssuedBy] = useState(customer?.passport?.issuedBy || '');
        const [passportIssuedDate, setPassportIssuedDate] = useState(customer?.passport?.issuedDate || '');
        const [registrationAddress, setRegistrationAddress] = useState(customer?.passport?.registrationAddress || '');

        const [driverLicenseNumber, setDriverLicenseNumber] = useState(customer?.driverLicense?.number || '');
        const [driverLicenseCategories, setDriverLicenseCategories] = useState(customer?.driverLicense?.categories?.join(', ') || '');
        const [driverLicenseIssuedDate, setDriverLicenseIssuedDate] = useState(customer?.driverLicense?.issuedDate || '');
        const [driverLicenseValidUntil, setDriverLicenseValidUntil] = useState(customer?.driverLicense?.expirationDate || '');
        const [driverLicenseExperienceFrom, setDriverLicenseExperienceFrom] = useState(customer?.driverLicense?.experienceFrom?.toString() || '');

        const passportStarted = Boolean(
            passportSerial.trim() ||
            passportNumber.trim() ||
            passportIssuedBy.trim() ||
            passportIssuedDate ||
            registrationAddress.trim()
        );

        const driverLicenseStarted = Boolean(
            driverLicenseNumber.trim() ||
            driverLicenseCategories.trim() ||
            driverLicenseIssuedDate ||
            driverLicenseValidUntil ||
            driverLicenseExperienceFrom
        );
        const handleSave = async () => {
            if (!isEditMode) {
                const missingFields: string[] = [];

                if (!firstName.trim()) {
                    missingFields.push('firstName');
                }

                if (!lastName.trim()) {
                    missingFields.push('lastName');
                }

                if (!phone.trim()) {
                    missingFields.push('phone');
                }

                setInvalidFields(missingFields);

                if (missingFields.length > 0) {
                    const labels: Record<string, string> = {
                        firstName: t('customer.form.firstNameLower'),
                        lastName: t('customer.form.lastNameAccusative'),
                        phone: t('customer.form.phoneLower'),
                    };

                    toast.error(
                        `${t("customer.form.requiredFieldsPrefix")} ${missingFields
                            .map(field => labels[field])
                            .join(', ')}`
                    );
                    return;
                }
            }
            const passportFields = [
                { key: 'passportSerial', label: t('customer.form.passportSeries'), value: passportSerial },
                { key: 'passportNumber', label: t('customer.form.passportNumber'), value: passportNumber },
                { key: 'passportIssuedBy', label: t('customer.form.passportIssuedBy'), value: passportIssuedBy },
                { key: 'passportIssuedDate', label: t('customer.form.passportIssueDate'), value: passportIssuedDate },
                { key: 'registrationAddress', label: t('customer.form.registrationAddress'), value: registrationAddress },
            ];

            const driverLicenseFields = [
                { key: 'driverLicenseNumber', label: t('customer.form.driverLicenseNumber'), value: driverLicenseNumber },
                { key: 'driverLicenseCategories', label: t('customer.form.driverLicenseCategories'), value: driverLicenseCategories },
                { key: 'driverLicenseIssuedDate', label: t('customer.form.driverLicenseIssueDate'), value: driverLicenseIssuedDate },
                { key: 'driverLicenseValidUntil', label: t('customer.form.driverLicenseValidUntil'), value: driverLicenseValidUntil },
                { key: 'driverLicenseExperienceFrom', label: t('customer.form.experienceFrom'), value: driverLicenseExperienceFrom },
            ];
            const missingPassport = passportStarted
                ? passportFields.filter(field => field.value.trim() === '')
                : [];

            const missingDriverLicense = driverLicenseStarted
                ? driverLicenseFields.filter(field => field.value.trim() === '')
                : [];

            const missingFields = [...missingPassport, ...missingDriverLicense];

            setInvalidFields(missingFields.map(field => field.key));

            if (missingFields.length > 0) {
                if (missingPassport.length > 0) {
                    setShowPassport(true);
                } else {
                    setShowDriverLicense(true);
                }

                const missingLabels = missingFields.map(field => field.label).join(', ');

                toast.error(`${t("customer.form.requiredFieldsPrefix")} ${missingLabels}`);

                const firstField = missingFields[0].key;

                requestAnimationFrame(() => {
                    const element = document.getElementById(`customer-field-${firstField}`);

                    element?.scrollIntoView({
                        behavior: 'smooth',
                        block: 'center',
                    });

                    element?.querySelector<HTMLElement>('input, textarea, select')?.focus({
                        preventScroll: true,
                    });
                });

                return;
            }

            if (isEditMode) {
                const request: UpdateCustomerRequest = {
                    type: CustomerType.Individual,
                    phone,
                    ...(email.trim() && { email: email.trim() }),
                    ...(birthDate && { dateOfBirth: birthDate }),
                    ...(firstName.trim() && { firstName: firstName.trim() }),
                    ...(lastName.trim() && { lastName: lastName.trim() }),
                    ...(middleName.trim() && { middleName: middleName.trim() }),
                    ...(passportSerial.trim() ||
                        passportNumber.trim() ||
                        passportIssuedBy.trim() ||
                        passportIssuedDate ||
                        registrationAddress.trim()
                        ? {
                            passport: {
                                serial: passportSerial,
                                number: passportNumber,
                                issuedBy: passportIssuedBy,
                                issuedDate: passportIssuedDate,
                                registrationAddress,
                            },
                        }
                        : {}),
                    ...(driverLicenseNumber.trim() ||
                        driverLicenseCategories.trim() ||
                        driverLicenseIssuedDate ||
                        driverLicenseValidUntil ||
                        driverLicenseExperienceFrom
                        ? {
                            driverLicense: {
                                number: driverLicenseNumber,
                                categories: driverLicenseCategories
                                    .split(',')
                                    .map(x => x.trim())
                                    .filter(Boolean),
                                issuedDate: driverLicenseIssuedDate || null,
                                expirationDate: driverLicenseValidUntil || null,
                                experienceFrom: driverLicenseExperienceFrom
                                    ? Number(driverLicenseExperienceFrom)
                                    : 0,
                            },
                        }
                        : {}),
                };
                await onUpdate?.(request);
                toast(t("customer.form.saved"));
            }
            else
            {
                const request: CreateCustomerRequest = {
                    type: CustomerType.Individual,
                    email: email,
                    phone: phone,
                    firstName: firstName,
                    lastName: lastName,
                    middleName: middleName,          
                    dateOfBirth: birthDate || null,
                    passport: {
                        serial: passportSerial,
                        number: passportNumber,
                        issuedBy: passportIssuedBy,
                        issuedDate: passportIssuedDate || null,
                        registrationAddress: registrationAddress
                    },
                    driverLicense: {
                        number: driverLicenseNumber,
                        categories: driverLicenseCategories.split(',').map(x => x.trim()).filter(Boolean),
                        issuedDate: driverLicenseIssuedDate || null,
                        expirationDate: driverLicenseValidUntil || null,
                        experienceFrom: driverLicenseExperienceFrom
                            ? Number(driverLicenseExperienceFrom)
                            : 0
                    }
                }

                await onCreate?.(request);
            }
    };

    return (
        <div className="space-y-4">
            {/* Личные данные */}
            <FormSection
                title={t("customer.form.personalData")}
                icon={User}
                open={showPersonalInfo}
                onToggle={() => setShowPersonalInfo(prev => !prev)}
            >
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div id="customer-field-lastName">
                        <CustomerFormField label={t("customer.form.lastName")}>
                            <input
                                className={`${customerInputClass} ${
                                    invalidFields.includes('lastName')
                                        ? 'border-red-500 ring-1 ring-red-500'
                                        : ''
                                }`}
                                placeholder={t("customer.form.lastName")}
                                value={lastName}
                                readOnly={readOnly}
                                onChange={e => {
                                    setLastName(e.target.value);
                                    setInvalidFields(prev =>
                                        prev.filter(field => field !== 'lastName')
                                    );
                                }}
                            />
                        </CustomerFormField>
                    </div>
                        <div id="customer-field-firstName">
                            <CustomerFormField label={t("customer.form.firstName")}>
                                <input
                                    className={`${customerInputClass} ${
                                        invalidFields.includes('firstName')
                                            ? 'border-red-500 ring-1 ring-red-500'
                                            : ''
                                    }`}
                                    placeholder={t("customer.form.firstName")}
                                    value={firstName}
                                    readOnly={readOnly}
                                    onChange={e => {
                                        setFirstName(e.target.value);
                                        setInvalidFields(prev =>
                                            prev.filter(field => field !== 'firstName')
                                        );
                                    }}
                                />
                            </CustomerFormField>
                        </div>

                    <CustomerFormField label={t("customer.form.middleName")}>
                        <input
                            className={customerInputClass}
                            placeholder={t("customer.form.middleName")}
                            value={middleName}
                            readOnly={readOnly}
                            onChange={e => setMiddleName(e.target.value)}
                        />
                    </CustomerFormField>

                    <CustomerFormField label={t("customer.form.birthDate")}>
                        <div className="relative">
                            <DateInput
                                value={birthDate}
                                onChange={setBirthDate}
                                maxDate={new Date()}
                                className={customerInputClass}
                            />
                        </div>
                    </CustomerFormField>

                    <div id="customer-field-phone">
                        <CustomerFormField label={t("common.phone")}>
                            <PhoneInput
                                className={`${customerInputClass} ${
                                    invalidFields.includes('phone')
                                        ? 'border-red-500 ring-1 ring-red-500'
                                        : ''
                                }`}
                                placeholder="+7 (___) ___-__-__"
                                value={phone}
                                readOnly={readOnly}
                                onChange={value => {
                                    setPhone(value);
                                    setInvalidFields(prev =>
                                        prev.filter(field => field !== 'phone')
                                    );
                                }}
                            />
                        </CustomerFormField>
                    </div>

                    <CustomerFormField label="Email">
                        <EmailInput
                            className={customerInputClass}
                            placeholder="example@mail.ru"
                            value={email}
                            readOnly={readOnly}
                            onChange={setEmail}
                        />
                    </CustomerFormField>
                </div>
            </FormSection>


            {/* Паспорт */}
            <FormSection
                title={t("customer.form.passport")}
                description={t("customer.form.passportDescription")}
                icon={SquareUser}
                open={showPassport}
                onToggle={() => setShowPassport(prev => !prev)}
            >
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div id="customer-field-passportSerial">
                        <CustomerFormField label={`${t("customer.form.series")}${passportStarted ? " *" : ""}`}>
                            <DigitsInput
                                className={customerInputClass}
                                placeholder="0000"
                                value={passportSerial}
                                readOnly={readOnly}
                                maxLength={4}
                                invalid={(passportSerial.length > 0 && passportSerial.length < 4) || invalidFields.includes('passportSerial')}
                                onChange={(value) => {
                                    setPassportSerial(value);
                                    setInvalidFields(prev => prev.filter(field => field !== 'passportSerial'));
                                }}
                            />
                        </CustomerFormField>
                    </div>
                    
                    <div id="customer-field-passportNumber">
                        <CustomerFormField label={`${t("customer.form.number")}${passportStarted ? " *" : ""}`}>
                            <DigitsInput
                                className={customerInputClass}
                                placeholder="000000"
                                value={passportNumber}
                                readOnly={readOnly}
                                maxLength={6}
                                invalid={(passportNumber.length > 0 && passportNumber.length < 6) || invalidFields.includes('passportNumber')} 
                                onChange={(value) => {
                                    setPassportNumber(value);
                                    setInvalidFields(prev => prev.filter(field => field !== 'passportNumber'));
                                }}
                            />
                        </CustomerFormField>
                    </div>
                    
                    <div id="customer-field-passportIssuedBy">
                        <CustomerFormField
                            label={`${t("customer.form.issuedBy")}${passportStarted ? " *" : ""}`}
                            className="md:col-span-2"
                        >
                            <input
                                className={`${customerInputClass} ${
                                    invalidFields.includes('passportIssuedBy')
                                        ? 'border-red-500 ring-1 ring-red-500'
                                        : ''
                                }`}
                                placeholder={t("customer.form.issuingAuthorityPlaceholder")}
                                value={passportIssuedBy}
                                readOnly={readOnly}
                                onChange={e => {
                                    setPassportIssuedBy(e.target.value);
                                    setInvalidFields(prev =>
                                        prev.filter(field => field !== 'passportIssuedBy')
                                    );
                                }}
                            />
                        </CustomerFormField>
                    </div>
                    
                    <div id="customer-field-passportIssuedDate">
                        <CustomerFormField
                            label={`${t("customer.form.issueDate")}${passportStarted ? " *" : ""}`}
                        >
                            <DateInput
                                className={customerInputClass}
                                value={passportIssuedDate}
                                readOnly={readOnly}
                                invalid={invalidFields.includes('passportIssuedDate')}
                                onChange={value => {
                                    setPassportIssuedDate(value);
                                    setInvalidFields(prev =>
                                        prev.filter(field => field !== 'passportIssuedDate')
                                    );
                                }}
                            />
                        </CustomerFormField>
                    </div>
                   
                    <div id="customer-field-registrationAddress">
                        <CustomerFormField
                            label={`${t("customer.form.registrationAddress")}${passportStarted ? " *" : ""}`}
                            className="md:col-span-2"
                        >
                            <textarea
                                className={`${customerInputClass} resize-none ${
                                    invalidFields.includes('registrationAddress')
                                        ? 'border-red-500 ring-1 ring-red-500'
                                        : ''
                                }`}
                                rows={3}
                                placeholder={t("customer.form.registrationAddressPlaceholder")}
                                value={registrationAddress}
                                readOnly={readOnly}
                                onChange={e => {
                                    setRegistrationAddress(e.target.value);
                                    setInvalidFields(prev =>
                                        prev.filter(field => field !== 'registrationAddress')
                                    );
                                }}
                            />
                        </CustomerFormField>
                    </div>
                </div>
            </FormSection>

            {/* Водительское удостоверение */}
            <FormSection
                title={t("customer.form.driverLicense")}
                description={t("customer.form.driverLicenseDescription")}
                icon={Car}
                open={showDriverLicense}
                onToggle={() => setShowDriverLicense(prev => !prev)}
            >
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div id="customer-field-driverLicenseNumber">
                        <CustomerFormField label={`${t("customer.form.driverLicenseNumber")}${driverLicenseStarted ? " *" : ""}`}>
                            <DriverLicenseInput
                                className={customerInputClass}
                                placeholder="00 00 000000"
                                value={driverLicenseNumber}
                                readOnly={readOnly}
                                invalid={(driverLicenseNumber.length > 0 && driverLicenseNumber.length < 10) || invalidFields.includes('driverLicenseNumber')}
                                onChange={value => {
                                    setDriverLicenseNumber(value);
                                    setInvalidFields(prev =>
                                        prev.filter(field => field !== 'driverLicenseNumber')
                                    );
                                }}
                            />
                        </CustomerFormField>
                    </div>
                    
                    <div id="customer-field-driverLicenseCategories">
                        <CustomerFormField label={`${t("customer.form.categories")}${driverLicenseStarted ? " *" : ""}`}>
                            <DriverLicenseCategoriesInput
                                className={customerInputClass}
                                placeholder="B, BE"
                                value={driverLicenseCategories}
                                readOnly={readOnly}
                                invalid={invalidFields.includes('driverLicenseCategories')}
                                onChange={value => {
                                    setDriverLicenseCategories(value);
                                    setInvalidFields(prev =>
                                        prev.filter(field => field !== 'driverLicenseCategories')
                                    );
                                }}
                            />
                        </CustomerFormField>
                    </div>
                    
                    <div id="customer-field-driverLicenseIssuedDate">
                        <CustomerFormField label={`${t("customer.form.issueDate")}${driverLicenseStarted ? " *" : ""}`}>
                            <DateInput
                                className={customerInputClass}
                                value={driverLicenseIssuedDate}
                                readOnly={readOnly}
                                invalid={invalidFields.includes('driverLicenseIssuedDate')}
                                onChange={value => {
                                    setDriverLicenseIssuedDate(value);
                                    setInvalidFields(prev =>
                                        prev.filter(field => field !== 'driverLicenseIssuedDate')
                                    );
                                }}
                            />
                        </CustomerFormField>
                    </div>
                    <div id="customer-field-driverLicenseValidUntil">
                        <CustomerFormField label={`${t("customer.form.validUntil")}${driverLicenseStarted ? " *" : ""}`}>
                            <DateInput
                                className={customerInputClass}
                                value={driverLicenseValidUntil}
                                readOnly={readOnly}
                                invalid={invalidFields.includes('driverLicenseValidUntil')}
                                onChange={value => {
                                    setDriverLicenseValidUntil(value);
                                    setInvalidFields(prev =>
                                        prev.filter(field => field !== 'driverLicenseValidUntil')
                                    );
                                }}
                            />
                        </CustomerFormField>
                    </div>
                    
                    <div id="customer-field-driverLicenseExperienceFrom">
                        <CustomerFormField label={`${t("customer.form.experienceFrom")}${driverLicenseStarted ? " *" : ""}`}>
                            <DigitsInput
                                value={driverLicenseExperienceFrom}
                                onChange={(value) => {
                                    const year = Number(value);
                                    if (value && year > new Date().getFullYear()) {
                                        return;
                                    }
                                    setDriverLicenseExperienceFrom(value);
                                    setInvalidFields(prev => prev.filter(field => field !== 'driverLicenseExperienceFrom'));
                                }}
                                invalid={invalidFields.includes('driverLicenseExperienceFrom')}
                                maxLength={4}
                            />
                        </CustomerFormField>
                    </div>
                </div>
            </FormSection>


            {/* Кнопка */}
            {!readOnly && (
                <div className="flex justify-end pt-2">
                    <button
                        type="button"
                        onClick={handleSave}
                        className="inline-flex items-center gap-2 px-5 py-2.5 rounded-lg bg-primary-600 text-white font-medium shadow-sm hover:bg-primary-700 hover:shadow-md transition-all disabled:opacity-50"
                    >
                        <Save className="w-4 h-4" />

                        {isEditMode
                            ? t('common.saveChanges')
                            : t('customer.create.submit')}
                    </button>
                </div>
            )}
        </div>
    );
};