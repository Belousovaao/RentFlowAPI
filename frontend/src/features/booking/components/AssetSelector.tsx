// features/booking/components/AssetSelector.tsx
import { useEffect, useRef, useState } from 'react';
import { useAssets } from '../../../entities/asset/asset.hooks';
import type { Asset } from '../../../entities/asset/asset.types';
import { useI18n } from '../../../app/providers/I18nProvider';
import { formatPrice } from '@/shared/utils/formatters';

interface AssetSelectorProps {
    value: Asset | null;
    onChange: (asset: Asset) => void;
}

export const AssetSelector = ({ value, onChange }: AssetSelectorProps) => {
    const [isOpen, setIsOpen] = useState(false);
    const [searchTerm, setSearchTerm] = useState('');
    const { data: assetsResponse, isLoading } = useAssets();
    const { t } = useI18n();

    const selectorRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (selectorRef.current && !selectorRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, []);
    
    const assets = assetsResponse?.data || [];
    const filteredAssets = assets?.filter(asset =>
        `${asset.brandName} ${asset.model}`.toLowerCase().includes(searchTerm.toLowerCase())
    ) || [];
    
    return (
        <div ref={selectorRef} className="relative">
            <div
                onClick={() => setIsOpen(!isOpen)}
                className="w-full p-3 border border-gray-300 dark:border-gray-600 
                           rounded-lg cursor-pointer bg-white dark:bg-gray-800
                           hover:border-primary-500 transition-colors"
            >
                <div className="flex justify-between items-center">
                    <span className={!value ? 'text-gray-400' : 'text-gray-900 dark:text-white'}>
                        {value ? `${value.brandName} ${value.model}` : t('booking.selectCar')}
                    </span>
                    <svg
                        className={`w-5 h-5 transition-transform ${isOpen ? 'rotate-180' : ''}`}
                        fill="none"
                        stroke="currentColor"
                        viewBox="0 0 24 24"
                    >
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                    </svg>
                </div>
            </div>
            
            {isOpen && (
                <div className="absolute top-full mt-2 w-full z-50 bg-white dark:bg-gray-800 
                                rounded-lg shadow-xl border border-gray-200 dark:border-gray-700">
                    <div className="p-2">
                        <input
                            type="text"
                            placeholder="Search cars..."
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                            className="w-full p-2 border border-gray-300 dark:border-gray-600 
                                       rounded-lg bg-white dark:bg-gray-700
                                       focus:outline-none focus:border-primary-500"
                        />
                    </div>
                    
                    <div className="max-h-64 overflow-y-auto">
                        {isLoading ? (
                            <div className="p-4 text-center text-gray-500">{t('common.loading')}</div>
                        ) : filteredAssets.length === 0 ? (
                            <div className="p-4 text-center text-gray-500">{t('asset.notfound')}</div>
                        ) : (
                            filteredAssets.map((asset) => (
                                <div
                                    key={asset.id}
                                    onClick={() => {
                                        onChange(asset);
                                        setIsOpen(false);
                                    }}
                                    className="p-3 hover:bg-gray-100 dark:hover:bg-gray-700 
                                               cursor-pointer transition-colors"
                                >
                                    <div className="flex justify-between items-center">
                                        <div>
                                            <p className="font-semibold text-gray-900 dark:text-white">
                                                {asset.brandName} {asset.model}
                                            </p>
                                            <p className="text-sm text-gray-500">
                                                {formatPrice(asset.dailyPrice)} {t('asset.perDay')}
                                            </p>
                                        </div>
                                        {value?.id === asset.id && (
                                            <svg className="w-5 h-5 text-primary-500" fill="currentColor" viewBox="0 0 20 20">
                                                <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                                            </svg>
                                        )}
                                    </div>
                                </div>
                            ))
                        )}
                    </div>
                </div>
            )}
        </div>
    );
};