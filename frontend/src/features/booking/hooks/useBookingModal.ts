import { useState } from 'react';
import { Asset } from '../../../entities/asset/asset.types';

export const useBookingModal = () => {
    const [isOpen, setIsOpen] = useState(false);
    const [selectedAsset, setSelectedAsset] = useState<Asset | null>(null);
    
    const openBookingModal = (asset: Asset) => {
        setSelectedAsset(asset);
        setIsOpen(true);
    };
    
    const closeBookingModal = () => {
        setIsOpen(false);
        setSelectedAsset(null);
    };
    
    return {
        isOpen,
        selectedAsset,
        openBookingModal,
        closeBookingModal,
    };
};