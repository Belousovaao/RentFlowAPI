import type { BookingStatus } from "@/entities/booking/booking.types"; 
import { CheckCircle, EyeIcon, LucideIcon, PlayCircle, XCircle, } from "lucide-react"; 
import { statusLabels } from "@/pages/admin/AdminBookingsPage"; 

interface BookingStatusModalProps 
{ 
    open: boolean; 
    currentStatus: BookingStatus; 
    onClose: () => void; 
    onChange: (status: BookingStatus) => void; 
} 

const allowedTransitions: Record<BookingStatus, BookingStatus[]> = 
{ 
    Pending: ["Confirmed", "Cancelled"], 
    Confirmed: ["Active", "Cancelled"], 
    Active: ["Completed"], 
    Cancelled: [], 
    Completed: [], 
}; 
const statusConfig: Record< BookingStatus, 
{ 
    icon: LucideIcon; 
    activeClass: string; iconClass: string; 
} > = 
{ 
    Pending: 
    { 
        icon: EyeIcon, 
        activeClass: "border-amber-400 bg-amber-50 text-amber-700 dark:border-amber-500 dark:bg-amber-900/20 dark:text-amber-400", 
        iconClass: "text-amber-500", 
    }, 
    Confirmed: 
    { 
        icon: CheckCircle, 
        activeClass: "border-green-400 bg-green-50 text-green-700 dark:border-green-500 dark:bg-green-900/20 dark:text-green-400", 
        iconClass: "text-green-500", 
    }, 
    Active: 
    { 
        icon: PlayCircle, 
        activeClass: "border-primary-400 bg-primary-50 text-primary-700 dark:border-primary-500 dark:bg-primary-900/20 dark:text-primary-400", 
        iconClass: "text-primary-500", 
    }, 
    Completed: 
    { 
        icon: CheckCircle, 
        activeClass: "border-blue-400 bg-blue-50 text-blue-700 dark:border-blue-500 dark:bg-blue-900/20 dark:text-blue-400", 
        iconClass: "text-blue-500", 
    }, 
    Cancelled: 
    { 
        icon: XCircle, 
        activeClass: "border-red-400 bg-red-50 text-red-700 dark:border-red-500 dark:bg-red-900/20 dark:text-red-400", 
        iconClass: "text-red-500", 
    }, 
}; 

export const BookingStatusModal = (
    { 
        open, 
        currentStatus, 
        onClose, 
        onChange, 
    }: BookingStatusModalProps) => 
        { 
            if (!open) 
                { return null; } 

            const handleChange = (status: BookingStatus) => 
                { 
                    if (status === currentStatus) 
                        { return; } 
                    onChange(status); 
                    onClose(); 
                }; 
            const availableStatuses: BookingStatus[] = 
            [ 
                currentStatus, 
                ...allowedTransitions[currentStatus], 
            ]; 
            
            return ( 
                <div 
                    className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 px-4" 
                    onClick={onClose} 
                > 
                    <div 
                        className="w-full max-w-md rounded-2xl bg-white p-6 shadow-2xl dark:bg-gray-800" 
                        onClick={(e) => e.stopPropagation()} 
                    > 
                        <div 
                            className="mb-6 flex items-start justify-between"> 
                        <div> 
                            <h2 className="text-xl font-bold text-gray-900 dark:text-white"> 
                                Изменить статус 
                            </h2> 
                            <p className="mt-1 text-sm text-gray-500 dark:text-gray-400"> 
                                Выберите новый статус бронирования. 
                            </p> 
                        </div> 
                        <button 
                            type="button" 
                            onClick={onClose} 
                            className="rounded-lg p-2 text-gray-400 hover:bg-gray-100 hover:text-gray-600 dark:hover:bg-gray-700 dark:hover:text-gray-200" > 
                                ✕
                        </button> 
                    </div> 
                    <div className="space-y-3"> 
                        {availableStatuses.map((status) => { 
                            const config = statusConfig[status]; 
                            const Icon = config.icon; 
                            const isCurrent = status === currentStatus; 
                            
                            return ( 
                                <button 
                                    key={status} 
                                    type="button" 
                                    onClick={() => handleChange(status)} 
                                    disabled={isCurrent} 
                                    className={`flex w-full items-center gap-4 rounded-xl border p-4 text-left transition-colors 
                                        ${ isCurrent 
                                            ? config.activeClass 
                                            : "border-gray-200 hover:border-primary-400 hover:bg-gray-50 dark:border-gray-700 dark:hover:bg-gray-700" 
                                        }`} > 
                                    <Icon className={`h-6 w-6 shrink-0 
                                        ${ isCurrent 
                                            ? config.iconClass 
                                            : "text-gray-500 dark:text-gray-400" 
                                        }`} 
                                    /> 
                                    <div className="flex-1"> 
                                        <p className="font-semibold text-gray-900 dark:text-white"> 
                                            {statusLabels[status]} 
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