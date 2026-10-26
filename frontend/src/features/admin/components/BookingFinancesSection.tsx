import { useCallback, useEffect, useMemo, useState } from "react";
import type { FormEvent, ReactNode } from "react";
import { Check, CircleDollarSign, Loader2, Plus, RefreshCw, X } from "lucide-react";
import { toast } from "react-hot-toast";
import { formatDateTime, formatPrice } from "@/shared/utils/formatters";
import type { BookingPriceDto, BookingServiceDto, BookingStatus } from "@/entities/booking/booking.types";
import {
    bookingFinanceApi,
    type BookingChargeDto,
    type BookingChargeSettingsDto,
    type BookingChargeStatus,
    type BookingChargeType,
    type BookingPaymentSummaryDto,
    type CreateBookingChargeRequest,
    type PaymentMethod,
    type PaymentTransactionDto,
} from "@/entities/booking/booking-finance.api";

interface Props {
    bookingId: string;
    price: BookingPriceDto;
    services: BookingServiceDto[];
    bookingStatus: BookingStatus;
}

type FinanceRow = {
    id: string;
    date: string | null;
    operation: string;
    details: string;
    status: string;
    amount: number;
    balance?: number;
    kind: "price" | "charge" | "payment";
    charge?: BookingChargeDto;
    payment?: PaymentTransactionDto;
};

const chargeTypeLabels: Record<BookingChargeType, string> = {
    LateCancellation: "Поздняя отмена",
    TrafficFine: "Штраф ГИБДД",
    Cleaning: "Уборка",
    Refueling: "Заправка",
    PropertyLossOrDamage: "Утрата имущества / повреждение",
    Other: "Прочее начисление",
};

const chargeStatusLabels: Record<BookingChargeStatus, string> = {
    PendingApproval: "На согласовании",
    Confirmed: "Подтверждено",
    Rejected: "Отклонено",
};

const paymentTypeLabels = {
    Prepayment: "Предоплата",
    AdditionalPayment: "Доплата",
    Refund: "Возврат",
} as const;

const paymentStatusLabels = {
    Pending: "Ожидает подтверждения",
    Completed: "Выполнено",
    Failed: "Ошибка",
    Cancelled: "Отменено",
} as const;

const inputClass =
    "w-full rounded-xl border border-gray-200 bg-white px-3 py-2.5 text-sm text-gray-900 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-100 dark:border-gray-700 dark:bg-gray-900 dark:text-white dark:focus:ring-blue-900/40";

const buttonClass =
    "inline-flex items-center justify-center gap-2 rounded-xl px-3 py-2 text-sm font-medium transition disabled:cursor-not-allowed disabled:opacity-50";

const emptyChargeForm = {
    type: "TrafficFine" as BookingChargeType,
    amount: "",
    quantity: "",
    title: "",
    description: "",
    referenceNumber: "",
};

export const BookingFinancesSection = ({ bookingId, price, services, bookingStatus }: Props) => {
    const [charges, setCharges] = useState<BookingChargeDto[]>([]);
    const [payments, setPayments] = useState<PaymentTransactionDto[]>([]);
    const [summary, setSummary] = useState<BookingPaymentSummaryDto | null>(null);
    const [loading, setLoading] = useState(true);
    const [busyId, setBusyId] = useState<string | null>(null);
    const [chargeModalOpen, setChargeModalOpen] = useState(false);
    const [settingsModalOpen, setSettingsModalOpen] = useState(false);
    const [lateCancellationTariff, setLateCancellationTariff] = useState("");
    const [refuelingPricePerLiter, setRefuelingPricePerLiter] = useState("");
    const [paymentModalOpen, setPaymentModalOpen] = useState(false);
    const [allocationPayment, setAllocationPayment] = useState<PaymentTransactionDto | null>(null);
    const [chargeForm, setChargeForm] = useState(emptyChargeForm);
    const [paymentAmount, setPaymentAmount] = useState("");
    const [paymentType, setPaymentType] = useState<"Prepayment" | "AdditionalPayment">("AdditionalPayment");
    const [paymentMethod, setPaymentMethod] = useState<PaymentMethod>("BankTransfer");
    const [paymentNote, setPaymentNote] = useState("");
    const [allocationAmounts, setAllocationAmounts] = useState<Record<string, string>>({});

    const loadFinance = useCallback(async () => {
        setLoading(true);
        try {
            const [chargesResult, paymentsResult, summaryResult, settingsResult] = await Promise.all([
                bookingFinanceApi.getCharges(bookingId),
                bookingFinanceApi.getTransactions(bookingId),
                bookingFinanceApi.getSummary(bookingId),
                bookingFinanceApi.getChargeSettings(),
            ]);
            setCharges(chargesResult);
            setPayments(paymentsResult);
            setSummary(summaryResult);
            setLateCancellationTariff(String(settingsResult.lateCancellationTariff));
            setRefuelingPricePerLiter(String(settingsResult.refuelingPricePerLiter));
        } catch (error) {
            console.error("Failed to load booking finance:", error);
            toast.error("Не удалось загрузить финансовые данные бронирования.");
        } finally {
            setLoading(false);
        }
    }, [bookingId]);

    useEffect(() => {
        void loadFinance();
    }, [loadFinance]);

    const runAction = async (key: string, action: () => Promise<unknown>, success: string): Promise<boolean> => {
        try {
            setBusyId(key);
            await action();
            toast.success(success);
            await loadFinance();
            return true;
        } catch (error) {
            console.error("Booking finance action failed:", error);
            toast.error("Не удалось выполнить финансовую операцию. Проверьте данные и статус бронирования.");
            return false;
        } finally {
            setBusyId(null);
        }
    };

    const submitSettings = async (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const lateTariff = Number(lateCancellationTariff);
        const refuelingPrice = Number(refuelingPricePerLiter);
        if (
            !Number.isFinite(lateTariff) || lateTariff < 0 ||
            !Number.isFinite(refuelingPrice) || refuelingPrice < 0 ||
            Number(lateTariff.toFixed(2)) !== lateTariff ||
            Number(refuelingPrice.toFixed(2)) !== refuelingPrice
        ) {
            toast.error("Тарифы должны быть неотрицательными суммами с двумя знаками после запятой.");
            return;
        }
        const succeeded = await runAction("charge-settings", () =>
            bookingFinanceApi.updateChargeSettings(lateTariff, refuelingPrice),
            "Тарифы начислений сохранены.",
        );
        if (succeeded) setSettingsModalOpen(false);
    };

    const submitCharge = async (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const amount = chargeForm.amount.trim() ? Number(chargeForm.amount) : undefined;
        const quantity = chargeForm.quantity.trim() ? Number(chargeForm.quantity) : undefined;

        if (amount !== undefined && (!Number.isFinite(amount) || amount <= 0)) {
            toast.error("Введите положительную сумму начисления.");
            return;
        }
        if (quantity !== undefined && (!Number.isFinite(quantity) || quantity <= 0)) {
            toast.error("Введите положительное количество литров.");
            return;
        }

        const request: CreateBookingChargeRequest = {
            bookingId,
            type: chargeForm.type,
            ...(amount !== undefined ? { amount } : {}),
            ...(quantity !== undefined ? { quantity } : {}),
            ...(chargeForm.title.trim() ? { title: chargeForm.title.trim() } : {}),
            ...(chargeForm.description.trim() ? { description: chargeForm.description.trim() } : {}),
            ...(chargeForm.referenceNumber.trim() ? { referenceNumber: chargeForm.referenceNumber.trim() } : {}),
        };

        const succeeded = await runAction("create-charge", () => bookingFinanceApi.createCharge(bookingId, request), "Начисление создано и отправлено на согласование.");
        if (succeeded) {
            setChargeModalOpen(false);
            setChargeForm(emptyChargeForm);
        }
    };

    const submitPayment = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const amount = Number(paymentAmount);
        if (!Number.isFinite(amount) || amount <= 0 || Number(amount.toFixed(2)) !== amount) {
            toast.error("Введите положительную сумму не более чем с двумя знаками после запятой.");
            return;
        }
        const succeeded = await runAction("register-payment", () => bookingFinanceApi.registerIncoming({
            bookingId,
            amount,
            type: paymentType,
            method: paymentMethod,
            note: paymentNote.trim() || null,
        }), "Платёж зарегистрирован.");
        if (succeeded) {
            setPaymentModalOpen(false);
            setPaymentAmount("");
            setPaymentNote("");
        }
    };

    const createRefund = async (payment: PaymentTransactionDto) => {
        const raw = window.prompt(
            `Доступный к возврату остаток по платежу: ${formatPrice(payment.amount)}. Введите сумму возврата:`,
        );
        if (raw === null) return;
        const amount = Number(raw.replace(",", "."));
        if (!Number.isFinite(amount) || amount <= 0 || Math.round(amount * 100) !== amount * 100) {
            toast.error("Введите положительную сумму не более чем с двумя знаками после запятой.");
            return;
        }
        await runAction(`refund-${payment.id}`, () => bookingFinanceApi.createRefund({
            originalPaymentId: payment.id,
            amount,
            note: "Возврат по бронированию",
        }), "Возврат зарегистрирован.");
    };

    const submitAllocations = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        if (!allocationPayment) return;
        const allocations = Object.entries(allocationAmounts)
            .map(([bookingChargeId, value]) => ({ bookingChargeId, amount: Number(value) }))
            .filter(item => valueIsPositive(item.amount));
        if (!allocations.length) {
            toast.error("Укажите сумму хотя бы для одного начисления.");
            return;
        }
        await runAction(`allocate-${allocationPayment.id}`, () =>
            bookingFinanceApi.allocatePayment(allocationPayment.id, allocations),
        "Платёж распределён по начислениям.");
        setAllocationPayment(null);
        setAllocationAmounts({});
    };

    const rows = useMemo<FinanceRow[]>(() => {
        const result: FinanceRow[] = [
            {
                id: "rental",
                date: null,
                operation: "Аренда",
                details: `Базовая стоимость ${price.rentalPrice.baseRentalPrice > price.rentalPrice.rentalPriceWithDiscount ? "(с учётом скидки)" : ""}`.trim(),
                status: "Расчёт",
                amount: price.rentalPrice.rentalPriceWithDiscount,
                kind: "price",
            },
        ];
        if (price.servicesPrice > 0) {
            result.push({
                id: "services",
                date: null,
                operation: "Дополнительные услуги",
                details: services.map(service => service.name).join(", ") || "Услуги по бронированию",
                status: "Расчёт",
                amount: price.servicesPrice,
                kind: "price",
            });
        }
        if (price.routeSurcharge > 0) {
            result.push({
                id: "route",
                date: null,
                operation: "Доплата за маршрут",
                details: "Дополнительная стоимость маршрута",
                status: "Расчёт",
                amount: price.routeSurcharge,
                kind: "price",
            });
        }
        if (price.deposit > 0) {
            result.push({ id: "deposit", date: null, operation: "Депозит", details: "Обеспечительный платёж; не входит в стоимость аренды", status: "Условие аренды", amount: price.deposit, kind: "price" });
        }
        for (const penalty of price.penalties ?? []) {
            result.push({
                id: `legacy-penalty-${penalty.id}`,
                date: null,
                operation: "Корректировка стоимости",
                details: penalty.type,
                status: "В расчёте",
                amount: penalty.amount,
                kind: "price",
            });
        }
        for (const charge of charges) {
            result.push({
                id: charge.id,
                date: charge.createdAt,
                operation: chargeTypeLabels[charge.type] ?? charge.title,
                details: [charge.title, charge.description, charge.referenceNumber, charge.quantity > 1 ? `${charge.quantity} ${charge.unit} × ${formatPrice(charge.unitPrice)}` : null].filter(Boolean).join(" · "),
                status: chargeStatusLabels[charge.status],
                amount: charge.amount,
                balance: charge.balance,
                kind: "charge",
                charge,
            });
        }
        for (const payment of payments) {
            result.push({
                id: payment.id,
                date: payment.createdAt,
                operation: paymentTypeLabels[payment.type],
                details: [
                    payment.method === "BankTransfer" ? "Банковский перевод" : "Онлайн",
                    payment.note,
                    payment.externalOperationId ? `Операция: ${payment.externalOperationId}` : null,
                ].filter(Boolean).join(" · "),
                status: paymentStatusLabels[payment.status],
                amount: payment.type === "Refund" ? -payment.amount : payment.amount,
                kind: "payment",
                payment,
            });
        }
        return result.sort((a, b) => {
            if (a.date === null && b.date !== null) return -1;
            if (a.date !== null && b.date === null) return 1;
            return (a.date ? new Date(a.date).getTime() : 0) - (b.date ? new Date(b.date).getTime() : 0);
        });
    }, [price, services, charges, payments]);

    const canRegisterPayment = bookingStatus === "Confirmed" || bookingStatus === "Active";
    const confirmedCharges = charges.filter(charge => charge.status === "Confirmed" && charge.balance > 0);
    const pendingCharges = charges.filter(charge => charge.status === "PendingApproval");

    return (
        <section className="rounded-2xl bg-white p-5 shadow-sm dark:bg-gray-800 sm:p-6 lg:col-span-2">
            <div className="mb-5 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                <div>
                    <h2 className="text-lg font-semibold text-gray-900 dark:text-white">Финансы бронирования</h2>
                    <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">
                        Стоимость, начисления и движение денежных средств в одной истории.
                    </p>
                </div>
                <div className="flex flex-wrap gap-2">
                    <button type="button" onClick={() => setPaymentModalOpen(true)} disabled={!canRegisterPayment} title={!canRegisterPayment ? "Платежи доступны только для подтверждённого или активного бронирования" : undefined} className={`${buttonClass} bg-blue-600 text-white hover:bg-blue-700`}>
                        <CircleDollarSign className="h-4 w-4" /> Добавить платёж
                    </button>
                    <button type="button" onClick={() => setChargeModalOpen(true)} className={`${buttonClass} border border-gray-200 text-gray-700 hover:bg-gray-50 dark:border-gray-700 dark:text-gray-200 dark:hover:bg-gray-700`}>
                        <Plus className="h-4 w-4" /> Добавить начисление
                    </button>
                    <button type="button" onClick={() => setSettingsModalOpen(true)} className={`${buttonClass} border border-gray-200 text-gray-700 hover:bg-gray-50 dark:border-gray-700 dark:text-gray-200 dark:hover:bg-gray-700`}>
                        Тарифы
                    </button>
                    <button type="button" onClick={() => void loadFinance()} disabled={loading} aria-label="Обновить финансы" className={`${buttonClass} border border-gray-200 text-gray-600 hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300`}>
                        <RefreshCw className={`h-4 w-4 ${loading ? "animate-spin" : ""}`} />
                    </button>
                </div>
            </div>

            <div className="mb-6 grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
                <SummaryCard label="Стоимость аренды" value={formatPrice(price.total)} hint="Без депозита" />
                <SummaryCard label="Поступило" value={formatPrice(summary?.totalReceived ?? 0)} hint="Подтверждённые платежи" />
                <SummaryCard label="Возвращено" value={formatPrice(summary?.totalRefunded ?? 0)} hint={summary ? `Ожидает возврата: ${formatPrice(summary.pendingRefunds)}` : "Загрузка…"} />
                <SummaryCard label="Чистая оплата" value={formatPrice(summary?.netPaid ?? 0)} hint={summary ? `Доступно к возврату: ${formatPrice(summary.availableToRefund)}` : "Загрузка…"} />
            </div>

            {pendingCharges.length > 0 && (
                <div className="mb-4 rounded-xl border border-amber-200 bg-amber-50 p-3 text-sm text-amber-900 dark:border-amber-900/60 dark:bg-amber-950/20 dark:text-amber-200">
                    На согласовании: {pendingCharges.length} {pendingCharges.length === 1 ? "начисление" : "начисления"}. До подтверждения они не увеличивают задолженность клиента.
                </div>
            )}

            <div className="overflow-x-auto rounded-xl border border-gray-200 dark:border-gray-700">
                <table className="min-w-[850px] w-full text-left text-sm">
                    <thead className="bg-gray-50 text-xs uppercase tracking-wide text-gray-500 dark:bg-gray-900/60 dark:text-gray-400">
                        <tr>
                            <th className="px-4 py-3 font-medium">Дата</th>
                            <th className="px-4 py-3 font-medium">Операция</th>
                            <th className="px-4 py-3 font-medium">Детали</th>
                            <th className="px-4 py-3 font-medium">Статус</th>
                            <th className="px-4 py-3 text-right font-medium">Сумма</th>
                            <th className="px-4 py-3 text-right font-medium">Остаток</th>
                            <th className="px-4 py-3 text-right font-medium">Действия</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-100 dark:divide-gray-700">
                        {rows.map(row => (
                            <tr key={row.id} className="align-top">
                                <td className="whitespace-nowrap px-4 py-3 text-gray-500 dark:text-gray-400">
                                    {row.date ? formatDateTime(new Date(row.date)) : "—"}
                                </td>
                                <td className="px-4 py-3 font-medium text-gray-900 dark:text-white">{row.operation}</td>
                                <td className="max-w-xs px-4 py-3 text-gray-500 dark:text-gray-400">{row.details || "—"}</td>
                                <td className="px-4 py-3">
                                    <span className={`inline-flex rounded-full px-2.5 py-1 text-xs font-medium ${statusClass(row.status)}`}>{row.status}</span>
                                </td>
                                <td className={`whitespace-nowrap px-4 py-3 text-right font-medium ${row.amount < 0 ? "text-green-600 dark:text-green-400" : "text-gray-900 dark:text-white"}`}>
                                    {row.amount < 0 ? "−" : ""}{formatPrice(Math.abs(row.amount))}
                                </td>
                                <td className="whitespace-nowrap px-4 py-3 text-right text-gray-600 dark:text-gray-300">
                                    {row.balance === undefined ? "—" : formatPrice(row.balance)}
                                </td>
                                <td className="px-4 py-3 text-right">
                                    {row.kind === "charge" && row.charge?.status === "PendingApproval" && (
                                        <div className="flex justify-end gap-1">
                                            <button type="button" title="Подтвердить начисление" disabled={busyId === row.id} onClick={() => void runAction(row.id, () => bookingFinanceApi.confirmCharge(bookingId, row.id), "Начисление подтверждено.")} className="rounded-lg p-2 text-green-700 hover:bg-green-50 disabled:opacity-50 dark:text-green-400 dark:hover:bg-green-950/30"><Check className="h-4 w-4" /></button>
                                            <button type="button" title="Отклонить начисление" disabled={busyId === row.id} onClick={() => void runAction(row.id, () => bookingFinanceApi.rejectCharge(bookingId, row.id), "Начисление отклонено.")} className="rounded-lg p-2 text-red-600 hover:bg-red-50 disabled:opacity-50 dark:text-red-400 dark:hover:bg-red-950/30"><X className="h-4 w-4" /></button>
                                        </div>
                                    )}
                                    {row.kind === "payment" && row.payment?.status === "Pending" && row.payment.type !== "Refund" && (
                                        <button type="button" disabled={busyId === row.id} onClick={() => void runAction(row.id, () => bookingFinanceApi.completeIncoming(row.id), "Платёж подтверждён.")} className="whitespace-nowrap rounded-lg px-2 py-1 text-xs font-medium text-blue-700 hover:bg-blue-50 disabled:opacity-50 dark:text-blue-300 dark:hover:bg-blue-950/30">Подтвердить</button>
                                    )}
                                    {row.kind === "payment" && row.payment?.status === "Pending" && row.payment.type === "Refund" && (
                                        <button type="button" disabled={busyId === row.id} onClick={() => void runAction(row.id, () => bookingFinanceApi.completeRefund(row.id), "Возврат подтверждён.")} className="whitespace-nowrap rounded-lg px-2 py-1 text-xs font-medium text-blue-700 hover:bg-blue-50 disabled:opacity-50 dark:text-blue-300 dark:hover:bg-blue-950/30">Подтвердить</button>
                                    )}
                                    {row.kind === "payment" && row.payment?.status === "Completed" && row.payment.type !== "Refund" && (
                                        <div className="flex flex-wrap justify-end gap-1">
                                            {confirmedCharges.length > 0 && (
                                                <button type="button" onClick={() => { setAllocationPayment(row.payment!); setAllocationAmounts({}); }} className="whitespace-nowrap rounded-lg px-2 py-1 text-xs font-medium text-blue-700 hover:bg-blue-50 dark:text-blue-300 dark:hover:bg-blue-950/30">Распределить</button>
                                            )}
                                            <button type="button" onClick={() => void createRefund(row.payment!)} className="whitespace-nowrap rounded-lg px-2 py-1 text-xs font-medium text-red-700 hover:bg-red-50 dark:text-red-300 dark:hover:bg-red-950/30">Возврат</button>
                                        </div>
                                    )}
                                </td>
                            </tr>
                        ))}
                        {!loading && rows.length === 0 && (
                            <tr><td colSpan={7} className="px-4 py-10 text-center text-sm text-gray-500 dark:text-gray-400">Финансовых операций пока нет.</td></tr>
                        )}
                    </tbody>
                </table>
            </div>

            {loading && <p className="mt-3 text-sm text-gray-500 dark:text-gray-400">Обновление финансовых данных…</p>}

            {settingsModalOpen && (
                <Modal title="Тарифы начислений" onClose={() => setSettingsModalOpen(false)}>
                    <form className="space-y-4" onSubmit={submitSettings}>
                        <Field label="Тариф за позднюю отмену, ₽">
                            <input className={inputClass} type="number" min="0" step="0.01" required value={lateCancellationTariff} onChange={event => setLateCancellationTariff(event.target.value)} />
                        </Field>
                        <Field label="Стоимость топлива за литр, ₽">
                            <input className={inputClass} type="number" min="0" step="0.01" required value={refuelingPricePerLiter} onChange={event => setRefuelingPricePerLiter(event.target.value)} />
                        </Field>
                        <p className="text-xs text-gray-500 dark:text-gray-400">Тарифы используются при создании новых начислений. Для поздней отмены и заправки сумма фиксируется в начислении на момент его создания.</p>
                        <div className="flex justify-end gap-2">
                            <button type="button" className="inline-flex items-center justify-center gap-2 rounded-xl border border-gray-200 px-3 py-2 text-sm font-medium text-gray-700 dark:border-gray-700 dark:text-gray-200" onClick={() => setSettingsModalOpen(false)}>Отмена</button>
                            <button type="submit" disabled={busyId === "charge-settings"} className="inline-flex items-center justify-center gap-2 rounded-xl bg-blue-600 px-3 py-2 text-sm font-medium text-white hover:bg-blue-700">{busyId === "charge-settings" && <Loader2 className="h-4 w-4 animate-spin" />} Сохранить</button>
                        </div>
                    </form>
                </Modal>
            )}

            {chargeModalOpen && (
                <Modal title="Новое начисление" onClose={() => setChargeModalOpen(false)}>
                    <form className="space-y-4" onSubmit={submitCharge}>
                        <Field label="Тип начисления">
                            <select className={inputClass} value={chargeForm.type} onChange={event => setChargeForm(prev => ({ ...prev, type: event.target.value as BookingChargeType }))}>
                                {Object.entries(chargeTypeLabels).map(([value, label]) => <option key={value} value={value} disabled={value === "LateCancellation" && bookingStatus !== "Cancelled"}>{label}</option>)}
                            </select>
                        </Field>
                        {chargeForm.type === "Refueling" ? (
                            <Field label="Количество литров">
                                <input className={inputClass} type="number" min="0.001" step="0.001" required value={chargeForm.quantity} onChange={event => setChargeForm(prev => ({ ...prev, quantity: event.target.value }))} />
                            </Field>
                        ) : chargeForm.type !== "LateCancellation" ? (
                            <Field label="Сумма, ₽">
                                <input className={inputClass} type="number" min="0.01" step="0.01" required value={chargeForm.amount} onChange={event => setChargeForm(prev => ({ ...prev, amount: event.target.value }))} />
                            </Field>
                        ) : (
                            <p className="rounded-lg bg-blue-50 p-3 text-sm text-blue-800 dark:bg-blue-950/30 dark:text-blue-200">Сумма будет взята из настроенного тарифа поздней отмены.</p>
                        )}
                        {chargeForm.type === "Other" && <Field label="Название"><input className={inputClass} required value={chargeForm.title} onChange={event => setChargeForm(prev => ({ ...prev, title: event.target.value }))} /></Field>}
                        {(chargeForm.type === "TrafficFine" || chargeForm.type === "Cleaning" || chargeForm.type === "PropertyLossOrDamage" || chargeForm.type === "Other") && (
                            <Field label={chargeForm.type === "TrafficFine" ? "Описание или номер постановления" : "Описание"}>
                                <textarea className={inputClass} required={chargeForm.type !== "TrafficFine"} rows={3} value={chargeForm.description} onChange={event => setChargeForm(prev => ({ ...prev, description: event.target.value }))} />
                            </Field>
                        )}
                        {chargeForm.type === "TrafficFine" && <Field label="Номер постановления (если есть)"><input className={inputClass} value={chargeForm.referenceNumber} onChange={event => setChargeForm(prev => ({ ...prev, referenceNumber: event.target.value }))} /></Field>}
                        <p className="text-xs text-gray-500 dark:text-gray-400">Новое начисление получает статус «На согласовании» и учитывается в долге только после подтверждения.</p>
                        <div className="flex justify-end gap-2">
                            <button type="button" className={`${buttonClass} border border-gray-200 text-gray-700 dark:border-gray-700 dark:text-gray-200`} onClick={() => setChargeModalOpen(false)}>Отмена</button>
                            <button type="submit" disabled={busyId === "create-charge"} className={`${buttonClass} bg-blue-600 text-white hover:bg-blue-700`}>{busyId === "create-charge" && <Loader2 className="h-4 w-4 animate-spin" />} Создать</button>
                        </div>
                    </form>
                </Modal>
            )}

            {paymentModalOpen && (
                <Modal title="Регистрация платежа" onClose={() => setPaymentModalOpen(false)}>
                    <form className="space-y-4" onSubmit={submitPayment}>
                        <Field label="Тип платежа">
                            <select className={inputClass} value={paymentType} onChange={event => setPaymentType(event.target.value as typeof paymentType)}>
                                <option value="Prepayment" disabled={bookingStatus !== "Confirmed"}>Предоплата</option>
                                <option value="AdditionalPayment">Доплата</option>
                            </select>
                        </Field>
                        <Field label="Сумма, ₽"><input className={inputClass} type="number" min="0.01" step="0.01" required value={paymentAmount} onChange={event => setPaymentAmount(event.target.value)} /></Field>
                        <Field label="Способ оплаты">
                            <select className={inputClass} value={paymentMethod} onChange={event => setPaymentMethod(event.target.value as PaymentMethod)}>
                                <option value="BankTransfer">Банковский перевод</option>
                                <option value="Online">Онлайн</option>
                            </select>
                        </Field>
                        <Field label="Комментарий (необязательно)"><textarea className={inputClass} rows={2} value={paymentNote} onChange={event => setPaymentNote(event.target.value)} /></Field>
                        <p className="text-xs text-gray-500 dark:text-gray-400">После регистрации платёж будет ожидать подтверждения. В сводке поступлений он появится после подтверждения.</p>
                        <div className="flex justify-end gap-2">
                            <button type="button" className={`${buttonClass} border border-gray-200 text-gray-700 dark:border-gray-700 dark:text-gray-200`} onClick={() => setPaymentModalOpen(false)}>Отмена</button>
                            <button type="submit" disabled={busyId === "register-payment"} className={`${buttonClass} bg-blue-600 text-white hover:bg-blue-700`}>{busyId === "register-payment" && <Loader2 className="h-4 w-4 animate-spin" />} Зарегистрировать</button>
                        </div>
                    </form>
                </Modal>
            )}

            {allocationPayment && (
                <Modal title="Распределение платежа" onClose={() => setAllocationPayment(null)}>
                    <form className="space-y-4" onSubmit={submitAllocations}>
                        <p className="text-sm text-gray-600 dark:text-gray-300">Платёж: {formatPrice(allocationPayment.amount)}. Укажите, какую сумму направить на каждое подтверждённое начисление. Общая сумма не должна превышать нераспределённый остаток платежа.</p>
                        {confirmedCharges.map(charge => (
                            <Field key={charge.id} label={`${charge.title} · остаток ${formatPrice(charge.balance)}`}>
                                <input className={inputClass} type="number" min="0" max={charge.balance} step="0.01" value={allocationAmounts[charge.id] ?? ""} onChange={event => setAllocationAmounts(prev => ({ ...prev, [charge.id]: event.target.value }))} />
                            </Field>
                        ))}
                        <div className="flex justify-end gap-2">
                            <button type="button" className={`${buttonClass} border border-gray-200 text-gray-700 dark:border-gray-700 dark:text-gray-200`} onClick={() => setAllocationPayment(null)}>Отмена</button>
                            <button type="submit" disabled={busyId === `allocate-${allocationPayment.id}`} className={`${buttonClass} bg-blue-600 text-white hover:bg-blue-700`}>Сохранить распределение</button>
                        </div>
                    </form>
                </Modal>
            )}
        </section>
    );
};

function valueIsPositive(value: number) {
    return Number.isFinite(value) && value > 0 && Math.round(value * 100) === value * 100;
}

function SummaryCard({ label, value, hint }: { label: string; value: string; hint: string }) {
    return (
        <div className="rounded-xl border border-gray-100 bg-gray-50 p-4 dark:border-gray-700 dark:bg-gray-900/40">
            <p className="text-sm text-gray-500 dark:text-gray-400">{label}</p>
            <p className="mt-1 text-xl font-semibold text-gray-900 dark:text-white">{value}</p>
            <p className="mt-1 text-xs text-gray-400 dark:text-gray-500">{hint}</p>
        </div>
    );
}

function Field({ label, children }: { label: string; children: ReactNode }) {
    return <label className="block space-y-1.5"><span className="text-sm font-medium text-gray-700 dark:text-gray-200">{label}</span>{children}</label>;
}

function Modal({ title, children, onClose }: { title: string; children: React.ReactNode; onClose: () => void }) {
    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4" role="presentation" onMouseDown={event => { if (event.target === event.currentTarget) onClose(); }}>
            <div role="dialog" aria-modal="true" aria-label={title} className="max-h-[90vh] w-full max-w-lg overflow-y-auto rounded-2xl bg-white p-5 shadow-xl dark:bg-gray-800 sm:p-6">
                <div className="mb-5 flex items-center justify-between gap-3">
                    <h3 className="text-lg font-semibold text-gray-900 dark:text-white">{title}</h3>
                    <button type="button" onClick={onClose} aria-label="Закрыть" className="rounded-lg p-2 text-gray-500 hover:bg-gray-100 dark:hover:bg-gray-700"><X className="h-4 w-4" /></button>
                </div>
                {children}
            </div>
        </div>
    );
}

function statusClass(status: string) {
    if (["Подтверждено", "Выполнено"].includes(status)) return "bg-green-100 text-green-800 dark:bg-green-950/40 dark:text-green-300";
    if (["Отклонено", "Ошибка", "Отменено"].includes(status)) return "bg-red-100 text-red-800 dark:bg-red-950/40 dark:text-red-300";
    if (["На согласовании", "Ожидает подтверждения"].includes(status)) return "bg-amber-100 text-amber-800 dark:bg-amber-950/40 dark:text-amber-300";
    return "bg-gray-100 text-gray-700 dark:bg-gray-700 dark:text-gray-200";
}
