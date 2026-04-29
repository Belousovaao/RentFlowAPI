import { motion } from "framer-motion";
import { useI18n } from "@/app/providers/I18nProvider";

export const AssetSpecs = ({ asset }: any) => {
    const { t } = useI18n();

    const items = [
        { label: t('asset.color'), value: asset.color},
        { label: t('asset.engine'), value: asset.engine },
        { label: t('asset.fuel'), value: asset.fuelType},
        { label: t('asset.driveType'), value: asset.driveType},
        { label: t('asset.transmission'), value: asset.transmission },
        { label: t('asset.seats'), value: asset.seats},
        { label: t('asset.doors'), value: asset.doors},
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
                    className="flex justify-between border-b pb-2"
                >
                    <span className="text-gray-500">{item.label}</span>
                    <span className="font-semibold">{item.value}</span>
                </motion.div>
            ))}
        </div>
    );
};