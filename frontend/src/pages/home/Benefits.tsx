import { motion } from 'framer-motion';
import { Shield, Clock, Car, MapPin, CalendarOff, Luggage } from 'lucide-react';
import { useI18n } from '@/app/providers/I18nProvider';

export const Benefits = () => {

        const {t} = useI18n();

    const benefits = [
        {
            icon: Shield,
            title: t('benefits.insurance.title'),
            description: t('benefits.insurance.description'),
            color: 'text-blue-600',
            bgColor: 'bg-blue-100 dark:bg-blue-900/20',
        },
        {
            icon: Clock,
            title: t('benefits.support.title'),
            description: t('benefits.support.description'),
            color: 'text-green-600',
            bgColor: 'bg-green-100 dark:bg-green-900/20',
        },
        {
            icon: CalendarOff,
            title: t('benefits.cancellation.title'),
            description: t('benefits.cancellation.description'),
            color: 'text-purple-600',
            bgColor: 'bg-purple-100 dark:bg-purple-900/20',
        },
        {
            icon: Luggage,
            title: t('benefits.services.title'),
            description: t('benefits.services.description'),
            color: 'text-orange-600',
            bgColor: 'bg-orange-100 dark:bg-orange-900/20',
        },
        {
            icon: Car,
            title: t('benefits.condition.title'),
            description: t('benefits.condition.description'),
            color: 'text-red-600',
            bgColor: 'bg-red-100 dark:bg-red-900/20',
        },
        {
            icon: MapPin,
            title: t('benefits.delivery.title'),
            description: t('benefits.delivery.description'),
            color: 'text-teal-600',
            bgColor: 'bg-teal-100 dark:bg-teal-900/20',
        },
    ];

    return (
        <section className="py-20 bg-white dark:bg-gray-900">
            <div className="container mx-auto px-4">
                {/* Заголовок */}
                <div className="text-center mb-12">
                    <h2 className="text-3xl md:text-4xl font-bold text-gray-900 dark:text-white mb-4">
                        {t('benefits.whywe')}
                    </h2>
                    <p className="text-lg text-gray-600 dark:Experience luxury car rental with premium benefits and exceptional servicetext-gray-300 max-w-2xl mx-auto">
                        {t( 'benefits.whywe.description')}
                    </p>
                </div>

                {/* Сетка преимуществ */}
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                    {benefits.map((benefit, index) => (
                        <motion.div
                            key={benefit.title}
                            initial={{ opacity: 0, y: 20 }}
                            whileInView={{ opacity: 1, y: 0 }}
                            transition={{ delay: index * 0.1, duration: 0.5 }}
                            viewport={{ once: true }}
                            className="group p-6 bg-gray-50 dark:bg-gray-800 rounded-2xl 
                                       hover:shadow-lg transition-all duration-300
                                       hover:-translate-y-1"
                        >
                            <div className={`w-14 h-14 ${benefit.bgColor} rounded-xl 
                                    flex items-center justify-center mb-4
                                    group-hover:scale-110 transition-transform duration-300`}>
                                <benefit.icon className={`w-7 h-7 ${benefit.color}`} />
                            </div>
                            
                            <h3 className="text-xl font-semibold text-gray-900 dark:text-white mb-2">
                                {benefit.title}
                            </h3>
                            
                            <p className="text-gray-600 dark:text-gray-300 leading-relaxed">
                                {benefit.description}
                            </p>
                        </motion.div>
                    ))}
                </div>

                {/* Статистика */}
                {/* <div className="mt-16 pt-8 border-t border-gray-200 dark:border-gray-700">
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-8 text-center">
                        <div>
                            <p className="text-3xl md:text-4xl font-bold text-primary-600 mb-2">
                                500+
                            </p>
                            <p className="text-gray-600 dark:text-gray-300">Luxury Vehicles</p>
                        </div>
                        <div>
                            <p className="text-3xl md:text-4xl font-bold text-primary-600 mb-2">
                                10k+
                            </p>
                            <p className="text-gray-600 dark:text-gray-300">Happy Customers</p>
                        </div>
                        <div>
                            <p className="text-3xl md:text-4xl font-bold text-primary-600 mb-2">
                                24/7
                            </p>
                            <p className="text-gray-600 dark:text-gray-300">Customer Support</p>
                        </div>
                    </div>
                </div> */}
            </div>
        </section>
    );
};