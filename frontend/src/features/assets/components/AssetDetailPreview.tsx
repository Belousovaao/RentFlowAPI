import { useState } from 'react';
import { motion } from 'framer-motion';
import { Fuel, Settings, Users, DoorOpen, Gauge, Sparkles, Shield, Star, MapPin, ChevronLeft, ChevronRight, CheckCircle, Zap, Car,} from 'lucide-react';
import type { Asset } from '@/entities/asset/asset.types';
import { formatPrice } from '@/shared/utils/formatters';
import { useFuelTypeLabel, useTransmissionLabel, useDriveTypeLabel } from '@/entities/asset/asset.hooks';
import { useI18n } from '@/app/providers/I18nProvider';

interface AssetDetailPreviewProps {
    asset: Asset;
    className?: string;
}

export const AssetDetailPreview = ({ asset, className = '' }: AssetDetailPreviewProps) => {
    const [selectedImage, setSelectedImage] = useState(0);
    const [viewType, setViewType] = useState<'exterior' | 'interior'>('exterior');
    const { t } = useI18n();
 
    const getFuelTypeLabel = useFuelTypeLabel();
    const getTransmissionLabel = useTransmissionLabel();
    const getDriveTypeLabel = useDriveTypeLabel();

    const getCurrentImages = (): string[] => {
        if (viewType === 'exterior') {
            if (asset.exteriorPhotos && asset.exteriorPhotos.length > 0) {
                return asset.exteriorPhotos.map(p => p.url);
            }
            return ['/car-placeholder.jpg', '/car-placeholder-2.jpg', '/car-placeholder-3.jpg'];
        } else {
            if (asset.interiorPhotos && asset.interiorPhotos.length > 0) {
                return asset.interiorPhotos.map(p => p.url);
            }
            return ['/interior-placeholder.jpg', '/interior-placeholder-2.jpg'];
        }
    };

    const images = getCurrentImages();
    const handleViewTypeChange = (type: 'exterior' | 'interior') => {
        setViewType(type);
        setSelectedImage(0);
    };

    return (
        <div
        className={`space-y-6 pr-1 ${className}`}
        onClick={(e) => e.stopPropagation()}
        >
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 lg:gap-8">
            {/* Галерея */}
            <div>
                {/* Переключатели экстерьер/интерьер */}
                <div className="flex gap-2 mb-3">
                        <button
                            type="button"
                            onClick={() => handleViewTypeChange('exterior')}
                            className={`px-4 py-2 rounded-lg text-sm font-medium transition-all
                                ${viewType === 'exterior' 
                                    ? 'bg-primary-600 text-white' 
                                    : 'bg-gray-100 dark:bg-gray-800 text-gray-600 dark:text-gray-400 hover:bg-gray-200 dark:hover:bg-gray-700'
                                }`}
                        >
                            🚗 Экстерьер
                        </button>
                        <button
                            type="button"
                            onClick={() => handleViewTypeChange('interior')}
                            className={`px-4 py-2 rounded-lg text-sm font-medium transition-all
                                ${viewType === 'interior' 
                                    ? 'bg-primary-600 text-white' 
                                    : 'bg-gray-100 dark:bg-gray-800 text-gray-600 dark:text-gray-400 hover:bg-gray-200 dark:hover:bg-gray-700'
                                }`}
                        >
                            🪑 Интерьер
                        </button>
                    </div>

                    <motion.div
                        key={viewType} // Анимация при смене типа
                        initial={{ opacity: 0, x: -20 }}
                        animate={{ opacity: 1, x: 0 }}
                        className="relative rounded-2xl overflow-hidden bg-gray-100 dark:bg-gray-800"
                    >
                        <img
                            src={images[selectedImage]}
                            alt={`${asset.brandName} ${asset.model} - ${viewType}`}
                            className="w-full h-[280px] sm:h-[320px] lg:h-[360px] object-cover"
                            onError={(e) => {
                                e.currentTarget.src = viewType === 'exterior' 
                                    ? '/car-placeholder.jpg' 
                                    : '/interior-placeholder.jpg';
                            }}
                        />

                        {images.length > 1 && (
                            <>
                                <button
                                    type="button"
                                    onClick={() =>
                                        setSelectedImage((prev) =>
                                            prev === 0 ? images.length - 1 : prev - 1
                                        )
                                    }
                                    className="absolute left-4 top-1/2 transform -translate-y-1/2
                                               p-2 bg-white/90 dark:bg-gray-800/90 rounded-full
                                               hover:bg-white dark:hover:bg-gray-800 transition-colors"
                                >
                                    <ChevronLeft className="w-5 h-5" />
                                </button>
                                <button
                                    type="button"
                                    onClick={() =>
                                        setSelectedImage((prev) =>
                                            prev === images.length - 1 ? 0 : prev + 1
                                        )
                                    }
                                    className="absolute right-4 top-1/2 transform -translate-y-1/2
                                               p-2 bg-white/90 dark:bg-gray-800/90 rounded-full
                                               hover:bg-white dark:hover:bg-gray-800 transition-colors"
                                >
                                    <ChevronRight className="w-5 h-5" />
                                </button>
                            </>
                        )}
                        
                        {/* Индикатор количества фото */}
                        {images.length > 0 && (
                            <div className="absolute bottom-3 right-3 bg-black/60 text-white text-xs px-2 py-1 rounded-full">
                                {selectedImage + 1} / {images.length}
                            </div>
                        )}
                    </motion.div>

                    {images.length > 1 && (
                        <div className="flex gap-2 mt-4 overflow-x-auto pb-2">
                            {images.map((img, idx) => (
                                <button
                                    type="button"
                                    key={idx}
                                    onClick={() => setSelectedImage(idx)}
                                    className={`w-16 h-16 sm:w-20 sm:h-20 rounded-lg overflow-hidden border-2 
                                               transition-all duration-200 flex-shrink-0
                                               ${
                                                   selectedImage === idx
                                                       ? 'border-primary-600'
                                                       : 'border-transparent opacity-60 hover:opacity-100'
                                               }`}
                                >
                                    <img
                                        src={img}
                                        alt=""
                                        className="w-full h-full object-cover"
                                        onError={(e) => {
                                            e.currentTarget.src = '/car-placeholder.jpg';
                                        }}
                                    />
                                </button>
                            ))}
                        </div>
                    )}
                </div>


                {/* Инфо */}
                <motion.div
                    initial={{ opacity: 0, x: 20 }}
                    animate={{ opacity: 1, x: 0 }}
                    className="space-y-5"
                >
                    <div>
                        <h2 className="text-2xl md:text-3xl font-bold text-gray-900 dark:text-white">
                            {asset.brandName} {asset.model}
                        </h2>
                        <div className="flex flex-wrap items-center justify-between gap-2 mt-2">
                            <div className="flex items-center space-x-2 text-sm">
                                <Star className="w-5 h-5 text-yellow-400 fill-current" />
                                <span className="text-gray-600 dark:text-gray-300">4.9</span>
                                <span className="text-gray-400">•</span>
                                <span className="text-gray-600 dark:text-gray-300">156 отзывов</span>
                            </div>
                            <div className="flex items-center text-gray-600 dark:text-gray-300 text-sm">
                                <MapPin className="w-4 h-4 mr-1 shrink-0" />
                                <span>{asset.locationName || 'Локация не указана'}</span>
                            </div>
                        </div>
                    </div>

                    <div className="flex items-baseline space-x-2">
                        <span className="text-2xl md:text-3xl font-bold text-primary-600">
                            {formatPrice(asset.dailyPrice)}
                        </span>
                        <span className="text-gray-600 dark:text-gray-400">{t('asset.perDay')}</span>
                    </div>

                    <div className="grid grid-cols-2 gap-3 py-3 border-y border-gray-200 dark:border-gray-700 text-sm">
                        <div className="flex items-center space-x-2 min-w-0">
                            <Gauge className="w-5 h-5 text-primary-600 shrink-0" />
                            <div className="min-w-0">
                                <p className="text-xs text-gray-500">{t('asset.engine')}</p>
                                <p className="font-semibold truncate">{asset.engine}</p>
                            </div>
                        </div>
                        <div className="flex items-center space-x-2 min-w-0">
                            <Fuel className="w-5 h-5 text-primary-600 shrink-0" />
                            <div className="min-w-0">
                                <p className="text-xs text-gray-500">{t('asset.fuel')}</p>
                                <p className="font-semibold">{getFuelTypeLabel(asset.fuelType)}</p>
                            </div>
                        </div>
                        <div className="flex items-center space-x-2 min-w-0">
                            <Settings className="w-5 h-5 text-primary-600 shrink-0" />
                            <div className="min-w-0">
                                <p className="text-xs text-gray-500">{t('asset.transmission')}</p>
                                <p className="font-semibold">{getTransmissionLabel(asset.transmission)}</p>
                            </div>
                        </div>
                        <div className="flex items-center space-x-2 min-w-0">
                            <Car className="w-5 h-5 text-primary-600 shrink-0" />
                            <div className="min-w-0">
                                <p className="text-xs text-gray-500">{t('asset.driveType')}</p>
                                <p className="font-semibold">{getDriveTypeLabel(asset.driveType)}</p>
                            </div>
                        </div>
                        <div className="flex items-center space-x-2">
                            <Users className="w-5 h-5 text-primary-600 shrink-0" />
                            <div>
                                <p className="text-xs text-gray-500">{t('asset.seats')}</p>
                                <p className="font-semibold">{asset.seats}</p>
                            </div>
                        </div>
                        <div className="flex items-center space-x-2">
                            <DoorOpen className="w-5 h-5 text-primary-600 shrink-0" />
                            <div>
                                <p className="text-xs text-gray-500">{t('asset.doors')}</p>
                                <p className="font-semibold">{asset.doors}</p>
                            </div>
                        </div>
                        <div className="flex items-center space-x-2">
                            <Zap className="w-5 h-5 text-primary-600 shrink-0" />
                            <div>
                                <p className="text-xs text-gray-500">{t('asset.power')}</p>
                                <p className="font-semibold">{asset.horsepower} {t('common.hp')}</p>
                            </div>
                        </div>
                        <div className="flex items-center space-x-2">
                            <Gauge className="w-5 h-5 text-primary-600 shrink-0" />
                            <div>
                                <p className="text-xs text-gray-500">{t('asset.topSpeed')}</p>
                                <p className="font-semibold">{asset.topSpeed} {t('asset.kmh')}</p>
                            </div>
                        </div>
                    </div>

                    <div>
                        <h3 className="text-base font-semibold mb-2">{t('asset.description')}</h3>
                        <p className="text-sm text-gray-600 dark:text-gray-300 leading-relaxed">
                            {asset.fullDescription || asset.shortDescription}
                        </p>
                    </div>

                    {asset.features && asset.features.length > 0 && (
                        <div>
                            <h3 className="text-base font-semibold mb-2">Особенности</h3>
                            <div className="grid grid-cols-1 gap-2">
                                {asset.features.map((feature, index) => (
                                    <div key={index} className="flex items-center space-x-2">
                                        <CheckCircle className="w-4 h-4 text-green-500 shrink-0" />
                                        <span className="text-sm text-gray-600 dark:text-gray-300">{feature}</span>
                                    </div>
                                ))}
                            </div>
                        </div>
                    )}
                </motion.div>
            </div>

            {/* Компактные плашки как на странице деталей */}
            <div className="grid grid-cols-1 sm:grid-cols-3 gap-3 pt-2">
                <div className="flex items-center space-x-2 p-3 bg-white/80 dark:bg-gray-800/80 rounded-xl border border-gray-100 dark:border-gray-700">
                    <Shield className="w-7 h-7 text-primary-600 shrink-0" />
                    <div>
                        <h4 className="text-sm font-semibold">{t('benefits.insurance.title')}</h4>
                        <p className="text-xs text-gray-500">{t('benefits.insurance.description')}</p>
                    </div>
                </div>
                <div className="flex items-center space-x-2 p-3 bg-white/80 dark:bg-gray-800/80 rounded-xl border border-gray-100 dark:border-gray-700">
                    <Sparkles className="w-7 h-7 text-primary-600 shrink-0" />
                    <div>
                        <h4 className="text-sm font-semibold">{t('benefits.cancellation.title')}</h4>
                        <p className="text-xs text-gray-500">{t('benefits.cancellation.description')}</p>
                    </div>
                </div>
                <div className="flex items-center space-x-2 p-3 bg-white/80 dark:bg-gray-800/80 rounded-xl border border-gray-100 dark:border-gray-700 sm:col-span-1">
                    <CheckCircle className="w-7 h-7 text-primary-600 shrink-0" />
                    <div>
                        <h4 className="text-sm font-semibold">{t('benefits.delivery.title')}</h4>
                        <p className="text-xs text-gray-500">{t('benefits.delivery.description')}</p>
                    </div>
                </div>
            </div>
        </div>
    );
};
