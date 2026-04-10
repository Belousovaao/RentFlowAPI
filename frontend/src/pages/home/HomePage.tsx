import { useScroll, useTransform, motion, useMotionValue, useSpring } from 'framer-motion';
import { usePopularAssets } from '../../entities/asset/asset.hooks';
import { AssetGrid } from '../../features/assets/components/AssetGrid';
import { BookingWidget } from '../../features/booking/components/BookingWidget';
import { Benefits } from './Benefits';
import { useI18n } from '@/app/providers/I18nProvider';

export const HomePage = () => {
    const { scrollYProgress } = useScroll();
    
    // ПАРАЛЛАКС ПРИ СКРОЛЛЕ
    const backgroundY = useTransform(scrollYProgress, [0, 1], ['0%', '-30%']);

    // ДВИЖЕНИЕ МЫШИ
    const mouseY = useMotionValue(0);
    const springY = useSpring(mouseY, { damping: 30, stiffness: 200 });

    const backgroundMouseX = useTransform(springY, [-300, 300], ['-0.5%', '0.5%']);
    
    // Нормализуем springY от -300..300 в 0..1
    const mouseProgress = useTransform(springY, [-300, 300], [0, 1]);
    
    const carMouseX = useTransform(mouseProgress, [0, 1], ['-27%', '-5%']);
    const carMouseY = useTransform(mouseProgress, [0, 1], ['-200%', '200%']);
    const carScale = useTransform(mouseProgress, [0, 1], [0.7, 2]);
    
    const { data: popularAssets, isLoading } = usePopularAssets(6);
    
    const handleMouseMove = (e: React.MouseEvent) => {
        const rect = e.currentTarget.getBoundingClientRect();
        const y = e.clientY - rect.top - rect.height / 2;
        mouseY.set(y);
    };
    
    const handleMouseLeave = () => {
        // Возвращаем все в исходное положение
        //mouseX.set(0);
        mouseY.set(0);
    };

    const {t} = useI18n();
    
    return (
        <main>
            {/* Hero Section */}
            <section 
                className="relative h-screen overflow-hidden"
                onMouseMove={handleMouseMove}
                onMouseLeave={handleMouseLeave}
            >
                {/* СЛОЙ 1: Фоновое изображение (реагирует на мышь) */}
                <motion.div 
                    className="absolute inset-0 rounded-xl overflow-hidden"
                    style={{ 
                        y: backgroundY,           // параллакс при скролле
                        x: backgroundMouseX       // эффект при движении мыши
                    }}
                >
                    <img 
                        src="/background.jpeg" 
                        className="w-full h-full object-cover"
                        alt="Luxury car background"
                    />
                    <div className="absolute inset-0" />
                </motion.div>
                
                {/* СЛОЙ 2: Автомобиль (реагирует на мышь сильнее) */}
                <motion.div 
                    className="absolute bottom-[40%] right-[22%] w-[40%] md:w-[25%] z-10"
                    style={{ 
                        x: carMouseX,
                        y: carMouseY,
                        scale: carScale,
                        transformOrigin: 'center center',
                    }}
                >
                    <img 
                        src="/geely-boyue-cool.png" 
                        className="w-[30%] h-auto"
                        alt="Geely Boyue Cool"
                        style={{ 
                            filter: 'drop-shadow(0 20px 15px rgba(0,0,0,0.3))' }}
                    />
                </motion.div>
                
                {/* Виджет бронирования */}
                <div className='absolute top-20 sm:top-24 md:top-28 lg:top-32 left-8 z-20'>
                    <BookingWidget />
                </div>
                
                {/* Индикатор скролла */}
                <motion.div
                    initial={{ opacity: 0, y: 20 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ delay: 1 }}
                    className="absolute bottom-8 left-1/2 transform -translate-x-1/2 z-20"
                >
                    <div className="w-6 h-10 border-2 border-white rounded-full flex justify-center">
                        <div className="w-1 h-2 bg-white rounded-full mt-2 animate-bounce" />
                    </div>
                </motion.div>
            </section>
            
            {/* Popular Assets */}
            <section className="py-20 bg-gray-50 dark:bg-gray-800/50">
                <div className="container mx-auto px-4">
                    <p className="text-xl text-gray-600 dark:text-gray-300 text-center mb-12">
                        {t('popular.vehicles')}
                    </p>
                    
                    {isLoading ? (
                        <div className="text-center">{t('common.loading')}</div>
                    ) : (
                        <AssetGrid assets={popularAssets || []} />
                    )}
                </div>
            </section>
            
            <Benefits />
        </main>
    );
};