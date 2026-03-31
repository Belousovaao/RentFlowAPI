import { useState } from "react";
import { createBooking } from "../../entities/booking/booking.api";

export const BookingsPage = () => {
    const [assetId, setAssetId] = useState("");
    const [customerId, setCustomerId] = useState("");
    const [startDate, setStartDate] = useState("");
    const [endDate, setEndDate] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [success, setSuccess] = useState(false);

    const handleSubmit = async () => {
        try {
            setError(null);
            setSuccess(false);

            await createBooking({
                assetId,
                customerId,
                startDate,
                endDate,
            });

            setSuccess(true);
        }
        catch (e: any) {
            setError(e.message);
        }
    };

    return (
        <div>
            <h1>CreateBooking</h1>

            <input
                placeholder="AssetId"
                value={assetId}
                onChange={(e) => setAssetId(e.target.value)}
            />

            <input
                placeholder="CustomerId"
                value={customerId}
                onChange={(e) => setCustomerId(e.target.value)}
            />

            <input
                placeholder="date"
                value={startDate}
                onChange={(e) => setStartDate(e.target.value)}
            />

            <input
                placeholder="date"
                value={endDate}
                onChange={(e) => setEndDate(e.target.value)}
            />

            <button onClick={handleSubmit}>Create</button>

            {error && <div style={{ color: "red" }}>{error}</div>}
            {success && <div style={{ color: "green" }}>Booking created</div>}
        </div>
    );
};