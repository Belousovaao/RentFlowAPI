import { apiClient } from "../../shared/api/client";
import type { CreateBookingCommand } from "./booking.types";

export const createBooking = async (cmd: CreateBookingCommand) => {
    return apiClient.post("/api/booking", cmd);
};