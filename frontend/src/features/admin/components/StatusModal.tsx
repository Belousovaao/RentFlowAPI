import { VerificationStatus } from "@/entities/customer/customer.type";
import { CheckCircle, EyeIcon, FileQuestion, Info, XCircle } from "lucide-react";

interface StatusModalProps {
    open: boolean;
    currentStatus: VerificationStatus;
    onClose: () => void;
    onChange: (status: VerificationStatus) => void;
}

const statusTypes = [
    {
        type: VerificationStatus.Approved,
        title: 'Одобрен',
        icon: CheckCircle,
    },
    {
        type: VerificationStatus.Pending,
        title: 'На проверке',
        icon: EyeIcon,
    },
    {
        type: VerificationStatus.Rejected,
        title: 'Отказ',
        icon: XCircle
    }
];

export const StatusModal = ({
    open,
    currentStatus,
    onClose,
    onChange,
}: StatusModalProps) => {
    if (!open)
        return null;

    const handleChange = (status: VerificationStatus) => { 
        if (status === currentStatus)
            return;

        onChange(status);
        onClose();
    };

    return ( 
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 px-4" 
            onClick={onClose} 
        > 
            <div className="w-full max-w-md rounded-2xl bg-white p-6 shadow-2xl dark:bg-gray-800" 
                onClick={(e) => e.stopPropagation()} 
            > 
                <div className="mb-6 flex items-center justify-between"> 
                    <div> 
                        <h2 className="text-xl font-bold text-gray-900 dark:text-white"> 
                            Статус клиента 
                        </h2> 
                        <p className="mt-1 text-sm text-gray-500 dark:text-gray-400"> 
                            Выберите новый статус верификации 
                        </p> 
                    </div> 
                    <button 
                        type="button" 
                        onClick={onClose} 
                        className="rounded-lg p-2 text-gray-400 hover:bg-gray-100 hover:text-gray-600 dark:hover:bg-gray-700 dark:hover:text-gray-200"> 
                        ✕ 
                    </button> 
                </div> 
                <div className="space-y-3"> 
                    {statusTypes.map((status) => { 
                        const Icon = status.icon; 
                        const isCurrent = status.type === currentStatus; 
                        return ( 
                            <button 
                                key={status.type} 
                                type="button" 
                                onClick={() => handleChange(status.type)} 
                                disabled={isCurrent} 
                                className={`flex w-full items-center gap-4 rounded-xl border p-4 text-left transition-colors 
                                    ${ isCurrent 
                                        ? "border-primary-500 bg-primary-50 dark:border-primary-500 dark:bg-primary-900/20" 
                                        : "border-gray-200 hover:border-primary-400 hover:bg-gray-50 dark:border-gray-700 dark:hover:bg-gray-700" 
                                    }`} 
                                > 
                                    <Icon 
                                        className={`h-6 w-6 shrink-0 
                                            ${ isCurrent 
                                                ? "text-primary-600" 
                                                : "text-gray-500 dark:text-gray-400" 
                                            }`} 
                                    /> 
                                    <div className="flex-1"> 
                                        <p className="font-semibold text-gray-900 dark:text-white"> 
                                            {status.title} 
                                        </p> 
                                    </div> 
                                    {isCurrent && ( 
                                        <span className="text-xs font-medium text-primary-600"> 
                                            Текущий 
                                        </span> 
                                    )} 
                            </button> 
                        ); 
                    })} 
                </div> 
                <div className="mt-6 flex justify-end"> 
                    <button 
                        type="button" 
                        onClick={onClose} 
                        className="rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-100 dark:border-gray-600 dark:text-gray-200 dark:hover:bg-gray-700" > 
                            Отмена 
                    </button> 
                </div> 
            </div> 
        </div> 
    );
};