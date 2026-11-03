import { useState } from 'react';
import { useI18n } from '@/app/providers/I18nProvider';
import { CustomerDetailsDto, CustomerType, UpdateCustomerRequest, type CreateCustomerRequest } from '@/entities/customer/customer.type';
import { Briefcase, Car, Landmark, Save, SquareUser, User } from 'lucide-react';
import { FormSection } from './FormSection';
import { CustomerFormField } from './CustomerFormField';
import { customerInputClass } from '@/entities/customer/customer.formStyles';
import { DigitsInput } from '@/features/ui/components/DigitsInput';
import { PhoneInput } from '@/features/ui/components/PhoneInput';
import { EmailInput } from '@/features/ui/components/EmailInput';
import { DriverLicenseInput } from '@/features/ui/components/DriverLicenseInput';
import { DriverLicenseCategoriesInput } from '@/features/ui/components/DriverLicenseCategoriesInput';

interface EntrepreneurFormProps {
    customer?: CustomerDetailsDto;
    readOnly?: boolean;
    onCreate?: (data: CreateCustomerRequest) => Promise<void>;
    onUpdate?: (data: UpdateCustomerRequest) => Promise<void>;
}

export const EntrepreneurForm = ({
    customer,
    readOnly,
    onCreate,
    onUpdate 
}: EntrepreneurFormProps) => {
        const { t } = useI18n();
        const isEditMode = !!customer;

        const [showPersonalInfo, setShowPersonalInfo] = useState(true);
        const [showPassport, setShowPassport] = useState(false);
        const [showDriverLicense, setShowDriverLicense] = useState(false);
        const [showEntrepreneurInfo, setShowEntrepreneurInfo] = useState(false);
        const [showBankInfo, setShowBankInfo] = useState(false);

        const [firstName, setFirstName] = useState(customer?.firstName || '');
        const [lastName, setLastName] = useState(customer?.lastName || '');
        const [middleName, setMiddleName] = useState(customer?.middleName || '');

        const [phone, setPhone] = useState(customer?.phone || '');
        const [email, setEmail] = useState(customer?.email || '');

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

        const [inn, setInn] = useState(customer?.inn || '');
        const [ogrnip, setOgrnip] = useState(customer?.ogrn || '');
        const [legalForm ] = useState(customer?.legalForm || '');
        const [organizationAddress, setOrganizationAddress] = useState(customer?.organizationAddress || '');
        const [factAddress, setFactAddress] = useState(customer?.factAddress || '');

        const [currentAccount, setCurrentAccount] = useState(customer?.bankInfo?.currentAccount || '');
        const [correspondentAccount, setCorrespondentAccount] = useState(customer?.bankInfo?.correspondentAccount || '');
        const [bik, setBik] = useState(customer?.bankInfo?.bik || '');
        const [bankName, setBankName] = useState(customer?.bankInfo?.bankName || '');

        const handleSave = async () => {
            if (isEditMode)
            {
                const request: UpdateCustomerRequest = {
                    type: CustomerType.IndividualEntrepreneur,
                    email: email,
                    phone: phone,
                    firstName: firstName,
                    lastName: lastName,
                    middleName: middleName,
                    passport: {
                        serial: passportSerial,
                        number: passportNumber,
                        issuedBy: passportIssuedBy,
                        issuedDate: passportIssuedDate,
                        registrationAddress: registrationAddress
                    },
                    inn: inn,
                    ogrnip: ogrnip,
                    legalForm: legalForm,
                    organizationAddress: organizationAddress,
                    factAddress: factAddress,
                    bankInfo: {
                        currentAccount,
                        correspondentAccount,
                        bik,
                        bankName
                    },
                    driverLicense: {
                        number: driverLicenseNumber,
                        categories: driverLicenseCategories.split(',').map(x => x.trim()).filter(Boolean),
                        issuedDate: driverLicenseIssuedDate,
                        expirationDate: driverLicenseValidUntil,
                        experienceFrom: driverLicenseExperienceFrom
                            ? Number(driverLicenseExperienceFrom)
                            : 0
                    }
                }

                await onUpdate?.(request);
            }
            else
            {
                const request: CreateCustomerRequest = {
                    type: CustomerType.IndividualEntrepreneur,
                    email: email,
                    phone: phone,
                    firstName: firstName,
                    lastName: lastName,
                    middleName: middleName,
                    passport: {
                        serial: passportSerial,
                        number: passportNumber,
                        issuedBy: passportIssuedBy,
                        issuedDate: passportIssuedDate,
                        registrationAddress: registrationAddress
                    },
                    inn: inn,
                    ogrnip: ogrnip,
                    legalForm: legalForm,
                    organizationAddress: organizationAddress,
                    factAddress: factAddress,
                    bankInfo: {
                        currentAccount,
                        correspondentAccount,
                        bik,
                        bankName
                    },
                    driverLicense: {
                        number: driverLicenseNumber,
                        categories: driverLicenseCategories.split(',').map(x => x.trim()).filter(Boolean),
                        issuedDate: driverLicenseIssuedDate,
                        expirationDate: driverLicenseValidUntil,
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

                <FormSection
                    title={t("customer.form.personalData")}
                    icon={User}
                    open={showPersonalInfo}
                    onToggle={() => setShowPersonalInfo(prev => !prev)}
                >
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">

                        <CustomerFormField label={t("customer.form.lastName")}>
                            <input
                                className={customerInputClass}
                                placeholder={t("customer.form.lastName")}
                                value={lastName}
                                readOnly={readOnly}
                                onChange={e => setLastName(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField label={t("customer.form.firstName")}>
                            <input
                                className={customerInputClass}
                                placeholder={t("customer.form.firstName")}
                                value={firstName}
                                readOnly={readOnly}
                                onChange={e => setFirstName(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField label={t("customer.form.middleName")}>
                            <input
                                className={customerInputClass}
                                placeholder={t("customer.form.middleName")}
                                value={middleName}
                                readOnly={readOnly}
                                onChange={e => setMiddleName(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField label={t("common.phone")}>
                            <PhoneInput
                                className={customerInputClass}
                                placeholder="+7 (___) ___-__-__"
                                value={phone}
                                readOnly={readOnly}
                                onChange={setPhone}
                            />
                        </CustomerFormField>

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


                <FormSection
                    title={t("customer.form.passport")}
                    icon={SquareUser}
                    open={showPassport}
                    onToggle={() => setShowPassport(prev => !prev)}
                >
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">

                        <CustomerFormField label={t("customer.form.series")}>
                        <DigitsInput
                            className={customerInputClass}
                            placeholder="0000"
                            value={passportSerial}
                            readOnly={readOnly}
                            maxLength={4}
                            invalid={passportSerial.length > 0 && passportSerial.length < 4}
                            onChange={setPassportSerial}
                        />
                    </CustomerFormField>

                    <CustomerFormField label={t("customer.form.number")}>
                        <DigitsInput
                            className={customerInputClass}
                            placeholder="000000"
                            value={passportNumber}
                            readOnly={readOnly}
                            maxLength={6}
                            invalid={passportNumber.length > 0 && passportNumber.length < 6}
                            onChange={setPassportNumber}
                        />
                    </CustomerFormField>

                        <CustomerFormField
                            label={t("customer.form.issuedBy")}
                            className="md:col-span-2"
                        >
                            <input
                                className={customerInputClass}
                                placeholder={t("customer.form.issuingAuthorityPlaceholder")}
                                value={passportIssuedBy}
                                readOnly={readOnly}
                                onChange={e => setPassportIssuedBy(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField label={t("customer.form.issueDate")}>
                            <input
                                type="date"
                                className={customerInputClass}
                                value={passportIssuedDate}
                                readOnly={readOnly}
                                onChange={e => setPassportIssuedDate(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField
                            label={t("customer.form.registrationAddress")}
                            className="md:col-span-2"
                        >
                            <textarea
                                className={`${customerInputClass} resize-none`}
                                rows={3}
                                placeholder={t("customer.form.registrationAddress")}
                                value={registrationAddress}
                                readOnly={readOnly}
                                onChange={e => setRegistrationAddress(e.target.value)}
                            />
                        </CustomerFormField>
                    </div>
                </FormSection>


                <FormSection
                    title={t("customer.form.driverLicense")}
                    icon={Car}
                    open={showDriverLicense}
                    onToggle={() => setShowDriverLicense(prev => !prev)}
                >
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">

                        <CustomerFormField label={t("customer.form.driverLicenseNumber")}>
                            <DriverLicenseInput
                                className={customerInputClass}
                                placeholder="00 00 000000"
                                value={driverLicenseNumber}
                                readOnly={readOnly}
                                invalid={driverLicenseNumber.length > 0 && driverLicenseNumber.length < 10}
                                onChange={setDriverLicenseNumber}
                            />
                        </CustomerFormField>

                        <CustomerFormField label={t("customer.form.categories")}>
                            <DriverLicenseCategoriesInput
                                className={customerInputClass}
                                placeholder="B, BE"
                                value={driverLicenseCategories}
                                readOnly={readOnly}
                                onChange={setDriverLicenseCategories}
                            />
                        </CustomerFormField>

                        <CustomerFormField label={t("customer.form.issueDate")}>
                            <input
                                type="date"
                                className={customerInputClass}
                                value={driverLicenseIssuedDate}
                                readOnly={readOnly}
                                onChange={e => setDriverLicenseIssuedDate(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField label={t("customer.form.validUntil")}>
                            <input
                                type="date"
                                className={customerInputClass}
                                value={driverLicenseValidUntil}
                                readOnly={readOnly}
                                onChange={e => setDriverLicenseValidUntil(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField label={t("customer.form.experienceFrom")}>
                            <DigitsInput
                                value={driverLicenseExperienceFrom}
                                onChange={(value) => {
                                    const year = Number(value);
                                    if (value && year > new Date().getFullYear()) {
                                        return;
                                    }

                                    setDriverLicenseExperienceFrom(value);
                                }}
                                maxLength={4}
                            />
                        </CustomerFormField>

                    </div>
                </FormSection>


                <FormSection
                    title={t("customer.form.entrepreneurData")}
                    icon={Briefcase}
                    open={showEntrepreneurInfo}
                    onToggle={() => setShowEntrepreneurInfo(prev => !prev)}
                >
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">

                        <CustomerFormField label={t("customer.form.inn")}>
                            <DigitsInput
                                className={customerInputClass}
                                placeholder={t("customer.form.twelveDigits")}
                                value={inn}
                                readOnly={readOnly}
                                maxLength={12}
                                invalid={inn.length > 0 && inn.length < 12}
                                onChange={setInn}
                            />
                        </CustomerFormField>

                        <CustomerFormField label={t("customer.form.ogrnip")}>
                            <DigitsInput
                                className={customerInputClass}
                                placeholder={t("customer.form.fifteenDigits")}
                                value={ogrnip}
                                readOnly={readOnly}
                                maxLength={15}
                                invalid={ogrnip.length > 0 && ogrnip.length < 15}
                                onChange={setOgrnip}
                            />
                        </CustomerFormField>

                        <CustomerFormField
                            label={t("customer.form.legalAddress")}
                            className="md:col-span-2"
                        >
                            <textarea
                                className={`${customerInputClass} resize-none`}
                                rows={2}
                                placeholder={t("customer.form.enterAddress")}
                                value={organizationAddress}
                                readOnly={readOnly}
                                onChange={e => setOrganizationAddress(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField
                            label={t("customer.form.actualAddress")}
                            className="md:col-span-2"
                        >
                            <textarea
                                className={`${customerInputClass} resize-none`}
                                rows={2}
                                placeholder={t("customer.form.enterAddress")}
                                value={factAddress}
                                readOnly={readOnly}
                                onChange={e => setFactAddress(e.target.value)}
                            />
                        </CustomerFormField>

                    </div>
                </FormSection>


                <FormSection
                    title={t("customer.form.bankDetails")}
                    icon={Landmark}
                    open={showBankInfo}
                    onToggle={() => setShowBankInfo(prev => !prev)}
                >
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">

                        <CustomerFormField label={t("customer.form.bankAccount")}>
                            <input
                                className={customerInputClass}
                                value={currentAccount}
                                readOnly={readOnly}
                                onChange={e => setCurrentAccount(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField label={t("customer.form.correspondentAccount")}>
                            <input
                                className={customerInputClass}
                                value={correspondentAccount}
                                readOnly={readOnly}
                                onChange={e => setCorrespondentAccount(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField label={t("customer.form.bik")}>
                            <input
                                className={customerInputClass}
                                value={bik}
                                readOnly={readOnly}
                                onChange={e => setBik(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField label={t("customer.form.bank")}>
                            <input
                                className={customerInputClass}
                                placeholder={t("customer.form.bankNamePlaceholder")}
                                value={bankName}
                                readOnly={readOnly}
                                onChange={e => setBankName(e.target.value)}
                            />
                        </CustomerFormField>

                    </div>
                </FormSection>


                {!readOnly && (
                    <div className="flex justify-end pt-2">
                        <button
                            type="button"
                            onClick={handleSave}
                            className="inline-flex items-center gap-2 px-5 py-2.5 rounded-lg bg-primary-600 text-white font-medium shadow-sm hover:bg-primary-700 hover:shadow-md transition-all"
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