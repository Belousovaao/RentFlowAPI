import { useState, useRef } from 'react';
import { useI18n } from '@/app/providers/I18nProvider';

type Props = {
    exterior: string[];
    interior: string[];
};

export const AssetViewer = ({ exterior, interior }: Props) => {
    const [mode, setMode] = useState<'exterior' | 'interior'>('exterior');
    const [currentIndex, setCurrentIndex] = useState(0);
    const { t } = useI18n();
    const isDragging = useRef(false);
    const startX = useRef(0);
    const startIndex = useRef(0);

    const images = mode === 'exterior' ? exterior : interior;

    // Если нет фото - показываем заглушку
    if (!images || images.length === 0) {
        return (
            <div className="relative">
                <div className="flex gap-2 mb-6 justify-center">
                    <button className="px-5 py-2 rounded-full text-sm bg-black text-white">
                        {t('asset.exterior')}
                    </button>
                    <button className="px-5 py-2 rounded-full text-sm bg-white/60 backdrop-blur border">
                        {t('asset.interior')}
                    </button>
                </div>
                <div className="h-[420px] flex items-center justify-center bg-gray-100 dark:bg-gray-800 rounded-xl">
                    <p className="text-gray-500">Нет фото для 360° обзора</p>
                </div>
            </div>
        );
    }

    const handleMouseDown = (e: React.MouseEvent) => {
        isDragging.current = true;
        startX.current = e.clientX;
        startIndex.current = currentIndex;
    };

    const handleMouseMove = (e: React.MouseEvent) => {
        if (!isDragging.current) return;
        const deltaX = e.clientX - startX.current;
        const deltaIndex = Math.round(deltaX / 8);
        
        if (deltaIndex !== 0) {
            let newIndex = startIndex.current + deltaIndex;
            newIndex = ((newIndex % images.length) + images.length) % images.length;
            setCurrentIndex(newIndex);
        }
    };

    const handleMouseUp = () => {
        isDragging.current = false;
    };

    return (
        <div className="relative">
            <div className="flex gap-2 mb-6 justify-center">
                <button
                    onClick={() => { setMode('exterior'); setCurrentIndex(0); }}
                    className={`px-5 py-2 rounded-full text-sm transition
                        ${mode === 'exterior' ? 'bg-black text-white' : 'bg-white/60 backdrop-blur border'}`}
                >
                    {t('asset.exterior')}
                </button>
                <button
                    onClick={() => { setMode('interior'); setCurrentIndex(0); }}
                    className={`px-5 py-2 rounded-full text-sm transition
                        ${mode === 'interior' ? 'bg-black text-white' : 'bg-white/60 backdrop-blur border'}`}
                >
                    {t('asset.interior')}
                </button>
            </div>

            <div
                className="relative h-[420px] flex items-center justify-center bg-gray-100 dark:bg-gray-800 rounded-xl cursor-grab active:cursor-grabbing overflow-hidden"
                onMouseDown={handleMouseDown}
                onMouseMove={handleMouseMove}
                onMouseUp={handleMouseUp}
                onMouseLeave={handleMouseUp}
            >
                <img
                    src={images[currentIndex]}
                    alt={`${mode} view ${currentIndex + 1}`}
                    className="max-h-full max-w-full object-contain"
                    draggable={false}
                />
                
                <div className="absolute bottom-4 left-1/2 transform -translate-x-1/2 bg-black/60 text-white text-xs px-3 py-1 rounded-full whitespace-nowrap">
                    🖱️ {t('asset.dragToRotate')}
                </div>
            </div>
        </div>
    );
};