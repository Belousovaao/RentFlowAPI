import { useEffect, useState } from 'react';
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
    
    // Блокировка прокрутки при открытии модалки
    useEffect(() => {
        if (!isOpen || !asset) return;

        const html = document.documentElement;
        const body = document.body;
        const prevHtmlOverflow = html.style.overflow;
        const prevBodyOverflow = body.style.overflow;
        const prevBodyPaddingRight = body.style.paddingRight;

        const scrollbarWidth = window.innerWidth - html.clientWidth;
        html.style.overflow = 'hidden';
        body.style.overflow = 'hidden';
        if (scrollbarWidth > 0) {
            body.style.paddingRight = `${scrollbarWidth}px`;
        }

        return () => {
            html.style.overflow = prevHtmlOverflow;
            body.style.overflow = prevBodyOverflow;
            body.style.paddingRight = prevBodyPaddingRight;
        };
    }, [isOpen, asset]);

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
                        initial={{ opacity: 0, y: 16 }}
                        animate={{ opacity: 1, y: 0 }}
                        exit={{ opacity: 0, y: 16 }}
                        transition={{ type: 'spring', damping: 28, stiffness: 320 }}
                        className="fixed inset-x-0 top-24 bottom-0 z-50 flex flex-col pointer-events-none min-h-0"
                    >
                        <div className="pointer-events-auto flex h-full min-h-0 flex-col max-w-[1600px] w-full mx-auto px-3 sm:px-4 pb-4">
                            <div className="sticky top-0 z-20 flex justify-end shrink-0 py-2 -mb-2 pointer-events-auto">
                                <button
                                    type="button"
                                    onClick={onClose}
                                    className="p-2 rounded-full bg-white/90 dark:bg-gray-800/90 text-gray-800 dark:text-gray-100 
                                             shadow-md border border-gray-200 dark:border-gray-600
                                             hover:bg-white dark:hover:bg-gray-800 transition-colors"
                                    aria-label="Закрыть"
                                >
                                    <X className="w-5 h-5" />
                                </button>
                            </div>
                            {/* Единая прокрутка модалки: виджет + предпросмотр вместе, без прокрутки страницы под ней */}
                            <div
                                className="flex-1 min-h-0 overflow-y-auto overscroll-y-contain [scrollbar-gutter:stable]"
                            >
                                <div className="flex flex-col lg:flex-row gap-4 lg:gap-6 overflow-visible pb-2">
                                    {/* Виджет — без overflow, чтобы не резать выпадающий календарь */}
                                    <div
                                        className="lg:w-1/2 flex flex-col items-stretch lg:items-start justify-start overflow-visible relative z-10"
                                        onClick={(e) => e.stopPropagation()}
                                    >
                                        <div className="w-full max-w-[440px] overflow-visible">
                                            <BookingWidget presetAsset={asset}/>
                                        </div>
                                    </div>
                                    {/* Предпросмотр */}
                                    <div
                                        className="lg:w-1/2 shrink-0 rounded-2xl bg-gray-50/95 dark:bg-gray-900/95 
                                                   border border-gray-200/80 dark:border-gray-700 p-4 sm:p-5 shadow-xl"
                                        onClick={(e) => e.stopPropagation()}
                                    >
                                    </div>
                                </div>
                            </div>
                        </div>
                    </motion.div>
                </>
            )}
        </AnimatePresence>
    );
};