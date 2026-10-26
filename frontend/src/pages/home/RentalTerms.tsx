import { useState } from 'react';
import { Link } from 'react-router-dom';
import { ArrowLeft, BadgeCheck, CalendarClock, CarFront, ChevronDown, ClipboardCheck, CreditCard, FileText, Gauge, ShieldCheck, Wallet, } from 'lucide-react';

type TermsSection = {
    id: string;
    title: string;
    icon: typeof FileText;
    paragraphs?: string[];
    bullets?: string[];
    subSections?: { title: string; bullets: string[] }[];
};

const sections: TermsSection[] = [
    {
        id: 'booking',
        title: 'Как работает аренда',
        icon: CalendarClock,
        paragraphs: [
            'Выберите автомобиль, даты поездки и дополнительные услуги. На странице бронирования вы увидите стоимость аренды, размер залога и условия для выбранной машины.',
            'Бронирование считается подтвержденным после внесения предоплаты в размере стоимости 1 суток аренды. Остальная сумма оплачивается после подписания договора аренды.',
        ],
    },
    {
        id: 'driver',
        title: 'Требования к водителю',
        icon: BadgeCheck,
        paragraphs: ['Для аренды автомобиля водитель должен соответствовать следующим требованиям:'],
        bullets: [
            'Возраст — не менее 25 лет.',
            'Водительский стаж — не менее 5 лет.',
            'Водительское удостоверение соответствующей категории, действительное на территории РФ.',
            'Предоставление документов, необходимых для оформления аренды.',
            'К управлению допускаются только водители, согласованные с арендодателем и указанные в договоре или заявке. Передавать управление другим лицам без согласования нельзя.',
        ],
    },
    {
        id: 'documents',
        title: 'Какие документы нужны',
        icon: FileText,
        subSections: [
            {
                title: 'Физическому лицу',
                bullets: [
                    'Паспорт.',
                    'Водительское удостоверение, действительное на территории РФ.',
                ],
            },
            {
                title: 'Индивидуальному предпринимателю (ИП)',
                bullets: [
                    'Паспорт.',
                    'Водительское удостоверение каждого водителя, допущенного к управлению.',
                    'Карточка с реквизитами ИП.',
                ],
            },
            {
                title: 'Юридическому лицу (ООО)',
                bullets: [
                    'Карточка организации с реквизитами.',
                    'Доверенность на подписание договора аренды, если договор подписывает представитель.',
                    'Доверенность на водителя, подтверждающая право управления автомобилем.',
                    'Паспорт водителя.',
                    'Водительское удостоверение, действительное на территории РФ.',
                ],
            },
        ],
        paragraphs: [
            'В отдельных случаях могут потребоваться дополнительные документы для подтверждения полномочий представителя и оформления договора.',
        ],
    },
    {
        id: 'deposit',
        title: 'Залог и его возврат',
        icon: Wallet,
        paragraphs: [
            'Размер залога зависит от автомобиля. Точная сумма указывается в карточке автомобиля или при оформлении бронирования.',
            'Залог возвращается после возврата автомобиля и проверки его состояния, а также после расчёта по обязательствам, предусмотренным договором.',
            'Автомобиль необходимо вернуть чистым, без новых повреждений (с учётом обычного износа и состояния, зафиксированного при выдаче) и с уровнем топлива не ниже уровня при получении.',
            'Если автомобиль возвращается в тёмное время суток или в загрязнённом состоянии и это не позволяет полноценно проверить его целостность, арендодатель вправе отложить окончательный расчёт и возврат залога до мойки и осмотра автомобиля.',
            'При наличии предусмотренных договором оснований из залога могут быть удержаны расходы и задолженность, связанные с арендой. Основания и размер удержаний сообщаются арендатору.',
        ],
    },
    {
        id: 'insurance',
        title: 'Страхование',
        icon: ShieldCheck,
        paragraphs: [
            'Для автомобилей предусмотрены ОСАГО и КАСКО. Условия и объём покрытия зависят от страховых договоров конкретного автомобиля.',
            'До начала аренды ознакомьтесь с применимыми условиями страхования: страховыми рисками и исключениями, франшизой (если она предусмотрена), ограничениями использования автомобиля и порядком действий при ДТП или другом страховом событии.',
            'Наличие ОСАГО и КАСКО не означает, что любой ущерб автоматически покрывается страховщиком. Ответственность определяется обстоятельствами происшествия, условиями страхования, договором аренды и применимым законодательством.',
        ],
    },
    {
        id: 'mileage',
        title: 'Лимит пробега',
        icon: Gauge,
        paragraphs: [
            'В стоимость включено 250 км пробега за каждые сутки аренды. Лимит суммируется за весь срок бронирования: неиспользованные километры за один день можно использовать в другой день аренды.',
            'При превышении общего лимита доплата составляет 15 ₽ за каждый дополнительный километр.',
            'Например, при аренде на 4 суток общий лимит составляет 1 000 км. Если за весь срок автомобиль проехал 1 150 км, превышение составит 150 км, а доплата — 2 250 ₽.',
            'Пробег определяется по разнице показаний одометра при передаче и возврате автомобиля. Показания рекомендуется фиксировать в акте и подтверждать фотографиями.',
        ],
    },
    {
        id: 'payment',
        title: 'Оплата и дополнительные расходы',
        icon: CreditCard,
        paragraphs: [
            'Стоимость аренды зависит от выбранного автомобиля, периода и дополнительных услуг. Условия и итоговая сумма сообщаются до подтверждения бронирования.',
            'В зависимости от ситуации дополнительно могут оплачиваться превышение лимита пробега, мойка и/или дозаправка, штрафы за нарушения в период аренды, а также расходы на устранение повреждений или утрату переданного имущества — если это предусмотрено договором.',
            'Применимые дополнительные платежи и основания их начисления определяются условиями бронирования и договором аренды.',
        ],
    },
    {
        id: 'handover',
        title: 'Получение и возврат автомобиля',
        icon: CarFront,
        paragraphs: [
            'Дата, время и место передачи и возврата автомобиля согласовываются при оформлении бронирования.',
            'При получении автомобиля проверьте его состояние, комплектность, уровень топлива и показания одометра. Замечания и уже имеющиеся повреждения следует зафиксировать в акте приёма-передачи и, по возможности, на фото или видео.',
            'При возврате проводится осмотр и сверка состояния автомобиля с данными, зафиксированными при выдаче. Вместе с автомобилем необходимо вернуть ключи, документы и дополнительное оборудование, если они передавались.',
            'О ДТП, повреждениях, неисправностях, угоне и других существенных происшествиях необходимо незамедлительно сообщить арендодателю и действовать по договору и требованиям законодательства.',
        ],
    },
    {
        id: 'changes',
        title: 'Изменение срока, продление и отмена',
        icon: ClipboardCheck,
        subSections: [
            {
                title: 'Изменение срока и продление',
                bullets: [
                    'Изменить первоначальный срок аренды можно по согласованию с арендодателем.',
                    'Продление нужно согласовать до окончания текущего срока. Дополнительный период оплачивается по применимому тарифу и в порядке, согласованном сторонами.',
                    'Досрочный возврат возможен. При сокращении срока уже начавшейся аренды расчёт производится с учётом фактического времени использования и оплаты дополнительных суток согласно условиям аренды.',
                ],
            },
            {
                title: 'Отмена бронирования клиентом',
                bullets: [
                    'Для подтверждения бронирования требуется предоплата в размере 1 суток аренды.',
                    'При отмене не позднее чем за 24 часа до начала бронирования предоплата возвращается полностью.',
                    'При отмене менее чем за 24 часа до начала аренды удерживается сумма, равная стоимости 1 суток аренды. Если внесённая предоплата была больше, оставшаяся часть возвращается клиенту.',
                ],
            },
            {
                title: 'Отмена со стороны арендодателя',
                bullets: [
                    'Если арендодатель отменяет бронирование или не может предоставить согласованный автомобиль, предоплата возвращается клиенту полностью.',
                ],
            },
        ],
    },
    {
        id: 'responsibility',
        title: 'Использование автомобиля и ответственность',
        icon: ShieldCheck,
        paragraphs: [
            'Во время аренды соблюдайте ПДД, бережно обращайтесь с автомобилем и используйте его по назначению.',
            'Нельзя передавать управление несогласованным водителям, управлять автомобилем в состоянии опьянения, использовать его для запрещённых договором целей или проводить ремонт и изменять конструкцию без согласования, кроме действий, необходимых в предусмотренных законом случаях.',
            'Арендатор обязан своевременно сообщать о ДТП, повреждениях, неисправностях и других существенных инцидентах. Ответственность за ущерб, штрафы, утрату ключей, документов или оборудования определяется договором аренды, обстоятельствами конкретного случая и применимым законодательством.',
        ],
    },
];

export const RentalTermsPage = () => {
    const [expandedSections, setExpandedSections] = useState<string[]>(['booking', 'changes']);

    const toggleSection = (id: string) => {
        setExpandedSections((current) =>
            current.includes(id) ? current.filter((item) => item !== id) : [...current, id],
        );
    };
    return (
        <main className="min-h-screen bg-gray-50 pb-16 pt-24 text-gray-900 dark:bg-gray-900 dark:text-gray-100">
            <div className="container mx-auto max-w-6xl px-4 sm:px-6">
                <section className="mb-8 rounded-3xl bg-white p-6 shadow-sm ring-1 ring-gray-200 dark:bg-gray-800 dark:ring-gray-800 sm:p-10">
                    <div className="max-w-3xl">
                        <h1 className="text-3xl font-bold tracking-tight sm:text-4xl text-black dark:text-white">
                            Условия аренды автомобилей
                        </h1>
                        <p className="mt-4 text-base leading-7 text-gray-600 dark:text-gray-300 sm:text-lg">
                            Главное об оформлении аренды, документах, залоге, пробеге и отмене бронирования — в одном месте.
                        </p>
                        <div className="mt-6 flex flex-wrap gap-3">
                            <Link
                                to="/assets"
                                className="inline-flex items-center justify-center rounded-xl bg-primary-600 px-5 py-3 text-sm font-semibold text-white transition-colors hover:bg-primary-700"
                            >
                                Выбрать автомобиль
                            </Link>
                            <a
                                href="#changes"
                                className="inline-flex items-center justify-center rounded-xl border border-gray-300 px-5 py-3 text-sm font-semibold text-gray-800 transition-colors hover:bg-gray-50 dark:border-gray-700 dark:text-gray-100 dark:hover:bg-gray-800"
                            >
                                Правила отмены
                            </a>
                        </div>
                    </div>
                </section>

                <section className="mb-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-4" aria-label="Основные условия">
                    <FactCard label="Возраст водителя" value="от 25 лет" icon={BadgeCheck} />
                    <FactCard label="Водительский стаж" value="от 5 лет" icon={CarFront} />
                    <FactCard label="Включённый пробег" value="250 км/сутки" icon={Gauge} />
                    <FactCard label="Сверх лимита" value="15 ₽/км" icon={Wallet} />
                </section>

                <section className="space-y-3" aria-label="Подробные условия аренды">
                    {sections.map((section) => {
                        const Icon = section.icon;
                        const isExpanded = expandedSections.includes(section.id);

                        return (
                            <article
                                key={section.id}
                                id={section.id}
                                className="scroll-mt-28 overflow-hidden rounded-2xl bg-white shadow-sm ring-1 ring-gray-200 dark:bg-gray-800 dark:ring-gray-800"
                            >
                                <button
                                    type="button"
                                    onClick={() => toggleSection(section.id)}
                                    aria-expanded={isExpanded}
                                    aria-controls={`${section.id}-content`}
                                    className="flex w-full items-center gap-4 px-5 py-5 text-left sm:px-6"
                                >
                                    <span className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-primary-50 text-primary-700 dark:bg-primary-950 dark:text-primary-300">
                                        <Icon className="h-5 w-5" aria-hidden="true" />
                                    </span>
                                    <span className="flex-1 text-base font-semibold sm:text-lg">{section.title}</span>
                                    <ChevronDown
                                        className={`h-5 w-5 shrink-0 text-gray-500 transition-transform ${isExpanded ? 'rotate-180' : ''}`}
                                        aria-hidden="true"
                                    />
                                </button>

                                {isExpanded && (
                                    <div id={`${section.id}-content`} className="space-y-4 px-5 pb-6 pl-5 sm:px-6 sm:pl-[5.75rem]">
                                        {section.paragraphs?.map((paragraph, index) => (
                                            <p key={`${section.id}-p-${index}`} className="leading-7 text-gray-600 dark:text-gray-300">
                                                {paragraph}
                                            </p>
                                        ))}

                                        {section.bullets && (
                                            <ul className="list-disc space-y-2 pl-5 leading-7 text-gray-600 dark:text-gray-300">
                                                {section.bullets.map((bullet) => <li key={bullet}>{bullet}</li>)}
                                            </ul>
                                        )}

                                        {section.subSections?.map((subSection) => (
                                            <div key={subSection.title} className="space-y-2">
                                                <h3 className="font-semibold text-gray-800 dark:text-white">{subSection.title}</h3>
                                                <ul className="list-disc space-y-2 pl-5 leading-7 text-gray-600 dark:text-gray-300">
                                                    {subSection.bullets.map((bullet) => <li key={bullet}>{bullet}</li>)}
                                                </ul>
                                            </div>
                                        ))}
                                    </div>
                                )}
                            </article>
                        );
                    })}
                </section>

                <section className="mt-8 rounded-3xl bg-gray-800 p-6 text-white dark:bg-gray-800 sm:p-8">
                    <div className="flex flex-col gap-5 sm:flex-row sm:items-center sm:justify-between">
                        <div>
                            <h2 className="text-xl font-bold sm:text-2xl text-white">Готовы выбрать автомобиль?</h2>
                            <p className="mt-2 max-w-2xl leading-6 text-gray-300">
                                Сравните доступные автомобили, стоимость аренды и условия конкретного предложения.
                            </p>
                        </div>
                        <Link
                            to="/assets"
                            className="inline-flex shrink-0 items-center justify-center rounded-xl bg-white px-5 py-3 text-sm font-semibold text-gray-900 transition-colors hover:bg-gray-100"
                        >
                            Перейти к автомобилям
                        </Link>
                    </div>
                </section>

                <p className="mt-6 text-sm leading-6 text-gray-500 dark:text-gray-400">
                    Информация на этой странице помогает ознакомиться с основными правилами. Индивидуальные условия выбранного автомобиля и бронирования фиксируются в соответствующих документах.
                </p>
            </div>
        </main>
    );
};

type FactCardProps = {
    label: string;
    value: string;
    icon: typeof FileText;
};

const FactCard = ({ label, value, icon: Icon }: FactCardProps) => (
    <div className="rounded-2xl bg-white p-5 shadow-sm ring-1 ring-gray-200 dark:bg-gray-800 dark:ring-gray-800">
        <div className="mb-4 flex h-10 w-10 items-center justify-center rounded-xl bg-primary-50 text-primary-700 dark:bg-primary-950 dark:text-primary-300">
            <Icon className="h-5 w-5" aria-hidden="true" />
        </div>
        <p className="text-sm text-gray-500 dark:text-gray-400">{label}</p>
        <p className="mt-1 text-lg font-bold">{value}</p>
    </div>
);

export default RentalTermsPage;