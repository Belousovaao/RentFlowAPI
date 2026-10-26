import { useI18n } from "@/app/providers/I18nProvider";
import { Button } from "@/features/ui/components/Button";

interface BookingButtonProps {
    disabled?: boolean;
    loading?: boolean;
    onClick: () => void | Promise<void>;
}

export const BookingButton = ({
    disabled,
    loading,
    onClick,
}: BookingButtonProps) => {
    const { t } = useI18n();

    return (
        <Button
            variant="gradient"
            className="flex-1"
            disabled={disabled || loading}
            onClick={onClick}
        >
            {loading
                ? t("common.loading")
                : t("checkout.confirm")}
        </Button>
    );
};