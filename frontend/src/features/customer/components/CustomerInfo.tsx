import { ShieldCheck } from "lucide-react";
import { useAuthStore } from "@/features/auth/stores/authStore";
import { useI18n } from "@/app/providers/I18nProvider";
import { CustomerDetailsDto, CustomerLookup } from "@/entities/customer/customer.type";

interface CustomerInfoProps {
    customer?: CustomerLookup | CustomerDetailsDto;
}

export const CustomerInfo = ({ customer } : CustomerInfoProps) => {
    const { user } = useAuthStore();
    const { t } = useI18n();

    const displayName = customer? ("displayName" in customer
        ? customer.displayName
        : customer.fullName ?? `${customer.firstName ?? ""} ${customer.lastName ?? ""}`.trim())
    : `${user?.firstName ?? ""} ${user?.lastName ?? ""}`.trim();
    const phone = customer?.phone ?? user?.phone;
    const email = customer?.email ?? user?.email;

    if (!user && !customer)
        return null;

    return (
        <div className="flex items-start gap-3">
            <ShieldCheck
                className="w-5 h-5 text-primary-600 mt-1 shrink-0"
            />
            <div className="flex-1">
                <p className="font-medium text-gray-900 dark:text-white">
                    {displayName}
                </p>

                {phone && (
                    <p className="text-sm text-gray-700 dark:text-gray-300">
                        {t("auth.methodPhone")}: {phone}
                    </p>
                )}

                {email && (
                    <p className="text-sm text-gray-700 dark:text-gray-300">
                        Email: {email}
                    </p>
                )}
            </div>
        </div>
    );
};