import { Link } from 'react-router-dom';
import { useI18n } from '../../../app/providers/I18nProvider';
import { motion } from 'framer-motion';
import { Instagram, Send, Phone, Mail, PhoneIncoming, MapPin, MessageCircle} from 'lucide-react';
import { BsTelegram, BsWhatsapp } from 'react-icons/bs';

export const Footer = () => {
    const { t } = useI18n();
    const currentYear = new Date().getFullYear();

    const quickLinks = [
        { name: t('nav.home'), href: '/' },
        { name: t('nav.exploreCars'), href: '/assets' },
        { name: t('nav.aboutUs'), href: '/about' },
        { name: t('nav.rentalTerms'), href: '/how-it-works' },
        { name: t('nav.contact'), href: '/contact' },
    ];

    const supportLinks = [
        { name: t('footer.faq'), href: '/faq' },
        { name: t('footer.privacy'), href: '/privacy' },
        { name: t('footer.supportCenter'), href: '/support' },
    ];

    const socialLinks = [
        { icon: Instagram, href: 'https://instagram.com', label: 'Instagram' },
        { icon: BsTelegram, href: 'https://instagram.com', label: 'Telegram' },
        { icon: BsWhatsapp, href: 'https://instagram.com', label: 'What\'s up' },
        { icon: MessageCircle, href: 'https://instagram.com', label: 'MAX' },
    ];

    const contactInfo = [
        { icon: PhoneIncoming, text: '7 (963) 565-28-17', href: 'tel:+79635652817' },
        { icon: Mail, text: 'info@volnamidrive.ru', href: 'mailto:info@volnamidrive.ru' },
        { icon: MapPin, text: t('address'), href: '#' },
    ];

    return (
        <footer className="bg-gray-900 dark:bg-gray-950 text-white">
            {/* Main Footer */}
            <div className="container mx-auto px-4 py-12">
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
                    {/* Brand Column */}
                    <div className="space-y-4">
                        <Link to="/" className="text-2xl font-bold text-primary-400">
                            {t('common.name')}
                        </Link>
                        <p className="text-gray-400 text-sm leading-relaxed">
                            {t('benefits.whywe.description')}
                        </p>
                        <div className="flex space-x-3">
                            {socialLinks.map((social, index) => (
                                <motion.a
                                    key={index}
                                    href={social.href}
                                    target="_blank"
                                    rel="noopener noreferrer"
                                    whileHover={{ scale: 1.1, y: -2 }}
                                    whileTap={{ scale: 0.95 }}
                                    className="p-2 bg-gray-800 hover:bg-primary-600 
                                             rounded-lg transition-colors duration-300"
                                    aria-label={social.label}
                                >
                                    <social.icon className="w-5 h-5" />
                                </motion.a>
                            ))}
                        </div>
                    </div>

                    {/* Quick Links */}
                    <div>
                        <h3 className="text-lg font-semibold mb-4">{t('footer.quickLinks')}</h3>
                        <ul className="space-y-2">
                            {quickLinks.map((link, index) => (
                                <li key={index}>
                                    <Link
                                        to={link.href}
                                        className="text-gray-400 hover:text-primary-400 
                                                   transition-colors duration-300 text-sm"
                                    >
                                        {link.name}
                                    </Link>
                                </li>
                            ))}
                        </ul>
                    </div>

                    {/* Support */}
                    <div>
                        <h3 className="text-lg font-semibold mb-4">{t('footer.support')}</h3>
                        <ul className="space-y-2">
                            {supportLinks.map((link, index) => (
                                <li key={index}>
                                    <Link
                                        to={link.href}
                                        className="text-gray-400 hover:text-primary-400 
                                                   transition-colors duration-300 text-sm"
                                    >
                                        {link.name}
                                    </Link>
                                </li>
                            ))}
                        </ul>
                    </div>

                    {/* Contact Info */}
                    <div>
                        <h3 className="text-lg font-semibold mb-4">{t('footer.contact')}</h3>
                        <ul className="space-y-3">
                            {contactInfo.map((item, index) => (
                                <li key={index}>
                                    <a
                                        href={item.href}
                                        className="flex items-center space-x-3 text-gray-400 
                                                   hover:text-primary-400 transition-colors duration-300 text-sm"
                                    >
                                        <item.icon className="w-4 h-4 flex-shrink-0" />
                                        <span>{item.text}</span>
                                    </a>
                                </li>
                            ))}
                        </ul>
                    </div>
                </div>
            </div>

            {/* Bottom Bar */}
            <div className="border-t border-gray-800">
                <div className="container mx-auto px-4 py-6">
                    <div className="flex flex-col md:flex-row justify-between items-center space-y-4 md:space-y-0">
                        <p className="text-gray-400 text-sm">
                            © {currentYear} VolnamiDrive. All rights reserved.
                        </p>
                        <div className="flex space-x-6">
                            <Link to="/cookies" className="text-gray-400 hover:text-primary-400 text-sm">
                                Cookies
                            </Link>
                        </div>
                    </div>
                </div>
            </div>
        </footer>
    );
};