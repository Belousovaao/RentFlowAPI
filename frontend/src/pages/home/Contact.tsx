import { Link } from 'react-router-dom';
import { ArrowRight, Clock3, Mail, MapPin, MessageCircle, Phone, } from 'lucide-react';
import type { LucideIcon } from 'lucide-react';
import { contactDetails } from '@/shared/types/contacts';

type ContactMethod = {
    title: string;
    description: string;
    value: string;
    href: string;
    icon: LucideIcon;
};

export const ContactPage = () => {
    const methods: ContactMethod[] = [
        ...(contactDetails.phone.trim()
            ? [
                  {
                      title: 'Телефон',
                      description: 'Позвоните, чтобы уточнить детали аренды.',
                      value: contactDetails.phone,
                      href: `tel:${contactDetails.phone.replace(/[\s()-]/g, '')}`,
                      icon: Phone,
                  },
              ]
            : []),
        ...(contactDetails.telegram.trim()
            ? [
                  {
                      title: 'Telegram',
                      description: 'Напишите по вопросам бронирования и аренды.',
                      value: 'Написать в Telegram',
                      href: contactDetails.telegram,
                      icon: MessageCircle,
                  },
              ]
            : []),
        ...(contactDetails.email.trim()
            ? [
                  {
                      title: 'Электронная почта',
                      description: 'Для вопросов, которые удобнее обсудить письменно.',
                      value: contactDetails.email,
                      href: `mailto:${contactDetails.email}`,
                      icon: Mail,
                  },
              ]
            : []),
        ...(contactDetails.max.trim()
            ? [
                  {
                      title: 'MAX',
                      description: 'Напишите по вопросам бронирования и аренды.',
                      value: 'Написать в MAX',
                      href: contactDetails.max,
                      icon: MessageCircle,
                  },
              ]
            : []),
    ];

    return (
        <main className="min-h-screen bg-gray-50 pb-16 pt-24 text-gray-900 dark:bg-gray-950 dark:text-gray-100">
            <div className="container mx-auto max-w-6xl px-4 sm:px-6">
                <section className="relative mb-8 overflow-hidden rounded-3xl bg-white p-6 shadow-sm ring-1 ring-gray-200 dark:bg-gray-900 dark:ring-gray-800 sm:p-10 lg:p-14">
                    <div
                        aria-hidden="true"
                        className="pointer-events-none absolute -right-20 -top-20 h-64 w-64 rounded-full bg-primary-100/70 blur-3xl dark:bg-primary-900/20"
                    />
                    <div className="relative max-w-3xl">
                        <h1 className="text-3xl font-bold tracking-tight sm:text-4xl lg:text-5xl text-black dark:text-white">
                            Контакты
                        </h1>
                        <p className="mt-4 text-lg font-medium text-gray-700 dark:text-gray-200 sm:text-xl">
                            Остались вопросы по аренде?
                        </p>
                        <p className="mt-4 max-w-2xl leading-7 text-gray-600 dark:text-gray-300 sm:text-lg">
                            Свяжитесь с нами, чтобы уточнить условия, доступность
                            автомобиля и детали бронирования. Мы поможем разобраться в
                            вопросах до начала аренды.
                        </p>
                    </div>
                </section>

                {methods.length > 0 ? (
                    <section className="mb-8" aria-labelledby="contact-methods-heading">
                        <div className="mb-5 max-w-2xl">
                            <p className="text-sm font-semibold uppercase tracking-wider text-primary-700 dark:text-primary-300">
                                Мы всегда на связи
                            </p>
                        </div>

                        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                            {methods.map((method) => {
                                const Icon = method.icon;
                                const isExternal = method.href.startsWith('https://');

                                return (
                                    <article
                                        key={method.title}
                                        className="rounded-2xl bg-white p-6 shadow-sm ring-1 ring-gray-200 dark:bg-gray-900 dark:ring-gray-800"
                                    >
                                        <div className="mb-4 flex h-11 w-11 items-center justify-center rounded-xl bg-primary-50 text-primary-700 dark:bg-primary-950/50 dark:text-primary-300">
                                            <Icon className="h-5 w-5" aria-hidden="true" />
                                        </div>
                                        <h2 className="text-lg font-semibold text-gray-900 dark:text-white">
                                            {method.title}
                                        </h2>
                                        <p className="mt-2 leading-7 text-gray-600 dark:text-gray-300">
                                            {method.description}
                                        </p>
                                        <a
                                            href={method.href}
                                            {...(isExternal
                                                ? { target: '_blank', rel: 'noreferrer' }
                                                : {})}
                                            className="mt-4 inline-flex items-center gap-2 break-all text-sm font-semibold text-primary-700 transition-colors hover:text-primary-800 dark:text-primary-300 dark:hover:text-primary-200"
                                        >
                                            {method.value}
                                            <ArrowRight className="h-4 w-4 shrink-0" aria-hidden="true" />
                                        </a>
                                    </article>
                                );
                            })}
                        </div>
                    </section>
                ) : (
                    <section
                        className="mb-8 rounded-2xl border border-gray-200 bg-white p-6 dark:border-gray-800 dark:bg-gray-900 sm:p-8"
                        aria-labelledby="contact-details-heading"
                    >
                        <div className="flex items-start gap-4">
                            <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-primary-50 text-primary-700 dark:bg-primary-950/50 dark:text-primary-300">
                                <MessageCircle className="h-5 w-5" aria-hidden="true" />
                            </div>
                            <div>
                                <h2
                                    id="contact-details-heading"
                                    className="text-xl font-bold tracking-tight"
                                >
                                    Контактные данные
                                </h2>
                                <p className="mt-2 max-w-3xl leading-7 text-gray-600 dark:text-gray-300">
                                    Контактные данные арендодателя будут опубликованы здесь.
                                    Пока вы можете ознакомиться с автомобилями и условиями
                                    аренды на сайте.
                                </p>
                            </div>
                        </div>
                    </section>
                )}

                {(contactDetails.city.trim() || contactDetails.workingHours.trim()) && (
                    <section className="mb-8 grid gap-4 sm:grid-cols-2">
                        {contactDetails.city.trim() && (
                            <div className="flex items-start gap-4 rounded-2xl bg-white p-6 shadow-sm ring-1 ring-gray-200 dark:bg-gray-900 dark:ring-gray-800">
                                <MapPin className="mt-1 h-5 w-5 shrink-0 text-primary-700 dark:text-primary-300" aria-hidden="true" />
                                <div>
                                    <h2 className="font-semibold text-black dark:text-white">Мы находимся</h2>
                                    <p className="mt-1 leading-7 text-gray-600 dark:text-gray-300">
                                        {contactDetails.city}
                                    </p>
                                </div>
                            </div>
                        )}
                        {contactDetails.workingHours.trim() && (
                            <div className="flex items-start gap-4 rounded-2xl bg-white p-6 shadow-sm ring-1 ring-gray-200 dark:bg-gray-900 dark:ring-gray-800">
                                <Clock3 className="mt-1 h-5 w-5 shrink-0 text-primary-700 dark:text-primary-300" aria-hidden="true" />
                                <div>
                                    <h2 className="font-semibold text-black dark:text-white">Лучшее время для звонка</h2>
                                    <p className="mt-1 leading-7 text-gray-600 dark:text-gray-300">
                                        {contactDetails.workingHours}
                                    </p>
                                </div>
                            </div>
                        )}
                    </section>
                )}

                <section className="rounded-3xl bg-gray-100 p-6 dark:bg-gray-900 sm:p-8">
                    <div className="flex flex-col gap-5 sm:flex-row sm:items-center sm:justify-between">
                        <div className="max-w-2xl">
                            <h2 className="text-xl font-bold tracking-tight sm:text-2xl text-black dark:text-white">
                                Уже выбираете автомобиль?
                            </h2>
                            <p className="mt-2 leading-7 text-gray-600 dark:text-gray-300">
                                Посмотрите доступные автомобили или заранее ознакомьтесь с
                                правилами аренды.
                            </p>
                        </div>
                        <div className="flex flex-col gap-3 sm:flex-row">
                            <Link
                                to="/assets"
                                className="inline-flex items-center justify-center gap-2 rounded-xl bg-primary-600 px-5 py-3 text-sm font-semibold text-white transition-colors hover:bg-primary-700 focus:outline-none focus-visible:ring-2 focus-visible:ring-primary-500 focus-visible:ring-offset-2 dark:focus-visible:ring-offset-gray-900"
                            >
                                Выбрать автомобиль
                                <ArrowRight className="h-4 w-4" aria-hidden="true" />
                            </Link>
                            <Link
                                to="/rental-terms"
                                className="inline-flex items-center justify-center rounded-xl border border-gray-300 bg-white px-5 py-3 text-sm font-semibold text-gray-800 transition-colors hover:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-100 dark:hover:bg-gray-700"
                            >
                                Условия аренды
                            </Link>
                        </div>
                    </div>
                </section>
            </div>
        </main>
    );
};

export default ContactPage;
