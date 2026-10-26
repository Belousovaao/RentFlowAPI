import { useState } from 'react';
import { CustomerDetailsDto, CustomerType, UpdateCustomerRequest, type CreateCustomerRequest } from '@/entities/customer/customer.type';
import { Building2, FileText, Landmark, Save } from 'lucide-react';
import { FormSection } from './FormSection';
import { CustomerFormField } from './CustomerFormField';
import { customerInputClass } from '@/entities/customer/customer.formStyles';
import { DigitsInput } from '@/features/ui/components/DigitsInput';
import { PhoneInput } from '@/features/ui/components/PhoneInput';
import { EmailInput } from '@/features/ui/components/EmailInput';

interface OrganizationFormProps {
    customer?: CustomerDetailsDto;
    readOnly?: boolean;
    onCreate?: (data: CreateCustomerRequest) => Promise<void>;
    onUpdate?: (data: UpdateCustomerRequest) => Promise<void>;
}

export const OrganizationForm = ({
    customer,
    readOnly,
    onCreate,
    onUpdate }: OrganizationFormProps) => {
        const isEditMode = !!customer;
        const [showOrganizationInfo, setShowOrganizationInfo] = useState(true);
        const [showOrganizationDetails, setShowOrganizationDetails] = useState(false);
        const [showBankInfo, setShowBankInfo] = useState(false);

        const [shortName, setShortName] = useState(customer?.shortName || '');
        const [fullName, setFullName] = useState(customer?.fullName || '');

        const [phone, setPhone] = useState(customer?.phone || '');
        const [email, setEmail] = useState(customer?.email || '');

        const [legalForm, setLegalForm] = useState(customer?.legalForm || '');
        const [kpp, setKpp] = useState(customer?.kpp || '');
        const [inn, setInn] = useState(customer?.inn || '');
        const [ogrn, setOgrn] = useState(customer?.ogrn || '');

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
                    type: CustomerType.Organization,
                    email: email,
                    phone: phone,
                    shortName: shortName,
                    fullName: fullName,
                    inn: inn,
                    ogrn: ogrn,
                    kpp: kpp,
                    legalForm: legalForm,
                    organizationAddress: organizationAddress,
                    factAddress: factAddress,
                    bankInfo: {
                        currentAccount,
                        correspondentAccount,
                        bik,
                        bankName
                    },
                };

                await onUpdate?.(request);
            }
            else
            {
                const request: CreateCustomerRequest = {
                    type: CustomerType.Organization,
                    email: email,
                    phone: phone,
                    shortName: shortName,
                    fullName: fullName,                   
                    inn: inn,
                    ogrn: ogrn,
                    kpp: kpp,
                    legalForm: legalForm,
                    organizationAddress: organizationAddress,
                    factAddress: factAddress,
                    bankInfo: {
                        currentAccount,
                        correspondentAccount,
                        bik,
                        bankName
                    },
                };

                await onCreate?.(request);
            }
        };

        return (
            <div className="space-y-4">

                <FormSection
                    title="Данные организации"
                    icon={Building2}
                    open={showOrganizationInfo}
                    onToggle={() =>
                        setShowOrganizationInfo(prev => !prev)
                    }
                >
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">

                        <CustomerFormField label="ОПФ">
                            <input
                                className={customerInputClass}
                                placeholder="ООО, АО и т.д."
                                value={legalForm}
                                readOnly={readOnly}
                                onChange={e => setLegalForm(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField label="Краткое наименование (без ОПФ)">
                            <input
                                className={customerInputClass}
                                placeholder="Название организации"
                                value={shortName}
                                readOnly={readOnly}
                                onChange={e => setShortName(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField
                            label="Полное наименование (без ОПФ)"
                            className="md:col-span-2"
                        >
                            <input
                                className={customerInputClass}
                                placeholder="Полное наименование"
                                value={fullName}
                                readOnly={readOnly}
                                onChange={e => setFullName(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField label="Телефон">
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
                    title="Реквизиты"
                    icon={FileText}
                    open={showOrganizationDetails}
                    onToggle={() =>
                        setShowOrganizationDetails(prev => !prev)
                    }
                >
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">

                        <CustomerFormField label="ИНН">
                            <DigitsInput
                                className={customerInputClass}
                                placeholder="10 цифр"
                                value={inn}
                                readOnly={readOnly}
                                maxLength={10}
                                invalid={inn.length > 0 && inn.length < 10}
                                onChange={setInn}
                            />
                        </CustomerFormField>

                        <CustomerFormField label="ОГРН">
                            <DigitsInput
                                className={customerInputClass}
                                placeholder="13 цифр"
                                value={ogrn}
                                readOnly={readOnly}
                                maxLength={13}
                                invalid={ogrn.length > 0 && ogrn.length < 13}
                                onChange={setOgrn}
                            />
                        </CustomerFormField>

                        <CustomerFormField label="КПП">
                            <DigitsInput
                                className={customerInputClass}
                                placeholder="9 цифр"
                                value={kpp}
                                readOnly={readOnly}
                                maxLength={9}
                                invalid={kpp.length > 0 && kpp.length < 9}
                                onChange={setKpp}
                            />
                        </CustomerFormField>

                        <div />

                        <CustomerFormField
                            label="Юридический адрес"
                            className="md:col-span-2"
                        >
                            <textarea
                                className={`${customerInputClass} resize-none`}
                                rows={2}
                                placeholder="Юридический адрес"
                                value={organizationAddress}
                                readOnly={readOnly}
                                onChange={e => setOrganizationAddress(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField
                            label="Фактический адрес"
                            className="md:col-span-2"
                        >
                            <textarea
                                className={`${customerInputClass} resize-none`}
                                rows={2}
                                placeholder="Фактический адрес"
                                value={factAddress}
                                readOnly={readOnly}
                                onChange={e => setFactAddress(e.target.value)}
                            />
                        </CustomerFormField>

                    </div>
                </FormSection>


                <FormSection
                    title="Банковские реквизиты"
                    icon={Landmark}
                    open={showBankInfo}
                    onToggle={() => setShowBankInfo(prev => !prev)}
                >
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">

                        <CustomerFormField label="Расчетный счет">
                            <input
                                className={customerInputClass}
                                value={currentAccount}
                                readOnly={readOnly}
                                onChange={e => setCurrentAccount(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField label="Корреспондентский счет">
                            <input
                                className={customerInputClass}
                                value={correspondentAccount}
                                readOnly={readOnly}
                                onChange={e =>
                                    setCorrespondentAccount(e.target.value)
                                }
                            />
                        </CustomerFormField>

                        <CustomerFormField label="БИК">
                            <input
                                className={customerInputClass}
                                value={bik}
                                readOnly={readOnly}
                                onChange={e => setBik(e.target.value)}
                            />
                        </CustomerFormField>

                        <CustomerFormField label="Банк">
                            <input
                                className={customerInputClass}
                                placeholder="Наименование банка"
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
                                ? 'Сохранить изменения'
                                : 'Создать клиента'}
                        </button>
                    </div>
                )}
            </div>
        );
};