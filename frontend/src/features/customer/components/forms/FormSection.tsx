import { ReactNode } from 'react';
import { ChevronDown, ChevronRight, LucideIcon } from 'lucide-react';

interface FormSectionProps {
    title: string;
    description?: string;
    icon: LucideIcon;
    open: boolean;
    onToggle: () => void;
    children: ReactNode;
}

export const FormSection = ({
    title,
    description,
    icon: Icon,
    open,
    onToggle,
    children,
}: FormSectionProps) => {
    return (
        <div className="rounded-xl border border-gray-200 dark:border-gray-700 bg-gray-50/50 dark:bg-gray-900/30 overflow-hidden">
            <button
                type="button"
                onClick={onToggle}
                className="w-full flex items-center justify-between p-4 text-left hover:bg-gray-100/70 dark:hover:bg-gray-800/60 transition-colors"
            >
                <div className="flex items-center gap-3">
                    <div className="flex items-center justify-center w-9 h-9 rounded-lg bg-primary-100 dark:bg-primary-900/30 text-primary-600 dark:text-primary-400">
                        <Icon className="w-5 h-5" />
                    </div>

                    <div>
                        <h3 className="font-semibold text-gray-900 dark:text-white">
                            {title}
                        </h3>

                        {description && (
                            <p className="text-xs text-gray-500 dark:text-gray-400 mt-0.5">
                                {description}
                            </p>
                        )}
                    </div>
                </div>

                {open ? (
                    <ChevronDown className="w-5 h-5 text-gray-400" />
                ) : (
                    <ChevronRight className="w-5 h-5 text-gray-400" />
                )}
            </button>

            {open && (
                <div className="border-t border-gray-200 dark:border-gray-700 p-4">
                    {children}
                </div>
            )}
        </div>
    );
};