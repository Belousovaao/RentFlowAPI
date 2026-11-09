import { useMemo, useState } from 'react';
import { useI18n } from '@/app/providers/I18nProvider';
import { Link } from 'react-router-dom';
import { ArrowRight, ChevronDown, HelpCircle, FileText, Search, ShieldCheck,
} from 'lucide-react';

type FAQItem = {
    question: string;
    answer: string;
};

type FAQCategory = {
    id: string;
    title: string;
    description: string;
    items: FAQItem[];
};

const faqCategories: FAQCategory[] = [
    {
        id: 'booking',
        title: 'Бронирование',
        description: 'Оформление, изменение и отмена брони.',
        items: [
            {
                question: 'Как забронировать автомобиль?',
                answer:
                    'Выберите автомобиль, укажите даты аренды и ознакомьтесь с расчётом стоимости. Затем заполните необходимые данные и отправьте заявку. Если для оформления бронирования требуются дополнительные сведения или документы, мы сообщим об этом. Или позвоните нам по номеру, указанному в разделе Контакты, и мы все сделаем сами.',
            },
            {
                question: 'Когда бронирование считается подтверждённым?',
                answer:
                    'Бронирование считается подтвержденным после внесения предоплаты. Отправка заявки сама по себе не означает, что автомобиль уже забронирован. Дождитесь подтверждения бронирования. Статус заявки отображается в личном кабинете, ориентируйтесь на него и полученные уведомления.',
            },
            {
                question: 'Можно ли изменить даты аренды?',
                answer:
                    'Да, вы можете обратиться с просьбой изменить даты. Возможность продления или переноса зависит от доступности автомобиля и условий бронирования. До получения подтверждения не считайте изменение согласованным.',
            },
            {
                question: 'Как отменить бронирование?',
                answer:
                    'Обратитесь к нам или воспользуйтесь доступной функцией отмены в личном кабинете. При отмене бронирования за 24 часа и более до начала аренды, предоплата возвращается в полном объеме. При отмене позднее будет удержана предоплата в размере стоимости одних суток аренды.',
            },
        ],
    },
    {
        id: 'requirements',
        title: 'Документы и требования',
        description: 'Требования к водителю и документы для оформления аренды.',
        items: [
            {
                question: 'Какие требования предъявляются к водителю?',
                answer:
                    'Возраст водителя должен быть не менее 25 лет, а водительский стаж — не менее 5 лет. Необходимо действующее водительское удостоверение соответствующей категории, действительное на территории РФ. К управлению допускаются только водители, согласованные с арендодателем и указанные в договоре или заявке. Передавать управление другим лицам без согласования нельзя.',
            },
            {
                question: 'Какие документы нужны физическому лицу?',
                answer:
                    'Для оформления аренды на физическое лицо необходимы паспорт и водительское удостоверение, действительное на территории РФ.',
            },
            {
                question: 'Какие документы нужны индивидуальному предпринимателю (ИП)?',
                answer:
                    'Для оформления аренды на ИП необходимы паспорт, водительское удостоверение каждого водителя, допущенного к управлению, и карточка с реквизитами ИП.',
            },
            {
                question: 'Какие документы нужны юридическому лицу (ООО)?',
                answer:
                    'Для оформления аренды на ООО необходимы карточка организации с реквизитами, паспорт водителя и его водительское удостоверение, действительное на территории РФ. Если договор подписывает представитель, требуется доверенность на подписание договора аренды. Также необходима доверенность на водителя, подтверждающая право управления автомобилем.',
            },
            {
                question: 'Может ли автомобилем управлять другой человек?',
                answer:
                    'К управлению допускаются только водители, согласованные с арендодателем и указанные в договоре или заявке. Передавать управление другим лицам без согласования нельзя.',
            },
            {
                question: 'Можно ли арендовать автомобиль с водительским стажем менее 5 лет?',
                answer:
                    'Нет. Минимальный водительский стаж для аренды автомобиля составляет 5 лет. Возраст водителя должен быть не менее 25 лет.',
            },
        ],
    },
    {
        id: 'payment',
        title: 'Стоимость, оплата и залог',
        description: 'Из чего складывается цена аренды.',
        items: [
            {
                question: 'Из чего складывается стоимость аренды?',
                answer:
                    'Итоговая стоимость зависит от выбранного автомобиля, продолжительности аренды и применимых дополнительных услуг и условий. В отдельных случаях могут учитываться маршрутные ограничения и другие заранее оговорённые платежи. Перед подтверждением бронирования проверьте итоговый расчёт.',
            },
            {
                question: 'Нужно ли вносить залог?',
                answer:
                    'Необходимость залога и его размер определяются условиями аренды конкретного автомобиля. До оформления бронирования уточните сумму залога, порядок его внесения и условия возврата.',
            },
            {
                question: 'Кто оплачивает штрафы ГИБДД?',
                answer:
                    'Штрафы ГИБДД, полученные в период аренды в связи с использованием автомобиля арендатором, оплачивает арендатор.',
            },
            {
                question: 'Как оплачиваются штрафы ГИБДД, пришедшие после возврата автомобиля?',
                answer:
                    'Если штраф относится к нарушению, допущенному в период аренды, обязанность по его оплате сохраняется за арендатором, даже если постановление поступило после возврата автомобиля.',
            },
        ],
    },
    {
        id: 'rental',
        title: 'Получение, использование и возврат',
        description: 'Что важно знать во время аренды.',
        items: [
            {
                question: 'Как происходит получение автомобиля?',
                answer:
                    'Место, время и порядок передачи автомобиля согласовываются при бронировании. При получении необходимо выполнить предусмотренные условия оформления, проверить автомобиль и зафиксировать обнаруженные повреждения до начала поездки.',
            },
            {
                question: 'Что проверить перед началом поездки?',
                answer:
                    'Осмотрите кузов, стёкла, колёса и салон. Зафиксируйте на фото или видео все заметные повреждения и сообщите о них арендодателю до выезда. Также проверьте комплектность автомобиля, уровень топлива и наличие необходимых документов и принадлежностей.',
            },
            {
                question: 'Какой ежедневный пробег включён в стоимость аренды?',
                answer:
                    'В стоимость аренды включено 250 км пробега в сутки.',
            },
            {
                question: 'Куда можно поехать на арендованном автомобиле?',
                answer:
                    'Вы можете передвигаться по маршруту, выбранному при бронировании. Поездки за пределы согласованного маршрута не допускаются.',
            },
            {
                question: 'Можно ли изменить маршрут во время аренды?',
                answer:
                    'Изменение выбранного при бронировании маршрута без предварительного согласования с арендодателем запрещено.',
            },
            {
                question: 'Что делать при поломке или ДТП?',
                answer:
                    'При ДТП сначала обеспечьте безопасность людей и действуйте в соответствии с требованиями законодательства. При необходимости вызовите экстренные службы. Незамедлительно свяжитесь с арендодателем и следуйте согласованному порядку действий. Не организуйте ремонт и не принимайте решения, которые могут повлиять на урегулирование происшествия, без согласования, если ситуация не требует немедленных мер безопасности.',
            },
            {
                question: 'Как происходит возврат автомобиля?',
                answer:
                    'Верните автомобиль в согласованные дату, время и место. Перед передачей проверьте, что личные вещи извлечены из салона, а автомобиль находится в состоянии, предусмотренном условиями аренды (чистый, заправленный топливом до уровня не ниже, чем при выдаче, или вы можете выспользоваться услугами мойки/дозаправки по тарифам, указанным в договоре аренды). Совместно с арендодателем зафиксируйте состояние машины и завершение аренды. В случае, если автомобиль передается в грязном состоянии, арендатор оставляет за собой право отложить возврат депозита до мойки автомобиля и проверки его состояния.',
            },
        ],
    },
];


const localizedFaqCategories: Record<'en' | 'zh', FAQCategory[]> = {
    "en": [
        {
            "title": "Booking",
            "description": "Creating, changing and cancelling a reservation.",
            "items": [
                [
                    "How do I book a car?",
                    "Choose a car, enter the rental dates and review the price calculation. Then provide the required details and submit your request. If additional information or documents are needed, we will let you know. Alternatively, call us using the number on the Contact page and we will arrange everything for you."
                ],
                [
                    "When is a booking considered confirmed?",
                    "A booking is confirmed after the prepayment has been made. Submitting a request alone does not mean that the car has been reserved. Wait for booking confirmation. You can check the request status in your account and should rely on that status and the notifications you receive."
                ],
                [
                    "Can I change the rental dates?",
                    "Yes. Contact us to request a change of dates. An extension or rescheduling depends on vehicle availability and the booking terms. Do not consider the change approved until you receive confirmation."
                ],
                [
                    "How do I cancel a booking?",
                    "Contact us or use the cancellation option available in your account. If you cancel at least 24 hours before the rental starts, the prepayment is refunded in full. For cancellations made less than 24 hours before the start, an amount equal to one day's rental price will be retained from the prepayment."
                ]
            ]
        },
        {
            "title": "Documents and requirements",
            "description": "Driver requirements and documents needed to arrange a rental.",
            "items": [
                [
                    "What are the driver requirements?",
                    "The driver must be at least 25 years old and have at least 5 years of driving experience. A valid driving licence of the appropriate category, valid in the Russian Federation, is required. Only drivers approved by the lessor and named in the agreement or application may drive the vehicle. The vehicle must not be handed over to another driver without approval."
                ],
                [
                    "What documents does an individual need?",
                    "To rent as an individual, you need a passport and a driving licence valid in the Russian Federation."
                ],
                [
                    "What documents does an individual entrepreneur (IE) need?",
                    "To rent as an individual entrepreneur, you need a passport, the driving licence of every authorised driver, and the entrepreneur's company details card."
                ],
                [
                    "What documents does a legal entity (LLC) need?",
                    "An LLC must provide its company details card, the driver's passport and a driving licence valid in the Russian Federation. If a representative signs the agreement, a power of attorney authorising them to sign the rental agreement is required. A separate power of attorney authorising the driver to operate the vehicle is also required."
                ],
                [
                    "Can someone else drive the car?",
                    "Only drivers approved by the lessor and named in the agreement or application may drive the vehicle. You may not allow another person to drive without prior approval."
                ],
                [
                    "Can I rent a car with less than 5 years of driving experience?",
                    "No. The minimum driving experience required is 5 years. The driver must also be at least 25 years old."
                ]
            ]
        },
        {
            "title": "Price, payment and deposit",
            "description": "What makes up the rental price.",
            "items": [
                [
                    "How is the rental price calculated?",
                    "The total price depends on the selected vehicle, rental duration, and applicable additional services and terms. Route restrictions and other charges agreed in advance may also apply. Review the final calculation before confirming your booking."
                ],
                [
                    "Is a deposit required?",
                    "Whether a deposit is required and its amount depend on the rental terms for the selected vehicle. Before booking, clarify the deposit amount, how it must be paid and the conditions for its return."
                ],
                [
                    "Who pays traffic fines?",
                    "The renter is responsible for traffic fines incurred while the vehicle is being used during the rental period."
                ],
                [
                    "How are traffic fines received after the car has been returned handled?",
                    "If a fine relates to a violation committed during the rental period, the renter remains responsible for paying it even if the notice arrives after the vehicle has been returned."
                ]
            ]
        },
        {
            "title": "Vehicle pickup, use and return",
            "description": "What you should know during the rental period.",
            "items": [
                [
                    "How do I pick up the car?",
                    "The pickup location, time and handover procedure are agreed when booking. At pickup, complete the required paperwork, inspect the vehicle and document any existing damage before driving."
                ],
                [
                    "What should I check before driving?",
                    "Inspect the bodywork, windows, wheels and interior. Photograph or video any visible damage and notify the lessor before departure. Also check the vehicle's equipment, fuel level, and required documents and accessories."
                ],
                [
                    "How many kilometres per day are included in the rental price?",
                    "The rental price includes 250 km per day."
                ],
                [
                    "Where can I drive the rental car?",
                    "You may drive along the route selected when booking. Travel outside the agreed route is not permitted."
                ],
                [
                    "Can I change the route during the rental?",
                    "You may not change the route selected at booking without the lessor's prior approval."
                ],
                [
                    "What should I do in case of a breakdown or accident?",
                    "In the event of an accident, first ensure everyone's safety and follow applicable legal requirements. Contact emergency services if necessary. Notify the lessor immediately and follow the agreed procedure. Do not arrange repairs or make decisions that could affect the incident claim without approval, unless immediate action is required for safety."
                ],
                [
                    "How do I return the car?",
                    "Return the vehicle at the agreed date, time and location. Before handover, remove all personal belongings and ensure the vehicle meets the rental conditions: it must be clean and have at least the same fuel level as at pickup. Alternatively, you may use the washing or refuelling services at the rates specified in the rental agreement. Inspect the vehicle together with the lessor and record its condition and the end of the rental. If the vehicle is returned dirty, the lessor may postpone the deposit refund until the vehicle has been washed and its condition checked."
                ]
            ]
        }
    ],
    "zh": [
        {
            "title": "预订",
            "description": "如何提交、修改和取消预订。",
            "items": [
                [
                    "如何预订汽车？",
                    "选择汽车，填写租赁日期并查看费用明细。然后填写所需信息并提交申请。如需补充信息或文件，我们会通知您。您也可以拨打“联系方式”页面上的电话，我们可以协助您完成预订。"
                ],
                [
                    "什么情况下预订才算确认？",
                    "支付预付款后，预订才算确认。仅提交申请并不代表车辆已为您预留。请等待预订确认。您可以在个人账户中查看申请状态，并以该状态及收到的通知为准。"
                ],
                [
                    "可以更改租赁日期吗？",
                    "可以。请联系我们申请更改日期。是否可以延长或调整租期取决于车辆是否可用及预订条款。在收到确认之前，请勿认为更改已获批准。"
                ],
                [
                    "如何取消预订？",
                    "请联系我们，或使用个人账户中提供的取消功能。如果在租赁开始前至少24小时取消，预付款将全额退还。如果距离租赁开始不足24小时取消，将从预付款中扣留相当于一天租金的金额。"
                ]
            ]
        },
        {
            "title": "文件与要求",
            "description": "驾驶员要求及办理租赁所需的文件。",
            "items": [
                [
                    "驾驶员需要满足哪些条件？",
                    "驾驶员年龄须至少为25岁，驾龄须至少为5年。驾驶员必须持有在俄罗斯联邦境内有效且类别相符的驾驶证。只有经出租方批准并列入合同或申请的驾驶员才可以驾驶车辆。未经批准不得将车辆交给其他人驾驶。"
                ],
                [
                    "个人承租需要哪些文件？",
                    "个人办理租赁需要提供护照，以及在俄罗斯联邦境内有效的驾驶证。"
                ],
                [
                    "个体经营者（ИП）需要哪些文件？",
                    "个体经营者办理租赁需要提供护照、每位获准驾驶人员的驾驶证，以及个体经营者的详细信息卡。"
                ],
                [
                    "法人（有限责任公司）需要哪些文件？",
                    "有限责任公司须提供公司详细信息卡、驾驶员护照，以及在俄罗斯联邦境内有效的驾驶证。如果由代理人签署合同，还须提供授权其签署租赁合同的委托书。此外，还须提供授权驾驶员驾驶该车辆的委托书。"
                ],
                [
                    "可以由其他人驾驶汽车吗？",
                    "只有经出租方批准并列入合同或申请的驾驶员才可以驾驶车辆。未经事先批准，不得允许其他人驾驶。"
                ],
                [
                    "驾龄不足5年可以租车吗？",
                    "不可以。租车要求的最低驾龄为5年，驾驶员年龄还须至少为25岁。"
                ]
            ]
        },
        {
            "title": "费用、付款与押金",
            "description": "租赁费用由哪些部分构成。",
            "items": [
                [
                    "租赁费用如何计算？",
                    "总费用取决于所选车辆、租赁时长以及适用的附加服务和条款。某些情况下，还可能涉及路线限制及事先约定的其他费用。确认预订前，请核对最终费用明细。"
                ],
                [
                    "需要支付押金吗？",
                    "是否需要押金及押金金额取决于所租车辆的具体条款。预订前请确认押金金额、支付方式及退还条件。"
                ],
                [
                    "交通罚款由谁支付？",
                    "租赁期间因承租人使用车辆而产生的交通罚款由承租人承担。"
                ],
                [
                    "车辆归还后收到的交通罚款如何处理？",
                    "如果罚款对应的违规行为发生在租赁期间，即使罚单在车辆归还后才送达，承租人仍有义务支付。"
                ]
            ]
        },
        {
            "title": "取车、使用与还车",
            "description": "租赁期间需要了解的重要事项。",
            "items": [
                [
                    "如何办理取车？",
                    "取车地点、时间和交接流程将在预订时协商确定。取车时须完成规定手续，检查车辆，并在出发前记录已有损坏。"
                ],
                [
                    "出发前应该检查什么？",
                    "检查车身、车窗、车轮和内饰。对所有明显损坏拍照或录像，并在出发前告知出租方。同时检查车辆配置、燃油量以及必要的文件和随车用品。"
                ],
                [
                    "租金包含每天多少公里？",
                    "租金包含每天250公里的行驶里程。"
                ],
                [
                    "租赁汽车可以开到哪里？",
                    "您可以按照预订时选择的路线行驶。不得驶离双方约定的路线。"
                ],
                [
                    "租赁期间可以更改路线吗？",
                    "未经出租方事先同意，不得更改预订时选定的路线。"
                ],
                [
                    "车辆发生故障或交通事故怎么办？",
                    "发生事故时，首先确保人员安全，并按照法律规定处理。必要时联系紧急救援部门。请立即联系出租方并遵循双方约定的处理流程。未经同意，不要自行安排维修或作出可能影响事故处理的决定；为确保即时安全而必须采取的措施除外。"
                ],
                [
                    "如何归还汽车？",
                    "请在约定的日期、时间和地点归还车辆。交车前请取走个人物品，并确保车辆符合租赁条件：车辆应保持清洁，燃油量不得低于取车时的水平。您也可以按照租赁合同规定的费率使用洗车或加油服务。请与出租方共同检查车辆状况并记录租赁结束情况。如果车辆归还时未清洁，出租方有权在车辆清洗并完成车况检查后再退还押金。"
                ]
            ]
        }
    ]
};

const allQuestions = faqCategories.reduce(
    (total, category) => total + category.items.length,
    0,
);

export const FAQ = () => {
    const { language, t } = useI18n();
    const currentFaqCategories = language === 'ru' ? faqCategories : localizedFaqCategories[language];
    const [searchQuery, setSearchQuery] = useState('');
    const [activeCategory, setActiveCategory] = useState('all');

    const normalizedQuery = searchQuery.trim().toLocaleLowerCase(language === 'zh' ? 'zh-CN' : language === 'en' ? 'en-US' : 'ru-RU');

    const filteredCategories = useMemo(() => {
        return currentFaqCategories
            .filter((category) => activeCategory === 'all' || category.id === activeCategory,)
            .map((category) => ({
                ...category,
                items: category.items.filter((item) => {
                    if (!normalizedQuery) return true;

                    return `${item.question} ${item.answer}`
                        .toLocaleLowerCase(language === 'zh' ? 'zh-CN' : language === 'en' ? 'en-US' : 'ru-RU')
                        .includes(normalizedQuery);
                }),
            }))
            .filter((category) => category.items.length > 0);
    }, [activeCategory, normalizedQuery, currentFaqCategories, language]);

    const visibleCount = filteredCategories.reduce(
        (total, category) => total + category.items.length,
        0,
    );

    const resetFilters = () => {
        setSearchQuery('');
        setActiveCategory('all');
    };

    return (
        <main className="min-h-screen bg-gray-50 pb-16 pt-24 text-gray-900 dark:bg-gray-950 dark:text-gray-100">
            <div className="container mx-auto max-w-6xl px-4 sm:px-6">
                <Link
                    to="/"
                    className="mb-6 inline-flex items-center gap-2 text-sm font-medium text-gray-600 transition-colors hover:text-gray-950 dark:text-gray-400 dark:hover:text-white"
                >
                    <ArrowRight
                        className="h-4 w-4 rotate-180"
                        aria-hidden="true"
                    />
                    {t('faq.home')}
                </Link>

                <header className="relative mb-8 overflow-hidden rounded-3xl bg-white p-6 shadow-sm ring-1 ring-gray-200 dark:bg-gray-900 dark:ring-gray-800 sm:p-10 lg:p-14">
                    <div
                        aria-hidden="true"
                        className="pointer-events-none absolute -right-20 -top-20 h-64 w-64 rounded-full bg-primary-100/70 blur-3xl dark:bg-primary-900/20"
                    />

                    <div className="relative max-w-3xl">
                        <div className="mb-5 flex h-12 w-12 items-center justify-center rounded-2xl bg-primary-50 text-primary-700 dark:bg-primary-950/50 dark:text-primary-300">
                            <HelpCircle
                                className="h-6 w-6"
                                aria-hidden="true"
                            />
                        </div>

                        <h1 className="text-3xl font-bold tracking-tight text-gray-950 dark:text-white sm:text-4xl lg:text-5xl">
                            {t('faq.title')}
                        </h1>

                        <p className="mt-5 max-w-2xl text-base leading-7 text-gray-600 dark:text-gray-300 sm:text-lg">
                            {t('faq.intro')}
                        </p>
                    </div>
                </header>

                <section
                    className="mb-8 rounded-2xl bg-white p-5 shadow-sm ring-1 ring-gray-200 dark:bg-gray-900 dark:ring-gray-800 sm:p-6"
                    aria-label={t('faq.searchLabel')}
                >
                    <label
                        htmlFor="faq-search"
                        className="mb-3 block text-sm font-semibold text-gray-900 dark:text-white"
                    >
                        {t('faq.findAnswer')}
                    </label>

                    <div className="relative">
                        <Search
                            className="pointer-events-none absolute left-4 top-1/2 h-5 w-5 -translate-y-1/2 text-gray-400"
                            aria-hidden="true"
                        />

                        <input
                            id="faq-search"
                            type="search"
                            value={searchQuery}
                            onChange={(event) =>
                                setSearchQuery(event.target.value)
                            }
                            placeholder="{t('faq.searchPlaceholder')}"
                            className="w-full rounded-xl border border-gray-200 bg-white py-3 pl-12 pr-4 text-sm text-gray-900 outline-none transition placeholder:text-gray-400 focus:border-primary-500 focus:ring-2 focus:ring-primary-500/20 dark:border-gray-700 dark:bg-gray-950 dark:text-gray-100 dark:placeholder:text-gray-500"
                        />
                    </div>

                    <div
                        className="mt-5 flex gap-2 overflow-x-auto pb-1"
                        aria-label={t('faq.categories')}
                    >
                        <button
                            type="button"
                            onClick={() => setActiveCategory('all')}
                            aria-pressed={activeCategory === 'all'}
                            className={`shrink-0 rounded-full px-4 py-2 text-sm font-medium transition-colors ${
                                activeCategory === 'all'
                                    ? 'bg-primary-600 text-white'
                                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200 dark:bg-gray-800 dark:text-gray-300 dark:hover:bg-gray-700'
                            }`}
                        >
                            {t('faq.all')}
                        </button>

                        {currentFaqCategories.map((category) => (
                            <button
                                key={category.id}
                                type="button"
                                onClick={() => setActiveCategory(category.id)}
                                aria-pressed={activeCategory === category.id}
                                className={`shrink-0 rounded-full px-4 py-2 text-sm font-medium transition-colors ${
                                    activeCategory === category.id
                                        ? 'bg-primary-600 text-white'
                                        : 'bg-gray-100 text-gray-700 hover:bg-gray-200 dark:bg-gray-800 dark:text-gray-300 dark:hover:bg-gray-700'
                                }`}
                            >
                                {category.title}
                            </button>
                        ))}
                    </div>
                </section>

                <div className="space-y-8">
                    {filteredCategories.length > 0 ? (
                        filteredCategories.map((category) => (
                            <section
                                key={category.id}
                                id={category.id}
                                className="scroll-mt-28"
                                aria-labelledby={`faq-${category.id}`}
                            >
                                <div className="mb-4">
                                    <h2
                                        id={`faq-${category.id}`}
                                        className="text-xl font-bold tracking-tight text-gray-950 dark:text-white sm:text-2xl"
                                    >
                                        {category.title}
                                    </h2>
                                    <p className="mt-1 text-sm leading-6 text-gray-500 dark:text-gray-400">
                                        {category.description}
                                    </p>
                                </div>

                                <div className="divide-y divide-gray-100 overflow-hidden rounded-2xl bg-white shadow-sm ring-1 ring-gray-200 dark:divide-gray-800 dark:bg-gray-900 dark:ring-gray-800">
                                    {category.items.map((item, index) => (
                                        <details
                                            key={item.question}
                                            className="group"
                                        >
                                            <summary className="flex cursor-pointer list-none items-start justify-between gap-4 p-5 transition-colors hover:bg-gray-50 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-inset focus-visible:ring-primary-500 dark:hover:bg-gray-800/60 sm:p-6 [&::-webkit-details-marker]:hidden">
                                                <span className="flex items-start gap-3">
                                                    <span className="mt-0.5 flex h-7 w-7 shrink-0 items-center justify-center rounded-lg bg-primary-50 text-xs font-semibold text-primary-700 dark:bg-primary-950/50 dark:text-primary-300">
                                                        {index + 1}
                                                    </span>
                                                    <span className="font-semibold leading-7 text-gray-900 dark:text-gray-100">
                                                        {item.question}
                                                    </span>
                                                </span>

                                                <ChevronDown
                                                    className="mt-1 h-5 w-5 shrink-0 text-gray-400 transition-transform duration-200 group-open:rotate-180"
                                                    aria-hidden="true"
                                                />
                                            </summary>

                                            <div className="pb-5 pl-5 pr-5 sm:pb-6 sm:pl-16 sm:pr-8">
                                                <p className="text-sm leading-7 text-gray-600 dark:text-gray-300 sm:text-base">
                                                    {item.answer}
                                                </p>
                                            </div>
                                        </details>
                                    ))}
                                </div>
                            </section>
                        ))
                    ) : (
                        <section className="rounded-2xl bg-white p-8 text-center shadow-sm ring-1 ring-gray-200 dark:bg-gray-900 dark:ring-gray-800 sm:p-12">
                            <Search
                                className="mx-auto h-8 w-8 text-gray-400"
                                aria-hidden="true"
                            />
                            <h2 className="mt-4 text-lg font-bold text-gray-900 dark:text-white">
                                {t('faq.notFound')}
                            </h2>
                            <p className="mt-2 text-sm leading-6 text-gray-600 dark:text-gray-300">
                                {t('faq.noResults')}
                            </p>
                            <button
                                type="button"
                                onClick={resetFilters}
                                className="mt-5 inline-flex items-center justify-center rounded-xl bg-primary-600 px-5 py-3 text-sm font-semibold text-white transition-colors hover:bg-primary-700 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary-500 focus-visible:ring-offset-2 dark:focus-visible:ring-offset-gray-900"
                            >
                                {t('faq.showAll')}
                            </button>
                        </section>
                    )}
                </div>

                <section className="mt-10 rounded-3xl bg-gray-100 p-6 dark:bg-gray-900 sm:p-8">
                    <div className="flex flex-col gap-6 sm:flex-row sm:items-start">
                        <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-2xl bg-white text-primary-700 dark:bg-gray-800 dark:text-primary-300">
                            <ShieldCheck
                                className="h-6 w-6"
                                aria-hidden="true"
                            />
                        </div>

                        <div className="flex-1">
                            <h2 className="text-xl font-bold tracking-tight text-gray-950 dark:text-white sm:text-2xl">
                                {t('faq.moreQuestions')}
                            </h2>
                            <p className="mt-2 max-w-2xl text-sm leading-7 text-gray-600 dark:text-gray-300 sm:text-base">
                                {t('faq.contactText')}
                            </p>

                            <div className="mt-5 flex flex-col gap-3 sm:flex-row">
                                <Link
                                    to="/contact"
                                    className="inline-flex items-center justify-center gap-2 rounded-xl bg-primary-600 px-5 py-3 text-sm font-semibold text-white transition-colors hover:bg-primary-700 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary-500 focus-visible:ring-offset-2 dark:focus-visible:ring-offset-gray-900"
                                >
                                    {t('faq.contactUs')}
                                    <ArrowRight
                                        className="h-4 w-4"
                                        aria-hidden="true"
                                    />
                                </Link>

                                <Link
                                    to="/rental-terms"
                                    className="inline-flex items-center justify-center gap-2 rounded-xl border border-gray-300 bg-white px-5 py-3 text-sm font-semibold text-gray-800 transition-colors hover:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-100 dark:hover:bg-gray-700"
                                >
                                    <FileText
                                        className="h-4 w-4"
                                        aria-hidden="true"
                                    />
                                    {t('faq.rentalTerms')}
                                </Link>
                            </div>
                        </div>
                    </div>
                </section>
            </div>
        </main>
    );
};

export default FAQ;