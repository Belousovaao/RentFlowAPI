import { useI18n } from "@/app/providers/I18nProvider";
import { useAsset } from "@/entities/asset/asset.hooks";
import { AssetSpecs } from "@/features/assets/components/AssetSpecs";
import { AssetViewer } from "@/features/assets/components/AssetViewer";
import { BackgroundLayer } from "@/features/assets/components/BackgroundLayer";
import { BookingPannel } from "@/features/assets/components/BookingPannel";
import { useParams } from "react-router-dom";
import { GlassCard } from "@/features/ui/components/GlassCard";

export const AssetDetailPage = () => {
    const { t } = useI18n();
    const { id } = useParams();
    const { data: asset } = useAsset(id!);

    console.log('Full asset object:', asset);
    console.log('Asset photos:', asset?.photos);
    if (!asset) return null;

    const getFullImageUrl = (url: string) => {
        return `http://localhost:9000/rentflow-photos${url}`;
    };

    const exteriorPhotos = (asset.photos || []).filter(p => p.type === 'Exterior');
    const interiorPhotos = (asset.photos || []).filter(p => p.type === 'Interior');
    console.log('Exterior photos after filter:', exteriorPhotos);
    console.log('Interior photos after filter:', interiorPhotos);

    const exteriorUrls = exteriorPhotos.map(p => getFullImageUrl(p.url));
    const interiorUrls = interiorPhotos.map(p => getFullImageUrl(p.url));

    console.log('Exterior photos:', exteriorUrls);
    console.log('Interior photos:', interiorUrls);

    return (
        <div className="min-h-screen pt-24 px-4">
            {/* Фон */}
            <BackgroundLayer  image={exteriorUrls[0]} />
            <div className="max-w-6xl mx-auto grid lg:grid-cols-2 gap-10">
                {/* Viewer */}
                <AssetViewer exterior={exteriorUrls} interior={interiorUrls} />

                {/* Правая колонка */}
                <div className="space-y-6">
                    {/* Марка и модель */}
                    <GlassCard>
                        <h1 className="text-3xl font-bold text-gray-900 dark:text-white">
                            {asset.brandName} {asset.model}
                        </h1>
                        <p className="text-gray-500 dark:text-gray-400 mt-1">
                            {asset.year} • {asset.category}
                        </p>
                    </GlassCard>
                    <GlassCard>
                        <h2 className="text-xl font-bold mb-4">{t('asset.characteristics')}</h2>
                        <AssetSpecs asset={asset} />
                    </GlassCard>

                    <GlassCard>
                        <BookingPannel asset={asset} />
                    </GlassCard>
                </div>
            </div>
        </div>
    )
}