import { useState, useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { useUIStore } from '../stores/uiStore';
import { useI18n } from '../../../app/providers/I18nProvider';
import { LanguageSwitcher } from './LanguageSwitcher';
import { ThemeToggle } from './ThemeToggle';

export const Header = () => {
    const [isScrolled, setIsScrolled] = useState(false);
    const { isHeaderVisible } = useUIStore();
    const { t, language } = useI18n();
    const location = useLocation();
    
    useEffect(() => {
        const handleScroll = () => {
            setIsScrolled(window.scrollY > 50);
        };
        window.addEventListener('scroll', handleScroll);
        return () => window.removeEventListener('scroll', handleScroll);
    }, []);
    
    const navigation = [
        { name: t('nav.exploreCars'), href: '/assets' },
        { name: t('nav.rentalTerms'), href: '/rental-terms' },
        { name: t('nav.aboutUs'), href: '/about-us'},
        { name: t('nav.contact'), href: '/contact' },
    ];
    
    const getLogoByLanguage = () => {
        switch(language){
            case 'ru':
                return '/logo-ru.png'; 
            default:
                return '/logo-en.png';
        }
    };
    if (!isHeaderVisible) return null;
    
    return (
        <header
            className={`fixed top-0 w-full z-50 transition-all duration-300
                        bg-white/40 dark:bg-gray-900/50 backdrop-blur-xl
                        shadow-2xl border-b-0 border-gray-200/50 dark:border-gray-700/50`}
        >
            <nav className="container mx-auto px-4 h-20 flex items-center justify-between">
                <Link to="/" className="flex-shrink-0">
                    <img 
                            src={getLogoByLanguage()} 
                            alt="RentFlow"
                            className="h-12 w-auto"
                            onError={(e) => {
                                // Если картинка не загрузилась, показываем текст
                                e.currentTarget.style.display = 'none';
                                const parent = e.currentTarget.parentElement;
                                if (parent) {
                                    parent.innerHTML = '<span class="text-2xl font-bold text-primary-600">RentFlow</span>';
                                }
                            }}
                        />
                </Link>
                
                <div className="hidden md:flex items-center space-x-8">
                    {navigation.map((item) => (
                        <Link
                            key={item.name}
                            to={item.href}
                            className={`transition-colors hover:text-primary-600 whitespace-nowrap ${
                                location.pathname === item.href
                                    ? 'text-primary-600 font-semibold'
                                    : 'text-gray-800 dark:text-gray-300'
                            }`}
                        >
                            {item.name}
                        </Link>
                    ))}
                </div>
                
                <div className="flex items-center space-x-3">
                    <LanguageSwitcher />
                    <ThemeToggle />
                    
                    <button className="px-4 py-2 bg-primary-600 text-white rounded-lg 
                                     hover:bg-primary-700 transition-colors whitespace-nowrap">
                        {t('nav.signIn')}
                    </button>
                </div>
            </nav>
        </header>
    );
};