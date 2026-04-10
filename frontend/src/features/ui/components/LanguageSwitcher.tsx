// features/ui/components/LanguageSwitcher.tsx
import { useI18n } from '../../../app/providers/I18nProvider';
import { useState } from 'react';

const languages = [
    { code: 'en', name: 'English', flag: '🇺🇸' },
    { code: 'ru', name: 'Русский', flag: '🇷🇺' },
    { code: 'zh', name: '中文', flag: '🇨🇳' },
];

export const LanguageSwitcher = () => {
    const { language, setLanguage } = useI18n();
    const [isOpen, setIsOpen] = useState(false);
    
    const currentLanguage = languages.find(l => l.code === language);
    
    return (
        <div className="relative">
            <button
                onClick={() => setIsOpen(!isOpen)}
                className="flex items-center space-x-2 px-3 py-2 rounded-lg
                         hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors
                         text-gray-800 dark:text-gray-300"
            >
                <span className="text-xl">{currentLanguage?.flag}</span>
                <span className="text-sm">{currentLanguage?.name}</span>
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                </svg>
            </button>
            
            {isOpen && (
                <div className="absolute top-full mt-2 right-0 bg-white dark:bg-gray-800 
                                rounded-lg shadow-xl border border-gray-200 dark:border-gray-700
                                overflow-hidden z-50">
                    {languages.map((lang) => (
                        <button
                            key={lang.code}
                            onClick={() => {
                                setLanguage(lang.code as any);
                                setIsOpen(false);
                            }}
                            className={`flex items-center space-x-3 w-full px-4 py-2 
                                       hover:bg-gray-100 dark:hover:bg-gray-700 transition-colors
                                       text-gray-800 dark:text-gray-300
                                       ${language === lang.code ? 'bg-primary-50 dark:bg-primary-900/20' : ''}`}
                        >
                            <span className="text-xl">{lang.flag}</span>
                            <span className="text-sm">{lang.name}</span>
                            {language === lang.code && (
                                <svg className="w-4 h-4 text-primary-500" fill="currentColor" viewBox="0 0 20 20">
                                    <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                                </svg>
                            )}
                        </button>
                    ))}
                </div>
            )}
        </div>
    );
};