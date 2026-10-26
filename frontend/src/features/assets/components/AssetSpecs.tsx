import { motion } from "framer-motion";
import { useI18n } from "@/app/providers/I18nProvider";
import { useDriveTypeLabel, useFuelTypeLabel, useTransmissionLabel } from "@/entities/asset/asset.hooks";

export const AssetSpecs = ({ asset }: any) => {
    const { t } = useI18n();

    const driveTypeLabel = useDriveTypeLabel();
    const transmissionLabel = useTransmissionLabel();
    const fuelTypeLabel = useFuelTypeLabel();

    const items = [
        { label: t('asset.color'), value: asset.color },
        { label: t('asset.engine'), value: asset.engine },
        { label: t('asset.fuel'), value: fuelTypeLabel(asset.fuelType) },
        { label: t('asset.driveType'), value: driveTypeLabel(asset.driveType) },
        { label: t('asset.transmission'), value: transmissionLabel(asset.transmission) },
        { label: t('asset.seats'), value: asset.seats },
        { label: t('asset.doors'), value: asset.doors },
        { label: t('asset.power'), value: `${asset.horsepower} ${t('common.hp')}`},
        { label: t('asset.topSpeed'), value: `${asset.topSpeed} ${t('asset.kmh')}`},
        { label: t('asset.acceleration'), value: asset.acceleration},
    ];

    return (
        <div className="space-y-3">
            {items.map((item, i) => (
                <motion.div
                    key={i}
                    initial={{ opacity: 0, x: 20 }}
                    animate={{ opacity: 1, x: 0 }}
                    transition={{ delay: i * 0.1 }}
                    className="flex justify-between border-b border-gray-400 pb-2"
                >
                    <span className="text-gray-900 dark:text-gray-300">{item.label}</span>
                    <span className="font-semibold text-gray-900 dark:text-gray-300">{item.value}</span>
                </motion.div>
            ))}
        </div>
    );
};