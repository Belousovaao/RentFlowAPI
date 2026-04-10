import { motion, AnimatePresence } from 'framer-motion';
import { X } from 'lucide-react';
import { Asset } from '../../../entities/asset/asset.types';
import { BookingWidget } from './BookingWidget';

interface BookingModalProps {
    isOpen: boolean;
    asset: Asset | null;
    onClose: () => void;
}

export const BookingModal = ({ isOpen, asset, onClose }: BookingModalProps) => {
    if (!asset) return null;
    
    return (
        <AnimatePresence>
            {isOpen && (
                <>
                    <motion.div
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        exit={{ opacity: 0 }}
                        onClick={onClose}
                        className="fixed inset-0 bg-black/50 backdrop-blur-sm z-50"
                    />
                    <motion.div
                        initial={{ opacity: 0, scale: 0.9, y: 20 }}
                        animate={{ opacity: 1, scale: 1, y: 0 }}
                        exit={{ opacity: 0, scale: 0.9, y: 20 }}
                        transition={{ type: 'spring', damping: 25, stiffness: 300 }}
                        className="fixed top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 z-50"
                    >
                        <div className="relative">
                            <button
                                onClick={onClose}
                                className="absolute -top-12 right-0 p-2 text-white hover:text-gray-200 
                                         transition-colors rounded-full hover:bg-white/10"
                                aria-label="Закрыть"
                            >
                                <X className="w-6 h-6" />
                            </button>
                            <BookingWidget presetAsset={asset} />
                        </div>
                    </motion.div>
                </>
            )}
        </AnimatePresence>
    );
};