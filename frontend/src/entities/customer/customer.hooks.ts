import { customerApi } from "@/features/customer/api/customerApi";
import { useQuery } from "@tanstack/react-query";

export const useCustomer = (id?: string) => useQuery({
    queryKey: ["customer", id],
    queryFn: () => customerApi.getById(id!).then(r => r.data),
    enabled: !!id,
});