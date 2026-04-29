import { useState, useRef, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { User, LogOut, ChevronDown, Shield } from 'lucide-react';
import { useAuthStore } from '../stores/authStore';
import { useI18n } from '@/app/providers/I18nProvider';

export const UserMenu = () => {
    const { user, logout } = useAuthStore();
    const { t } = useI18n();
    const [open, setOpen] = useState(false);
    const ref = useRef<HTMLDivElement>(null);

    useEffect(() => {
        const close = (e: MouseEvent) => {
            if (ref.current && !ref.current.contains(e.target as Node)) setOpen(false);
        };
        document.addEventListener('mousedown', close);
        return () => document.removeEventListener('mousedown', close);
    }, []);

    if (!user) return null;

    const display = user.email || user.phone || user.firstName;
    const isAdmin = user.role === 'admin';

    return (
        <div className="relative" ref={ref}>
            <button
                type="button"
                onClick={() => setOpen(!open)}
                className="flex items-center gap-2 rounded-lg border border-gray-200 bg-white/80 px-3 py-2 text-sm font-medium text-gray-800 shadow-sm transition-colors hover:bg-gray-50 dark:border-gray-600 dark:bg-gray-800 dark:text-gray-100 dark:hover:bg-gray-700"
            >
                <span className="max-w-[140px] truncate">{display}</span>
                {isAdmin && <Shield className="h-4 w-4 shrink-0 text-amber-500" aria-hidden />}
                <ChevronDown className={`h-4 w-4 shrink-0 transition-transform ${open ? 'rotate-180' : ''}`} />
            </button>
            {open && (
                <div
                    className="absolute right-0 z-[60] mt-2 min-w-[200px] rounded-xl border border-gray-200 bg-white py-1 shadow-lg dark:border-gray-600 dark:bg-gray-800"
                >
                    <Link
                        to="/account"
                        className="flex items-center gap-2 px-4 py-2.5 text-sm text-gray-700 hover:bg-gray-50 dark:text-gray-200 dark:hover:bg-gray-700"
                        onClick={() => setOpen(false)}
                    >
                        <User className="h-4 w-4" />
                        {t('nav.account')}
                    </Link>
                    <button
                        type="button"
                        onClick={() => {
                            logout();
                            setOpen(false);
                        }}
                        className="flex w-full items-center gap-2 px-4 py-2.5 text-left text-sm text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20"
                    >
                        <LogOut className="h-4 w-4" />
                        {t('auth.logout')}
                    </button>
                </div>
            )}
        </div>
    );
};
