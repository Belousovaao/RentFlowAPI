import type { Asset, FuelType, TransmissionType, DriveType } from '../../../entities/asset/asset.types';
import { getFuelTypeLabel, getTransmissionLabel, getDriveTypeLabel } from '../../../entities/asset/asset.utils';
import { useState } from 'react';
import { Link } from 'react-router-dom';
import { motion } from 'framer-motion';
import { Fuel, Settings, Users, Gauge, Calendar, ChevronLeft, ChevronRight, Car } from 'lucide-react';

interface AssetCardProps {
  asset: Asset;
  index?: number;
  onBookClick?: (asset: Asset) => void;
}

export const AssetCard = ({ asset, index, onBookClick }: AssetCardProps) => {
    const [currentPhotoIndex, setCurrentPhotoIndex] = useState(0);
    const [isHovered, setIsHovered] = useState(false);
    
    const hasMultiplePhotos = asset.photos && asset.photos.length > 1;
    
    const nextPhoto = (e: React.MouseEvent) => {
        e.preventDefault();
        e.stopPropagation();
        if (hasMultiplePhotos) {
            setCurrentPhotoIndex((prev) => (prev + 1) % asset.photos.length);
        }
    };
    
    const prevPhoto = (e: React.MouseEvent) => {
        e.preventDefault();
        e.stopPropagation();
        if (hasMultiplePhotos) {
            setCurrentPhotoIndex((prev) => (prev - 1 + asset.photos.length) % asset.photos.length);
        }
    };

    const currentPhoto = asset.photos && asset.photos.length > 0 
        ? asset.photos[currentPhotoIndex] 
        : '/car-placeholder.jpg';
    
    return (
        <motion.div
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            transition={{ delay: (index || 0) * 0.05, duration: 0.4 }}
            viewport={{ once: true }}
            onMouseEnter={() => setIsHovered(true)}
            onMouseLeave={() => setIsHovered(false)}
            className="group bg-white dark:bg-gray-800 rounded-2xl overflow-hidden 
                       shadow-sm hover:shadow-xl transition-all duration-300
                       border border-gray-100 dark:border-gray-700"
        >
            {/* Галерея изображений */}
            <div className="relative h-56 overflow-hidden bg-gray-100 dark:bg-gray-700">
                <img
                    src={currentPhoto}
                    alt={`${asset.brandName} ${asset.model}`}
                    className="w-full h-full object-cover transition-transform duration-500
                              group-hover:scale-105"
                />
                
                {/* Стрелки навигации по фото */}
                {hasMultiplePhotos && isHovered && (
                    <>
                        <button
                            onClick={prevPhoto}
                            className="absolute left-2 top-1/2 -translate-y-1/2 p-1.5 
                                       bg-black/50 hover:bg-black/70 rounded-full 
                                       text-white transition-all duration-200"
                        >
                            <ChevronLeft className="w-4 h-4" />
                        </button>
                        <button
                            onClick={nextPhoto}
                            className="absolute right-2 top-1/2 -translate-y-1/2 p-1.5 
                                       bg-black/50 hover:bg-black/70 rounded-full 
                                       text-white transition-all duration-200"
                        >
                            <ChevronRight className="w-4 h-4" />
                        </button>
                    </>
                )}

                {/* Индикаторы количества фото */}
                {hasMultiplePhotos && (
                    <div className="absolute bottom-2 left-1/2 -translate-x-1/2 
                                  flex gap-1">
                        {asset.photos.map((_, idx) => (
                            <div
                                key={idx}
                                className={`w-1.5 h-1.5 rounded-full transition-all
                                    ${idx === currentPhotoIndex 
                                        ? 'bg-white w-3' 
                                        : 'bg-white/50'}`}
                            />
                        ))}
                    </div>
                )}
                
                {/* Цена */}
                <div className="absolute top-2 right-2 bg-primary-600 text-white 
                              px-3 py-1 rounded-lg text-sm font-bold">
                    {asset.dailyPrice.toLocaleString()} ₽/сут
                </div>
            </div>
            {/* Информация */}
            <div className="p-4">
                <div className="flex justify-between items-start mb-2">
                    <div>
                        <h3 className="font-bold text-lg text-gray-900 dark:text-white">
                            {asset.brandName} {asset.model}
                        </h3>
                        <div className="flex items-center gap-2 text-sm text-gray-500">
                            <Calendar className="w-3 h-3" />
                            <span>{asset.year}</span>
                            <span className="w-1 h-1 bg-gray-400 rounded-full" />
                            <Users className="w-3 h-3" />
                            <span>{asset.seats} мест</span>
                        </div>
                    </div>
                </div>

                {/* Характеристики */}
                <div className="grid grid-cols-2 gap-2 mt-3 py-2 border-y border-gray-100 dark:border-gray-700">
                    <div className="flex items-center gap-2">
                        <Fuel className="w-4 h-4 text-gray-400" />
                        <span className="text-sm">{getFuelTypeLabel(asset.fuelType)}</span>
                    </div>
                    <div className="flex items-center gap-2">
                        <Settings className="w-4 h-4 text-gray-400" />
                        <span className="text-sm">{getTransmissionLabel(asset.transmission)}</span>
                    </div>
                    <div className="flex items-center gap-2">
                        <Gauge className="w-4 h-4 text-gray-400" />
                        <span className="text-sm">{asset.horsepower} л.с.</span>
                    </div>
                    <div className="flex items-center gap-2">
                        <Car className="w-4 h-4 text-gray-400" />
                        <span className="text-sm">{getDriveTypeLabel(asset.driveType)}</span>
                    </div>
                </div>

                {/* Кнопки */}
                <div className="mt-4 grid grid-cols-1 gap-2 sm:grid-cols-2">
                    <Link
                        to={`/assets/${asset.id}`}
                        className="px-3 py-2 text-center border border-primary-600 
                                   text-primary-600 rounded-lg hover:bg-primary-50 
                                   dark:hover:bg-primary-900/20 transition-colors
                                   font-medium text-xs sm:text-sm whitespace-nowrap"
                    >
                        Подробнее
                    </Link>
                    <button
                        onClick={() => onBookClick?.(asset)}
                        disabled={!asset.isAvailable}
                        className="px-3 py-2 bg-primary-600 text-white rounded-lg 
                                   hover:bg-primary-700 transition-colors font-medium text-xs sm:text-sm
                                   whitespace-nowrap
                                   disabled:bg-gray-300 disabled:cursor-not-allowed
                                   dark:disabled:bg-gray-700"
                    >
                        Забронировать
                    </button>
                </div>
            </div>
        </motion.div>
    );
};