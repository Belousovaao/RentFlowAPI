import React, { createContext, useContext, useEffect, useState } from 'react';

type Language = 'en' | 'ru' | 'zh';

interface Translations {
    [key: string]: string;
}

// Базовые переводы
const translations: Record<Language, Translations> = {
    en: {
        'nav.home': 'Home',
        'nav.exploreCars': 'Explore cars',
        'nav.rentalTerms': 'Rental terms',
        'nav.aboutUs': 'About us',
        'nav.contact': 'Contact',
        'nav.signIn': 'Sign in',
        'booking.title': 'Ride your wave',
        'booking.selectCar': 'Select a car',
        'booking.selectDates': 'Select dates',
        'booking.bookNow': 'Book Now',
        'booking.reset': 'Reset',
        'booking.dailyRate': 'Daily rate:',
        'booking.numberOfDays': 'Number of days:',
        'booking.days' : 'days',
        'booking.total': 'Total',
        'asset.available': 'Available Now',
        'asset.comingSoon': 'Coming Soon',
        'asset.details': 'Details',
        'asset.perDay': 'per day',
        'asset.transmission': 'Transmission',
        'asset.seats': 'Seats',
        'asset.doors': 'Doors',
        'asset.fromYou': 'km from you',
        'common.loading': 'Loading...',
        'common.error': 'Error',
        'common.save': 'Save',
        'common.cancel': 'Cancel',
        'common.rubles': 'rub.',
        'common.name': 'Volnami Drive',
        'popular.vehicles': 'Most popular vehicles',
        'benefits.insurance.title': 'Full Insurance',
        'benefits.insurance.description': 'Comprehensive coverage included with every rental',
        'benefits.support.title': '24/7 support',
        'benefits.support.description': 'Round-the-clock customer service assistance',
        'benefits.cancellation.title': 'Free Cancellation',
        'benefits.cancellation.description': 'Cancel up to 24 hours before pickup',
        'benefits.condition.title': 'Perfect Condition',
        'benefits.condition.description': 'Every vehicle is thoroughly cleaned and fully serviced before rental',
        'benefits.delivery.title': 'Free Delivery',
        'benefits.delivery.description': 'Free delivery within 10km of our locations',
        'benefits.services.title': 'Extra Services',
        'benefits.services.description': 'Child seat, Airport delivery, Photo session, Driver service, Mobile Wi-Fi, Refreshments, Extended insurance',
        'benefits.whywe': 'Why Volnami Drive?',
        'benefits.whywe.description': 'Experience luxury car rental with premium benefits and exceptional service',
        'asset.notfound': 'No cars found',
        'footer.faq': 'FAQ',
        'footer.privacy': 'Privacy Policy',
        'footer.supportCenter': 'Support Center',
        'address': 'Krasnoyarsk, 40/1 Krasnodarskaya st.',
        'footer.quickLinks': 'Site Map',
        'footer.support': 'Support',
        'footer.contact': 'Contact us',
    },
    ru: {
        'nav.home': 'Главная',
        'nav.exploreCars': 'Автомобили',
        'nav.rentalTerms': 'Условия аренды',
        'nav.aboutUs': 'О компании',
        'nav.contact': 'Контакты',
        'booking.title': 'Двигайся на своей волне',
        'booking.selectCar': 'Выберите автомобиль',
        'booking.selectDates': 'Выберите даты',
        'booking.bookNow': 'Забронировать',
        'booking.reset': 'Сбросить',
        'booking.dailyRate': 'Цена за день:',
        'booking.numberOfDays': 'Количество дней:',
        'booking.days' : 'суток',
        'booking.day1' : 'сутки',
        'booking.total': 'Итого',
        'asset.available': 'Доступно сейчас',
        'asset.comingSoon': 'Скоро',
        'asset.details': 'Подробнее',
        'asset.perDay': 'в день',
        'asset.transmission': 'Коробка',
        'asset.seats': 'Мест',
        'asset.doors': 'Дверей',
        'asset.fromYou': 'км от вас',
        'common.loading': 'Загрузка...',
        'common.error': 'Ошибка',
        'common.save': 'Сохранить',
        'common.cancel': 'Отмена',
        'common.rubles': 'руб.',
        'common.name': 'Волнами Драйв',
        'nav.signIn': 'Войти',
        'popular.vehicles': 'Наиболее популярные авто',
        'benefits.insurance.title': 'Страхование',
        'benefits.insurance.description': 'Каждый автомобиль полностью застрахован, также по вашему усмотрению можно включить доп. пакеты',
        'benefits.support.title': '24/7 поддержка',
        'benefits.support.description': 'Круглосуточный приём заявок и поддержка клиентов',
        'benefits.cancellation.title': 'Бесплатная отмена',
        'benefits.cancellation.description': 'Вы можете отменить бронирование не позднее чем за 24 часа без удержания предоплаты',
        'benefits.condition.title': 'Идеальное состояние',
        'benefits.condition.description': 'Каждый автомобиль тщательно вымыт и полностью технически исправен',
        'benefits.delivery.title': 'Бесплатная доставка',
        'benefits.delivery.description': 'Бесплатная подача авто в радиусе 10 км от его исходного местоположения',
        'benefits.services.title': 'Дополнительные услуги',
        'benefits.services.description': 'Детское к ресло, Встреча в аэропорту, Фотосессия, Услуги водителя, Мобильный Wi-Fi, Напитки и снеки, Расширенная страховка',
        'benefits.whywe' : 'Почему Волнами Drive?',
        'benefits.whywe.description': 'Ощутите роскошь аренды автомобилей с первоклассными преимуществами и исключительным сервисом',
        'asset.notfound': 'Такой машины не найдено',
        'footer.faq': 'Часто задаваемые вопросы',
        'footer.privacy': 'Политика конфиденциальности',
        'footer.supportCenter': 'Служба поддержки',
        'address': 'г. Красноярск, ул. Краснодарская 40/1',
        'footer.quickLinks': 'Карта сайта',
        'footer.support': 'Поддержка',
        'footer.contact': 'Связаться с нами',
    },
    zh: {
        'nav.home': '首页',
        'nav.exploreCars': '车辆',
        'nav.rentalTerms': '如何运作',
        'nav.aboutUs': '关于我们',
        'nav.contact': '联系我们',
        'booking.title': '乘着你的波浪',
        'booking.selectCar': '选择车辆',
        'booking.selectDates': '选择日期',
        'booking.bookNow': '立即预订',
        'booking.reset': 'Reset',
        'booking.dailyRate': '每日价格:',
        'booking.numberOfDays': '天数:',
        'booking.days' : 'days',
        'booking.total': '总计',
        'asset.available': '现在可用',
        'asset.comingSoon': '即将推出',
        'asset.details': '详情',
        'asset.perDay': '每天',
        'asset.transmission': '变速箱',
        'asset.seats': '座位',
        'asset.doors': '门',
        'asset.fromYou': '公里远',
        'common.loading': '加载中...',
        'common.error': '错误',
        'common.save': '保存',
        'common.cancel': '取消',
        'common.rubles': 'руб.',
        'nav.signIn': 'Sigh in',
        'popular.vehicles': 'Popular vehicles',
        'benefits.insurance.title': 'Полное страховое покрытие',
        'benefits.insurance.description': 'Каждый автомобиль застрахован, полное КАСКО',
        'benefits.support.title': '24/7 поддержка',
        'benefits.support.description': 'Круглосуточный приём заявок и поддержка клиентов',
        'benefits.cancellation.title': 'Бесплатная отмена',
        'benefits.cancellation.description': 'Вы можете отменить бронирование не позднее чем за 24 часа без удержания предоплаты',
        'benefits.condition.title': '完美状态',
        'benefits.condition.description': '每辆车在租赁前都经过彻底清洁和全面检修',
        'benefits.delivery.title': 'Бесплатная доставка',
        'benefits.delivery.description': 'Бесплатная подача авто в радиусе 10 км от его исходного местоположения',
        'benefits.services.title': '附加服务',
        'benefits.services.description': '儿童座椅, 机场接送, 摄影服务, 司机服务, 移动Wi-Fi, 饮料小吃, 扩展保险',
        'benefits.whywe' : 'Почему Волнами Drive?',
        'benefits.whywe.description': 'Ощутите роскошь аренды автомобилей с первоклассными преимуществами и исключительным сервисом',
        'asset.notfound': 'Ни одной машины не найдено',
        'footer.faq': 'FAQ',
        'footer.privacy': 'Privacy Policy',
        'footer.supportCenter': 'Support Center',
        'address': 'Krasnoyarsk, 40/1 Krasnodarskaya st.',
        'common.name': 'Volnami Drive',
        'footer.quickLinks': 'Site Map',
        'footer.support': 'Support',
        'footer.contact': 'Contact us',
    },
};

interface I18nContextType {
    language: Language;
    setLanguage: (lang: Language) => void;
    t: (key: string) => string;
}

const I18nContext = createContext<I18nContextType | undefined>(undefined);

export const useI18n = () => {
    const context = useContext(I18nContext);
    if (!context) {
        throw new Error('useI18n must be used within I18nProvider');
    }
    return context;
};

export const I18nProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const [language, setLanguage] = useState<Language>(() => {
        const saved = localStorage.getItem('language') as Language;
        return saved && ['en', 'ru', 'zh'].includes(saved) ? saved : 'ru';
    });

    useEffect(() => {
        localStorage.setItem('language', language);
        document.documentElement.lang = language;
    }, [language]);

    const t = (key: string): string => {
        return translations[language][key] || translations.en[key] || key;
    };

    return (
        <I18nContext.Provider value={{ language, setLanguage, t }}>
            {children}
        </I18nContext.Provider>
    );
};