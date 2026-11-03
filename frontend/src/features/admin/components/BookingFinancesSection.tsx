import { useCallback, useEffect, useMemo, useState } from "react";
import type { FormEvent, ReactNode } from "react";
import { CircleDollarSign, Loader2, Plus, RefreshCw, X } from "lucide-react";
import { toast } from "react-hot-toast";
import { useI18n } from "@/app/providers/I18nProvider";
import { formatDateTime, formatPrice } from "@/shared/utils/formatters";
import type { BookingPriceDto, BookingServiceDto, BookingStatus } from "@/entities/booking/booking.types";
import { bookingFinanceApi, type BookingChargeDto, type BookingChargeStatus, type BookingChargeType, type BookingPaymentSummaryDto, type CreateBookingChargeRequest, type PaymentMethod, type PaymentTransactionDto, } from "@/entities/booking/booking-finance.api";

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
    status?: BookingChargeStatus;
    amount: number;
    paidAmount?: number;
    balance?: number;
    kind: "charge" | "payment";
    charge?: BookingChargeDto;
    payment?: PaymentTransactionDto;
};

const chargeTypeLabels: Record<BookingChargeType, string> = {
    Rental: t("finance.charge.rental"),
    AdditionalServices: t("finance.charge.services"),
    RouteSurcharge: t("finance.charge.route"),
    Deposit: t("finance.charge.deposit"),
    LateCancellation: t("finance.charge.lateCancellation"),
    TrafficFine: t("finance.charge.trafficFine"),
    Cleaning: t("finance.charge.cleaning"),
    Refueling: t("finance.charge.refueling"),
    ExcessMileage: t("finance.charge.excessMileage"),
    PropertyLossOrDamage: t("finance.charge.damage"),
    Other: t("finance.charge.other"),
};

const paymentTypeLabels = {
    Prepayment: t("finance.payment.prepayment"),
    AdditionalPayment: t("finance.payment.additional"),
    Refund: t("finance.payment.refund"),
} as const;



const inputClass = "w-full rounded-xl border border-gray-200 bg-white px-3 py-2.5 text-sm text-gray-900 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-100 dark:border-gray-700 dark:bg-gray-900 dark:text-white dark:focus:ring-blue-900/40";
const buttonClass = "inline-flex items-center justify-center gap-2 rounded-xl px-3 py-2 text-sm font-medium transition disabled:cursor-not-allowed disabled:opacity-50";

const emptyChargeForm = {
    type: "TrafficFine" as BookingChargeType,
    amount: "",
    quantity: "",
    title: "",
    description: "",
    referenceNumber: "",
};

export const BookingFinancesSection = ({ bookingId, price, services, bookingStatus }: Props) => {
    const { t } = useI18n();
    const [charges, setCharges] = useState<BookingChargeDto[]>([]);
    const [payments, setPayments] = useState<PaymentTransactionDto[]>([]);
    const [summary, setSummary] = useState<BookingPaymentSummaryDto | null>(null);
    const [loading, setLoading] = useState(true);
    const [busyId, setBusyId] = useState<string | null>(null);
    const [chargeModalOpen, setChargeModalOpen] = useState(false);
    const [settingsModalOpen, setSettingsModalOpen] = useState(false);
    const [rentalTariffs, setRentalTariffs] = useState<Record<string, string>>({
        fuel_per_liter: "",
        excess_mileage_per_km: "",
        body_wash: "",
    });
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
                bookingFinanceApi.getRentalTariffs(),
            ]);
            setCharges(chargesResult);
            setPayments(paymentsResult);
            setSummary(summaryResult);
            setRentalTariffs(Object.fromEntries(settingsResult.map(tariff => [tariff.code, String(tariff.price)])));
        } catch (error) {
            console.error("Failed to load booking finance:", error);
            toast.error(t("finance.error.load"));
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
            toast.error(t("finance.error.action"));
            return false;
        } finally {
            setBusyId(null);
        }
    };

    const submitSettings = async (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const values = [
            ["fuel_per_liter", Number(rentalTariffs.fuel_per_liter), "л"],
            ["excess_mileage_per_km", Number(rentalTariffs.excess_mileage_per_km), "км"],
            ["body_wash", Number(rentalTariffs.body_wash), "услуга"],
        ] as const;
        if (values.some(([, value]) =>
            !Number.isFinite(value) || value < 0 || Number(value.toFixed(2)) !== value
        )) {
            toast.error(t("finance.error.tariffs"));
            return;
        }
        const succeeded = await runAction("rental-tariffs", async () => {
            for (const [code, value, unit] of values) {
                await bookingFinanceApi.updateRentalTariff(code, value, unit);
            }
        }, t("finance.success.tariffs"));
        if (succeeded) setSettingsModalOpen(false);
    };

    const submitCharge = async (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const amount = chargeForm.amount.trim() ? Number(chargeForm.amount) : undefined;
        const quantity = chargeForm.quantity.trim() ? Number(chargeForm.quantity) : undefined;

        if (amount !== undefined && (!Number.isFinite(amount) || amount <= 0 || Number(amount.toFixed(2)) !== amount)) {
            toast.error(t("finance.error.chargeAmount"));
            return;
        }
        if (quantity !== undefined && (!Number.isFinite(quantity) || quantity <= 0 || Number(quantity.toFixed(3)) !== quantity)) {
            toast.error(t("finance.error.quantity"));
            return;
        }

        if (chargeForm.type === "TrafficFine" && !chargeForm.description.trim() && !chargeForm.referenceNumber.trim()) {
            toast.error(t("finance.error.fineDetails"));
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

        const succeeded = await runAction("create-charge", () => bookingFinanceApi.createCharge(bookingId, request), t("finance.success.charge"));
        if (succeeded) {
            setChargeModalOpen(false);
            setChargeForm(emptyChargeForm);
        }
    };

    const submitPayment = async (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const amount = Number(paymentAmount);
        if (!Number.isFinite(amount) || amount <= 0 || Number(amount.toFixed(2)) !== amount) {
            toast.error(t("finance.error.paymentAmount"));
            return;
        }
        const succeeded = await runAction("register-payment", () => bookingFinanceApi.registerIncoming({
            bookingId,
            amount,
            type: paymentType,
            method: paymentMethod,
            note: paymentNote.trim() || null,
        }), t("finance.success.payment"));
        if (succeeded) {
            setPaymentModalOpen(false);
            setPaymentAmount("");
            setPaymentNote("");
        }
    };

    const createRefund = async (payment: PaymentTransactionDto) => {
        const maxRefund = Math.min(payment.availableToAllocate, summary?.availableToRefund ?? 0);
        if (maxRefund <= 0) {
            toast.error(t("finance.error.refundDebt"));
            return;
        }
        const raw = window.prompt(`Максимально доступно к возврату с учётом задолженности: ${formatPrice(maxRefund)}. Введите сумму возврата:`);
        if (raw === null) return;
        const amount = Number(raw.replace(",", "."));
        if (!Number.isFinite(amount) || amount <= 0 || amount > maxRefund || Number(amount.toFixed(2)) !== amount) {
            toast.error(`Введите сумму от 0,01 до ${formatPrice(maxRefund)} с точностью до двух знаков после запятой.`);
            return;
        }
        await runAction(`refund-${payment.id}`, () => bookingFinanceApi.createRefund({
            originalPaymentId: payment.id,
            amount,
            note: t("finance.refundNote"),
        }), t("finance.success.refund"));
    };

    const submitAllocations = async (event: FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        if (!allocationPayment) return;
        const allocations = Object.entries(allocationAmounts)
            .map(([bookingChargeId, value]) => ({ bookingChargeId, amount: Number(value) }))
            .filter(item => valueIsPositive(item.amount));
        if (!allocations.length) {
            toast.error(t("finance.error.allocation"));
            return;
        }
        const succeeded = await runAction(`allocate-${allocationPayment.id}`, () =>
            bookingFinanceApi.allocatePayment(allocationPayment.id, allocations), t("finance.success.allocation"));
        if (succeeded) {
            setAllocationPayment(null);
            setAllocationAmounts({});
        }
    };

    const rows = useMemo<FinanceRow[]>(() =>
        charges.map(charge => ({
            id: charge.id,
            date: charge.createdAt,
            operation: chargeTypeLabels[charge.type] ?? charge.title,
            // Details contain the admin-entered description, not the operation title.
            details: [
                charge.description,
                charge.referenceNumber,
                charge.quantity > 1 ? `${charge.quantity} ${charge.unit} × ${formatPrice(charge.unitPrice)}` : null,
            ].filter(Boolean).join(" · "),
            status: charge.status,
            amount: charge.amount,
            paidAmount: charge.paidAmount,
            balance: charge.status === "Accrued" ? charge.balance : 0,
            kind: "charge" as const,
            charge,
        })).sort((a, b) =>
            (a.date ? new Date(a.date).getTime() : 0) -
            (b.date ? new Date(b.date).getTime() : 0)
        ), [charges]);


    const canRegisterPayment = bookingStatus === "Confirmed" || bookingStatus === "Active" || bookingStatus === "Completed";
    const accruedCharges = charges.filter(charge => charge.status === "Accrued" && charge.balance > 0);
    
    return (
        <section className="rounded-2xl bg-white p-5 shadow-sm dark:bg-gray-800 sm:p-6 lg:col-span-2">
            <div className="mb-5 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                <div>
                    <h2 className="text-lg font-semibold text-gray-900 dark:text-white">{t("finance.title")}</h2>
                </div>
                <div className="flex flex-wrap gap-2">
                    <button 
                        type="button" 
                        onClick={() => setPaymentModalOpen(true)} 
                        disabled={!canRegisterPayment} 
                        title={!canRegisterPayment 
                            ? t("finance.payment.disabled") 
                            : undefined} 
                        className={`${buttonClass} bg-blue-600 text-white hover:bg-blue-700`}>
                        <CircleDollarSign className="h-4 w-4" /> 
                            Добавить платёж
                    </button>
                    <button 
                        type="button" 
                        onClick={() => setChargeModalOpen(true)} 
                        className={`${buttonClass} border border-gray-200 text-gray-700 hover:bg-gray-50 dark:border-gray-700 dark:text-gray-200 dark:hover:bg-gray-700`}>
                        <Plus className="h-4 w-4" /> 
                            Добавить начисление
                    </button>
                    <button 
                        type="button" 
                        onClick={() => setSettingsModalOpen(true)} 
                        className={`${buttonClass} border border-gray-200 text-gray-700 hover:bg-gray-50 dark:border-gray-700 dark:text-gray-200 dark:hover:bg-gray-700`}>
                            Тарифы
                    </button>
                    <button 
                        type="button" 
                        onClick={() => void loadFinance()} 
                        disabled={loading} 
                        aria-label=t("finance.refresh") 
                        className={`${buttonClass} border border-gray-200 text-gray-600 hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300`}>
                        <RefreshCw className={`h-4 w-4 ${loading ? "animate-spin" : ""}`} />
                    </button>
                </div>
            </div>

            <div className="mb-6 grid gap-3 sm:grid-cols-2 xl:grid-cols-5">
                <SummaryCard label=t("finance.rentalCost") value={formatPrice(price.total)} hint=t("finance.withoutDeposit") />
                <SummaryCard label=t("finance.totalWithDeposit") value={formatPrice(price.totalToPay)} hint="" />
                <SummaryCard label=t("finance.received") value={formatPrice(summary?.totalReceived ?? 0)} hint="" />
                <SummaryCard label=t("finance.refunded") value={formatPrice(summary?.totalRefunded ?? 0)} hint="" />
                <SummaryCard label=t("finance.netPaid") value={formatPrice(summary?.netPaid ?? 0)} hint="" />
            </div>



            <div className="overflow-x-auto rounded-xl border border-gray-200 dark:border-gray-700">
                <table className="min-w-[1000px] w-full text-left text-sm">
                    <thead className="bg-gray-50 text-xs uppercase tracking-wide text-gray-500 dark:bg-gray-900/60 dark:text-gray-400">
                        <tr>
                            <th className="px-4 py-3 font-medium">{t("common.date")}</th>
                            <th className="px-4 py-3 font-medium">{t("finance.operation")}</th>
                            <th className="px-4 py-3 font-medium">{t("common.details")}</th>
                            <th className="px-4 py-3 text-right font-medium">{t("finance.accrued")}</th>
                            <th className="px-4 py-3 text-right font-medium">{t("finance.paid")}</th>
                            <th className="px-4 py-3 text-right font-medium">{t("finance.balance")}</th>
                            <th className="px-4 py-3 text-right font-medium">{t("common.actions")}</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-100 dark:divide-gray-700">
                        {rows.map(row => (
                            <tr key={row.id} className="align-top">
                                <td className="whitespace-nowrap px-4 py-3 text-gray-500 dark:text-gray-400">
                                    {row.date ? formatDateTime(new Date(row.date)) : "—"}
                                </td>
                                <td className="px-4 py-3 font-medium text-gray-900 dark:text-white">
                                    <span className="inline-flex items-center gap-1.5">
                                        {row.status === "Rejected" && <X className="h-4 w-4 shrink-0 text-red-500" aria-label=t("finance.chargeRejected") />}
                                        {row.operation}
                                    </span>
                                </td>
                                <td className="max-w-xs px-4 py-3 text-gray-500 dark:text-gray-400">{row.details || "—"}</td>
                                <td className="whitespace-nowrap px-4 py-3 text-right font-medium text-gray-900 dark:text-white">{formatPrice(row.amount)}</td>
                                <td className="whitespace-nowrap px-4 py-3 text-right text-gray-600 dark:text-gray-300">{formatPrice(row.paidAmount ?? 0)}</td>
                                <td className="whitespace-nowrap px-4 py-3 text-right text-gray-600 dark:text-gray-300">{formatPrice(row.balance ?? 0)}</td>
                                <td className="px-4 py-3 text-right">
                                    {row.kind === "charge" && row.charge?.status === "Accrued" && row.charge.type !== "Rental" && (
                                        <div className="flex justify-end gap-1">
                                            <button
                                                type="button"
                                                title="Отклонить начисление"
                                                disabled={busyId === row.id}
                                                onClick={() => {
                                                    const paidAmount = row.charge?.paidAmount ?? 0;
                                                    const message = paidAmount > 0
                                                        ? `Отклонить начисление «${row.charge?.title}» на ${formatPrice(row.charge?.amount ?? 0)}? Распределённая оплата ${formatPrice(paidAmount)} будет возвращена в нераспределённый остаток платежа.`
                                                        : `Отклонить начисление «${row.charge?.title}» на ${formatPrice(row.charge?.amount ?? 0)}?`;
                                                    if (window.confirm(message)) {
                                                        void runAction(row.id, () => bookingFinanceApi.rejectCharge(bookingId, row.id), "Начисление отклонено. Распределённая оплата освобождена.");
                                                    }
                                                }}
                                                className="rounded-lg p-2 text-red-600 hover:bg-red-50 disabled:opacity-50 dark:text-red-400 dark:hover:bg-red-950/30"
                                            ><X className="h-4 w-4" /></button>
                                        </div>
                                    )}

                                </td>
                            </tr>
                        ))}
                        {!loading && rows.length === 0 && (
                            <tr><td colSpan={7} className="px-4 py-10 text-center text-sm text-gray-500 dark:text-gray-400">{t("finance.empty")}</td></tr>
                        )}
                    </tbody>
                </table>
            </div>

            <div className="mt-5 rounded-xl border border-gray-200 p-4 dark:border-gray-700">
                <div className="mb-3 flex items-center justify-between gap-3">
                    <h3 className="font-medium text-gray-900 dark:text-white">{t("finance.incomingPayments")}</h3>
                </div>
                {payments.filter(payment => payment.type !== "Refund" && payment.availableToAllocate > 0).length === 0 ? (
                    <p className="text-sm text-gray-500 dark:text-gray-400">{t("finance.noUnallocated")}</p>
                ) : (
                    <div className="space-y-2">
                        {payments.filter(payment => payment.type !== "Refund" && payment.availableToAllocate > 0).map(payment => (
                            <div key={payment.id} className="flex flex-col gap-2 rounded-lg bg-gray-50 p-3 dark:bg-gray-900/40 sm:flex-row sm:items-center sm:justify-between">
                                <div>
                                    <p className="text-sm font-medium text-gray-900 dark:text-white">{paymentTypeLabels[payment.type]} · доступно {formatPrice(payment.availableToAllocate)}</p>
                                    <p className="text-xs text-gray-500 dark:text-gray-400">Поступило {formatPrice(payment.amount)} · {formatDateTime(new Date(payment.createdAt))}{payment.note ? ` · ${payment.note}` : ""}</p>
                                </div>
                                <div className="flex gap-2">
                                    {accruedCharges.length > 0 && (
                                        <button type="button" onClick={() => { setAllocationPayment(payment); setAllocationAmounts({}); }} className="whitespace-nowrap rounded-lg px-2 py-1 text-xs font-medium text-blue-700 hover:bg-blue-100 dark:text-blue-300 dark:hover:bg-blue-950/30">{t("finance.allocate")}</button>
                                    )}
                                    <button type="button" onClick={() => void createRefund(payment)} className="whitespace-nowrap rounded-lg px-2 py-1 text-xs font-medium text-red-700 hover:bg-red-100 dark:text-red-300 dark:hover:bg-red-950/30">{t("finance.payment.refund")}</button>
                                </div>
                            </div>
                        ))}
                    </div>
                )}
            </div>

            {loading && <p className="mt-3 text-sm text-gray-500 dark:text-gray-400">{t("finance.updating")}</p>}

            {settingsModalOpen && (
                <Modal title=t("finance.tariffSettings") onClose={() => setSettingsModalOpen(false)}>
                    <form className="space-y-4" onSubmit={submitSettings}>
                        <Field label=t("finance.fuelTariff")>
                            <input className={inputClass} type="number" min="0" step="0.01" required value={rentalTariffs.fuel_per_liter ?? ""} onChange={event => setRentalTariffs(prev => ({ ...prev, fuel_per_liter: event.target.value }))} />
                        </Field>
                        <Field label=t("finance.mileageTariff")>
                            <input className={inputClass} type="number" min="0" step="0.01" required value={rentalTariffs.excess_mileage_per_km ?? ""} onChange={event => setRentalTariffs(prev => ({ ...prev, excess_mileage_per_km: event.target.value }))} />
                        </Field>
                        <Field label=t("finance.washTariff")>
                            <input className={inputClass} type="number" min="0" step="0.01" required value={rentalTariffs.body_wash ?? ""} onChange={event => setRentalTariffs(prev => ({ ...prev, body_wash: event.target.value }))} />
                        </Field>
                        <p className="text-xs text-gray-500 dark:text-gray-400">{t("finance.tariffHint")}</p>
                        <div className="flex justify-end gap-2">
                            <button type="button" className="inline-flex items-center justify-center gap-2 rounded-xl border border-gray-200 px-3 py-2 text-sm font-medium text-gray-700 dark:border-gray-700 dark:text-gray-200" onClick={() => setSettingsModalOpen(false)}>{t("common.cancel")}</button>
                            <button type="submit" disabled={busyId === "charge-settings"} className="inline-flex items-center justify-center gap-2 rounded-xl bg-blue-600 px-3 py-2 text-sm font-medium text-white hover:bg-blue-700">{busyId === "charge-settings" && <Loader2 className="h-4 w-4 animate-spin" />} Сохранить</button>
                        </div>
                    </form>
                </Modal>
            )}

            {chargeModalOpen && (
                <Modal title=t("finance.newCharge") onClose={() => setChargeModalOpen(false)}>
                    <form className="space-y-4" onSubmit={submitCharge}>
                        <Field label=t("finance.chargeType")>
                            <select className={inputClass} value={chargeForm.type} onChange={event => setChargeForm(prev => ({ ...prev, type: event.target.value as BookingChargeType }))}>
                                {Object.entries(chargeTypeLabels).filter(([value]) => !["Rental", "AdditionalServices", "RouteSurcharge", "Deposit"].includes(value)).map(([value, label]) => <option key={value} value={value} disabled={value === "LateCancellation" && bookingStatus !== "Cancelled"}>{label}</option>)}
                            </select>
                        </Field>
                        {chargeForm.type === "Refueling" || chargeForm.type === "ExcessMileage" ? (
                            <Field label={chargeForm.type === "Refueling" ? t("finance.liters") : t("finance.kilometers")}>
                                <input className={inputClass} type="number" min="0.001" step="0.001" required value={chargeForm.quantity} onChange={event => setChargeForm(prev => ({ ...prev, quantity: event.target.value }))} />
                            </Field>
                        ) : chargeForm.type !== "LateCancellation" ? (
                            <Field label=t("finance.amountRub")>
                                <input className={inputClass} type="number" min="0.01" step="0.01" required={chargeForm.type !== "Cleaning"} value={chargeForm.amount} onChange={event => setChargeForm(prev => ({ ...prev, amount: event.target.value }))} />
                                {chargeForm.type === "Cleaning" && <span className="text-xs text-gray-500">{t("finance.washHint")}</span>}
                            </Field>
                        ) : (
                            <p className="rounded-lg bg-blue-50 p-3 text-sm text-blue-800 dark:bg-blue-950/30 dark:text-blue-200">{t("finance.chargePriceHint")}</p>
                        )}
                        {chargeForm.type === "Other" && <Field label=t("common.title")><input className={inputClass} required value={chargeForm.title} onChange={event => setChargeForm(prev => ({ ...prev, title: event.target.value }))} /></Field>}
                        {(chargeForm.type === "TrafficFine" || chargeForm.type === "Cleaning" || chargeForm.type === "PropertyLossOrDamage" || chargeForm.type === "Other") && (
                            <Field label={chargeForm.type === "TrafficFine" ? t("finance.fineDescription") : t("common.description")}>
                                <textarea className={inputClass} required={chargeForm.type === "Cleaning" || chargeForm.type === "PropertyLossOrDamage"} rows={3} value={chargeForm.description} onChange={event => setChargeForm(prev => ({ ...prev, description: event.target.value }))} />
                            </Field>
                        )}
                        {chargeForm.type === "TrafficFine" && <Field label=t("finance.fineNumber")><input className={inputClass} value={chargeForm.referenceNumber} onChange={event => setChargeForm(prev => ({ ...prev, referenceNumber: event.target.value }))} /></Field>}
                        <p className="text-xs text-gray-500 dark:text-gray-400">{t("finance.chargeHint")}</p>
                        <div className="flex justify-end gap-2">
                            <button type="button" className={`${buttonClass} border border-gray-200 text-gray-700 dark:border-gray-700 dark:text-gray-200`} onClick={() => setChargeModalOpen(false)}>{t("common.cancel")}</button>
                            <button type="submit" disabled={busyId === "create-charge"} className={`${buttonClass} bg-blue-600 text-white hover:bg-blue-700`}>{busyId === "create-charge" && <Loader2 className="h-4 w-4 animate-spin" />} Создать</button>
                        </div>
                    </form>
                </Modal>
            )}

            {paymentModalOpen && (
                <Modal title=t("finance.registerPayment") onClose={() => setPaymentModalOpen(false)}>
                    <form className="space-y-4" onSubmit={submitPayment}>
                        <Field label=t("finance.paymentType")>
                            <select className={inputClass} value={paymentType} onChange={event => setPaymentType(event.target.value as typeof paymentType)}>
                                <option value="Prepayment" disabled={bookingStatus !== "Confirmed"}>{t("finance.payment.prepayment")}</option>
                                <option value="AdditionalPayment">Доплата</option>
                            </select>
                        </Field>
                        <Field label=t("finance.amountRub")><input className={inputClass} type="number" min="0.01" step="0.01" required value={paymentAmount} onChange={event => setPaymentAmount(event.target.value)} /></Field>
                        <Field label=t("finance.paymentMethod")>
                            <select className={inputClass} value={paymentMethod} onChange={event => setPaymentMethod(event.target.value as PaymentMethod)}>
                                <option value="BankTransfer">{t("finance.bankTransfer")}</option>
                                <option value="Online">{t("finance.online")}</option>
                            </select>
                        </Field>
                        <Field label=t("common.commentOptional")><textarea className={inputClass} rows={2} value={paymentNote} onChange={event => setPaymentNote(event.target.value)} /></Field>
                        <p className="text-xs text-gray-500 dark:text-gray-400">{t("finance.paymentHint")}</p>
                        <div className="flex justify-end gap-2">
                            <button type="button" className={`${buttonClass} border border-gray-200 text-gray-700 dark:border-gray-700 dark:text-gray-200`} onClick={() => setPaymentModalOpen(false)}>{t("common.cancel")}</button>
                            <button type="submit" disabled={busyId === "register-payment"} className={`${buttonClass} bg-blue-600 text-white hover:bg-blue-700`}>{busyId === "register-payment" && <Loader2 className="h-4 w-4 animate-spin" />} Зарегистрировать</button>
                        </div>
                    </form>
                </Modal>
            )}

            {allocationPayment && (
                <Modal title=t("finance.allocationTitle") onClose={() => setAllocationPayment(null)}>
                    <form className="space-y-4" onSubmit={submitAllocations}>
                        <p className="text-sm text-gray-600 dark:text-gray-300">Доступно для распределения: {formatPrice(allocationPayment.availableToAllocate)} из {formatPrice(allocationPayment.amount)}. Укажите, какую сумму направить на каждое начисление. Общая сумма не должна превышать доступный остаток.</p>
                        {accruedCharges.map(charge => (
                            <Field key={charge.id} label={`${charge.title} · остаток ${formatPrice(charge.balance)}`}>
                                <input className={inputClass} type="number" min="0" max={charge.balance} step="0.01" value={allocationAmounts[charge.id] ?? ""} onChange={event => setAllocationAmounts(prev => ({ ...prev, [charge.id]: event.target.value }))} />
                            </Field>
                        ))}
                        <div className="flex justify-end gap-2">
                            <button type="button" className={`${buttonClass} border border-gray-200 text-gray-700 dark:border-gray-700 dark:text-gray-200`} onClick={() => setAllocationPayment(null)}>{t("common.cancel")}</button>
                            <button type="submit" disabled={busyId === `allocate-${allocationPayment.id}`} className={`${buttonClass} bg-blue-600 text-white hover:bg-blue-700`}>{t("finance.saveAllocation")}</button>
                        </div>
                    </form>
                </Modal>
            )}
        </section>
    );
};

function valueIsPositive(value: number) {
    return Number.isFinite(value) && value > 0 && Number(value.toFixed(2)) === value;
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

function Modal({ title, children, onClose }: { title: string; children: ReactNode; onClose: () => void }) {
    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4" role="presentation" onMouseDown={event => { if (event.target === event.currentTarget) onClose(); }}>
            <div role="dialog" aria-modal="true" aria-label={title} className="max-h-[90vh] w-full max-w-lg overflow-y-auto rounded-2xl bg-white p-5 shadow-xl dark:bg-gray-800 sm:p-6">
                <div className="mb-5 flex items-center justify-between gap-3">
                    <h3 className="text-lg font-semibold text-gray-900 dark:text-white">{title}</h3>
                    <button type="button" onClick={onClose} aria-label=t("common.close") className="rounded-lg p-2 text-gray-500 hover:bg-gray-100 dark:hover:bg-gray-700"><X className="h-4 w-4" /></button>
                </div>
                {children}
            </div>
        </div>
    );
}

function statusClass(status: string) {
    if (status === t("finance.accrued")) return "bg-blue-100 text-blue-800 dark:bg-blue-950/40 dark:text-blue-300";
    if (status === t("finance.status.rejected")) return "bg-gray-100 text-gray-500 dark:bg-gray-700 dark:text-gray-400";
    return "bg-gray-100 text-gray-700 dark:bg-gray-700 dark:text-gray-200";
}
