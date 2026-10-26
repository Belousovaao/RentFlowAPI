import { useMemo, useState } from "react";
import { Check, FileCheck2, SquareUserRound, ShieldCheck, ShieldAlert } from "lucide-react";
import toast from "react-hot-toast";
import { customerApi } from "@/features/customer/api/customerApi";
import { DocumentType } from "@/entities/customer/customer.type";
import { Button } from "@/features/ui/components/Button";
import { DocumentUploadField } from "./DocumentUploadField";

interface Props {
    open: boolean;
    onClose: () => void;
    onSuccess: () => Promise<void>;
}

export const UploadDocumentsModal = ({
    open,
    onClose,
    onSuccess
}: Props) => {
    const [loading, setLoading] = useState(false);
    const [passportFront, setPassportFront] = useState<File | null>(null);
    const [passportRegistration, setPassportRegistration] = useState<File | null>(null);
    const [driverFront, setDriverFront] = useState<File | null>(null);
    const [driverBack, setDriverBack] = useState<File | null>(null);
    const files = useMemo(
        () => [passportFront, passportRegistration, driverFront, driverBack],
        [passportFront, passportRegistration, driverFront, driverBack]
    );

    const uploadedCount = files.filter(Boolean).length;
    const progress = (uploadedCount / 4) * 100;
    const passportCount = Number(!!passportFront) + Number(!!passportRegistration);
    const driverLicenseCount = Number(!!driverFront) + Number(!!driverBack);
    const passportComplete = passportCount === 2;
    const driverLicenseComplete = driverLicenseCount === 2;
    const allDocumentsComplete = passportComplete && driverLicenseComplete;
    const currentStep = allDocumentsComplete ? 2 : passportComplete ? 1 : 0;

    if (!open) {
        return null;
    }

    const upload = async () => {
        if (uploadedCount !== 4) {
            toast.error("Загрузите все четыре документа перед отправкой");
            return;
        }

        if (
            !passportFront ||
            !passportRegistration ||
            !driverFront ||
            !driverBack
        ) {
            return;
        }

        setLoading(true);

        try {
            await customerApi.uploadDocuments(
                DocumentType.PassportFront,
                passportFront
            );

            await customerApi.uploadDocuments(
                DocumentType.PassportRegistration,
                passportRegistration
            );

            await customerApi.uploadDocuments(
                DocumentType.DriverLicenseFront,
                driverFront
            );

            await customerApi.uploadDocuments(
                DocumentType.DriverLicenseBack,
                driverBack
            );

            toast.success("Документы отправлены на проверку");
            onClose();
            await onSuccess();
        } catch (err: any) {
            console.error("UPLOAD DOCUMENTS ERROR");
            console.error("status:", err?.response?.status);
            console.error("data:", err?.response?.data);
            console.error("message:", err?.message);
            toast.error("Не удалось загрузить документы. Попробуйте ещё раз.");
        } finally {
            setLoading(false);
        }
    };

    const handleClose = () => {
        if (loading) {
            return;
        }

        onClose();
    };

    const steps = [
        {
            title: "Паспорт",
            icon: SquareUserRound,
        },
        {
            title: "Водительское",
            icon: ShieldCheck,
        },
        {
            title: "Проверка",
            icon: FileCheck2,
        },
    ];

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 p-4 backdrop-blur-sm">
            <div className="flex max-h-[90vh] w-full max-w-2xl flex-col overflow-hidden rounded-2xl bg-white shadow-2xl dark:bg-gray-900">

                {/* Header */}
                <div className="border-b border-gray-100 px-6 py-5 dark:border-gray-800">
                    <div className="flex items-start justify-between gap-4">
                        <div>
                            <div className="flex items-center gap-2">
                                <div className="flex h-9 w-9 items-center justify-center rounded-xl bg-primary-50 text-primary-600 dark:bg-primary-950/40">
                                    <ShieldAlert className="h-5 w-5" />
                                </div>

                                <h2 className="text-xl font-semibold text-gray-900 dark:text-white">
                                    Проверка документов
                                </h2>
                            </div>

                            <p className="mt-2 text-sm text-gray-500">
                                Для завершения регистрации, необходимо загрузить паспорт и водительское удостоверение. После отправки менеджер проверит документы и свяжется с вами.
                            </p>
                        </div>
                    </div>

                    {/* Steps */}
                    <div className="mt-6">
                        <div className="flex items-center">
                            {steps.map((step, index) => {
                                const Icon = step.icon;

                                const completed =
                                    index < currentStep ||
                                    (index === 3 &&
                                        uploadedCount === 4);

                                const active =
                                    index === currentStep;

                                return (
                                    <div
                                        key={step.title}
                                        className="flex flex-1 items-center"
                                    >
                                        <div className="flex flex-col items-center">
                                            <div
                                                className={`flex h-9 w-9 items-center justify-center rounded-full border-2transition-all
                                                    ${
                                                        completed
                                                            ? "border-primary-600 bg-primary-600 text-white"
                                                            : active
                                                                ? "border-primary-600 bg-primary-50 text-primary-600 dark:bg-primary-950/30"
                                                                : "border-gray-200 bg-white text-gray-400 dark:border-gray-700 dark:bg-gray-900"
                                                    }
                                                `}
                                            >
                                                {completed ? (
                                                    <Check className="h-4 w-4" />
                                                ) : (
                                                    <Icon className="h-4 w-4" />
                                                )}
                                            </div>

                                            <span
                                                className={`mt-2 whitespace-nowrap text-xs
                                                    ${
                                                        active ||
                                                        completed
                                                            ? "font-medium text-gray-800 dark:text-gray-200"
                                                            : "text-gray-400"
                                                    }
                                                `}
                                            >
                                                {step.title}
                                            </span>
                                        </div>

                                        {index < steps.length - 1 && (
                                            <div className="mx-2 mb-5 h-0.5 flex-1 bg-gray-200 dark:bg-gray-700">
                                                <div
                                                    className="h-full bg-primary-600 transition-all duration-300"
                                                    style={{width: index < currentStep ? "100%" : "0%"}}
                                                />
                                            </div>
                                        )}
                                    </div>
                                );
                            })}
                        </div>
                    </div>
                </div>

                {/* Content */}
                <div className="flex-1 overflow-y-auto px-6 py-6">
                    {/* Progress */}
                    <div className="mb-6">
                        <div className="mb-2 flex items-center justify-between">
                            <span className="text-sm font-medium text-gray-700 dark:text-gray-200">
                                Документы загружены
                            </span>

                            <span className="text-sm font-semibold text-primary-600">
                                {uploadedCount} из 4
                            </span>
                        </div>

                        <div className="h-2 overflow-hidden rounded-full bg-gray-100 dark:bg-gray-800">
                            <div
                                className="h-full rounded-full bg-primary-600 transition-all duration-300"
                                style={{
                                    width: `${progress}%`,
                                }}
                            />
                        </div>
                    </div>

                    <div className="space-y-5">
                        {/* Passport */}
                        <div className="rounded-2xl border border-gray-100 bg-gray-50/70 p-4 dark:border-gray-800 dark:bg-gray-800/30">
                            <div className="mb-4">
                                <h3 className="font-semibold text-gray-900 dark:text-white">
                                    Паспорт
                                </h3>
                            </div>

                            <div className="space-y-4">
                                <DocumentUploadField
                                    title="Разворот с фотографией"
                                    file={passportFront}
                                    onChange={setPassportFront}
                                />

                                <DocumentUploadField
                                    title="Страница с регистрацией"
                                    file={passportRegistration}
                                    onChange={setPassportRegistration}
                                />
                            </div>
                        </div>

                        {/* Driver license */}
                        <div className="rounded-2xl border border-gray-100 bg-gray-50/70 p-4 dark:border-gray-800 dark:bg-gray-800/30">
                            <div className="mb-4">
                                <h3 className="font-semibold text-gray-900 dark:text-white">
                                    Водительское удостоверение
                                </h3>
                            </div>

                            <div className="space-y-4">
                                <DocumentUploadField
                                    title="Лицевая сторона"
                                    file={driverFront}
                                    onChange={setDriverFront}
                                />

                                <DocumentUploadField
                                    title="Оборотная сторона"
                                    file={driverBack}
                                    onChange={setDriverBack}
                                />
                            </div>
                        </div>
                    </div>

                    {/* Info */}
                    <div className="mt-5 flex gap-3 rounded-xl bg-primary-50 p-4 dark:bg-primary-950/20">
                        <ShieldCheck className="mt-0.5 h-5 w-5 shrink-0 text-primary-600" />

                        <div>
                            <p className="text-sm font-medium text-gray-800">
                                Что будет дальше?
                            </p>

                            <p className="mt-1 text-xs leading-5 text-gray-500">
                                После отправки документы поступят менеджеру
                                на проверку. Внимание! Отправка заявки и документов не гарантирует подтверждение бронировния. Менеджер свяжется с вами в ближайшее время.
                            </p>
                        </div>
                    </div>
                </div>

                {/* Footer */}
                <div className="flex items-center justify-between gap-3 border-t border-gray-100 px-6 py-4 dark:border-gray-800">
                    <p className="text-xs text-gray-400">
                        {uploadedCount === 4
                            ? "Все документы готовы к отправке"
                            : `Осталось загрузить ${4 - uploadedCount} ${
                                  4 - uploadedCount === 1
                                      ? "документ"
                                      : "документа"
                              }`}
                    </p>

                    <div className="flex gap-3">
                        <Button
                            variant="outline"
                            onClick={handleClose}
                            disabled={loading}
                        >
                            Отмена
                        </Button>

                        <Button
                            onClick={upload}
                            disabled={loading || uploadedCount !== 4}
                        >
                            {loading
                                ? "Отправка..."
                                : "Отправить документы"}
                        </Button>
                    </div>
                </div>
            </div>
        </div>
    );
};
