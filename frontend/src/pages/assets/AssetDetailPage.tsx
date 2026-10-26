import { useI18n } from "@/app/providers/I18nProvider";
import { useAsset, useAssetCategoryLabel } from "@/entities/asset/asset.hooks";
import { AssetSpecs } from "@/features/assets/components/AssetSpecs";
import { AssetViewer } from "@/features/assets/components/AssetViewer";
import { BackgroundLayer } from "@/features/assets/components/BackgroundLayer";
import { BookingPannel } from "@/features/assets/components/BookingPannel";
import { useParams } from "react-router-dom";
import { GlassCard } from "@/features/ui/components/GlassCard";
import { getPhotoUrl } from '@/shared/utils/photoUrl';

export const AssetDetailPage = () => {
    const { t } = useI18n();
    const { id } = useParams();
    const { data: asset } = useAsset(id!);
    const categoryLabel = useAssetCategoryLabel();

    if (!asset) return null;

    const exteriorPhotos = (asset.photos || []).filter(p => p.type === 'Exterior');
    const interiorPhotos = (asset.photos || []).filter(p => p.type === 'Interior');

    const exteriorUrls = exteriorPhotos.map(p => getPhotoUrl(p.url));
    const interiorUrls = interiorPhotos.map(p => getPhotoUrl(p.url));

    return (
        <div className="min-h-screen bg-gray-50 dark:bg-gray-900 pt-24 pb-12">
            <div className="max-w-6xl mx-auto grid grid-cols-1 lg:grid-cols-2 gap-6">
                <div className="space-y-6">
                    {/* Viewer */}
                    <AssetViewer exterior={exteriorUrls} interior={interiorUrls} />
                    <div className="relative z-40">
                        <GlassCard>
                            <BookingPannel asset={asset} />
                        </GlassCard>
                    </div>   
                </div>

                {/* Правая колонка */}
                <div className="space-y-6">
                    {/* Марка и модель */}
                    <GlassCard>
                        <h1 className="text-3xl font-bold text-gray-900 dark:text-white">
                            {asset.brandName} {asset.model}
                        </h1>
                        <p className="text-gray-900 dark:text-white mt-1">
                            {asset.year} • {categoryLabel(asset.category)}
                        </p>
                    </GlassCard>
                    <GlassCard>
                        <h2 className="text-xl font-bold mb-4 text-gray-900 dark:text-white">{t('asset.characteristics')}</h2>
                        <AssetSpecs asset={asset} />
                    </GlassCard>
                </div>
            </div>
        </div>
    )
}