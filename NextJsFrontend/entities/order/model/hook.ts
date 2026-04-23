"use client";

import { useQuery, useQueryClient } from "@tanstack/react-query";
import { ORDER_INFO_KEY, type OrderInfo, type OrderInfoPatch } from "./type";

type UseOrderInfoOptions<Address, Shipping, DiscountAndNote, Payment> = {
  storageKey?: string;
  initialData?: OrderInfo<Address, Shipping, DiscountAndNote, Payment>;
};

export function useOrderInfo<Address = unknown, Shipping = unknown, DiscountAndNote = unknown, Payment = unknown>(
  options?: UseOrderInfoOptions<Address, Shipping, DiscountAndNote, Payment>,
) {
  const queryClient = useQueryClient();
  const storageKey = options?.storageKey ?? "checkout-order-info";

  const readStorage = (): OrderInfo<Address, Shipping, DiscountAndNote, Payment> => {
    if (typeof window === "undefined") return {};
    try {
      const raw = window.sessionStorage.getItem(storageKey);
      if (!raw) return {};
      const parsed = JSON.parse(raw) as OrderInfo<Address, Shipping, DiscountAndNote, Payment>;
      return parsed ?? {};
    } catch {
      return {};
    }
  };

  const writeStorage = (data: OrderInfo<Address, Shipping, DiscountAndNote, Payment>) => {
    if (typeof window === "undefined") return;
    try {
      window.sessionStorage.setItem(storageKey, JSON.stringify(data));
    } catch {
      return;
    }
  };

  const initialData = options?.initialData ?? readStorage();

  const { data } = useQuery<OrderInfo<Address, Shipping, DiscountAndNote, Payment>>({
    queryKey: ORDER_INFO_KEY,
    queryFn: async () => readStorage(),
    initialData,
    staleTime: Number.POSITIVE_INFINITY,
  });

  const setOrderInfo = (patch: OrderInfoPatch<Address, Shipping, DiscountAndNote, Payment>) => {
    const current =
      queryClient.getQueryData<OrderInfo<Address, Shipping, DiscountAndNote, Payment>>(ORDER_INFO_KEY) ?? readStorage();
    const next = { ...current, ...patch };
    queryClient.setQueryData(ORDER_INFO_KEY, next);
    writeStorage(next);
  };

  const clearOrderInfo = () => {
    queryClient.setQueryData(ORDER_INFO_KEY, {} as OrderInfo<Address, Shipping, DiscountAndNote, Payment>);
    writeStorage({});
  };

  const setAddress = (address: Address) => setOrderInfo({ address });
  const setShipping = (shipping: Shipping) => setOrderInfo({ shipping });
  const setDiscountAndNote = (discountAndNote: DiscountAndNote) => setOrderInfo({ discountAndNote });
  const setPayment = (payment: Payment) => setOrderInfo({ payment });

  return {
    orderInfo: data ?? initialData,
    setOrderInfo,
    setAddress,
    setShipping,
    setDiscountAndNote,
    setPayment,
    clearOrderInfo,
  };
}

export default useOrderInfo;