import { motion, AnimatePresence } from "framer-motion";

export const BackgroundLayer = ( { image }: { image: string }) => {
    return (
        <div className="fixed inset-0 -z-10 overflow-hidden">
            <AnimatePresence mode="wait">
                <motion.img
                    key={image}
                    src={image}
                    initial={{ opacity: 0, scale: 1.1 }}
                    animate={{ opacity: 0.4, scale: 1 }}
                    exit={{ opacity: 0 }}
                    transition={{ duration: 0.6 }}
                    className="w-full h-full object-cover blur-3xl"
                />
            </AnimatePresence>

            {/*Затемнение*/}
            <div className="absolute inset-0 bg-white/70 dark:bg-gray-900/80 backdrop-blur-2xl" />
        </div>
    );
};