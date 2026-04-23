"use client";

import http, { FullResponse, HttpError, MyCustomResponse } from "@/services/http";
import { UndefinedInitialDataOptions, useMutation, useQuery, UseQueryResult, useQueryClient } from "@tanstack/react-query";
import { AddCartItemData, CartItemType, CartType, CustomUseQueryOptions, UpdateCartItemData, CART_KEY, CHECKOUT_KEY } from "./type";
import { getUserJwtToken, safeAuthorizationHeader } from "@/lib/auth-helper";
import getErrorMessage from "@/utils/error-helper";
import { Id } from "@/types/common-type";

export function useGetCart(): UseQueryResult<CartType, HttpError>;
export function useGetCart<TData>(
	select: (cart: CartType) => TData,
): UseQueryResult<TData, HttpError>;
export function useGetCart<TData>(
	select: (cart: CartType) => TData,
	options: CustomUseQueryOptions<CartType>
): UseQueryResult<TData, HttpError>;
export function useGetCart<TData>(
	select?: (cart: CartType) => TData,
	options?: CustomUseQueryOptions<CartType>
): UseQueryResult<TData, HttpError> {

	const defaultQueryOptions: UndefinedInitialDataOptions<CartType, HttpError, TData> = {
		queryKey: CART_KEY,
		queryFn: async ({ signal }) => {
			const res = await http.get<CartType>(
				"/carts",
				{
					headers: {
						...safeAuthorizationHeader(),
					},
				},
				signal,
			);

			return res.payload?.data as CartType;
		},
		enabled: () => getUserJwtToken() !== null,
		...(select && { select }),
		...(options || {}),
	};

	return useQuery(defaultQueryOptions);
}

export function useAddCartItem(onSuccessUI?: (message: string) => void, onErrorUI?: (errorMessage: string) => void) {
	const queryClient = useQueryClient();

	return useMutation<FullResponse<CartItemType[]>, HttpError, AddCartItemData>({
		mutationFn: (data: AddCartItemData) => {
			return http.post<CartItemType[]>(
				"/carts/items",
				data,
				{
					headers: {
						...safeAuthorizationHeader(),
					},
				},
			) as Promise<FullResponse<CartItemType[]>>;
		},
		onError: async (error) => {
			if (typeof onErrorUI === "function") {
				try {
					const errorCode = (error.payload as MyCustomResponse).errorCode;
					onErrorUI(getErrorMessage(errorCode));
				} catch {
					onErrorUI(getErrorMessage(null));
				}
			}
		},
		onSuccess: async (data) => {
			queryClient.invalidateQueries({ queryKey: CART_KEY });

			if (typeof onSuccessUI === "function") {
				onSuccessUI(data.payload.message);
			}
		},
	});
}

export function useUpdateCartItemQuantity(onSuccessUI?: (message: string) => void, onErrorUI?: (errorMessage: string) => void) {
	const queryClient = useQueryClient();

	return useMutation<FullResponse<CartItemType[]>, HttpError, UpdateCartItemData>({
		mutationFn: ({ itemId, newQuantity }: UpdateCartItemData) => {
			return http.patch<CartItemType[]>(
				`/carts/items/${itemId}`,
				{ newQuantity },
				{
					headers: {
						...safeAuthorizationHeader(),
					},
				},
			) as Promise<FullResponse<CartItemType[]>>;
		},
		onError: async (error) => {
			if (typeof onErrorUI === "function") {
				try {
					const errorCode = (error.payload as MyCustomResponse).errorCode;
					onErrorUI(getErrorMessage(errorCode));
				} catch {
					onErrorUI(getErrorMessage(null));
				}
			}
		},
		onSuccess: async (data) => {
			queryClient.invalidateQueries({ queryKey: CART_KEY });

			if (typeof onSuccessUI === "function") {
				onSuccessUI(data.payload.message);
			}
		},
	});
}

export function useDeleteCartItem(onSuccessUI?: (message: string) => void, onErrorUI?: (errorMessage: string) => void) {
	const queryClient = useQueryClient();

	return useMutation<FullResponse<CartItemType[]>, HttpError, Id>({
		mutationFn: (itemId: Id) => {
			return http.delete<CartItemType[]>(
				`/carts/items/${itemId}`,
				undefined,
				{
					headers: {
						...safeAuthorizationHeader(),
					},
				},
			) as Promise<FullResponse<CartItemType[]>>;
		},
		onError: async (error) => {
			if (typeof onErrorUI === "function") {
				try {
					const errorCode = (error.payload as MyCustomResponse).errorCode;
					onErrorUI(getErrorMessage(errorCode));
				} catch {
					onErrorUI(getErrorMessage(null));
				}
			}
		},
		onSuccess: async (data) => {
			queryClient.invalidateQueries({ queryKey: CART_KEY });

			if (typeof onSuccessUI === "function") {
				onSuccessUI(data.payload.message);
			}
		},
	});
}

export function useCardCheckout() { 
	const queryClient = useQueryClient();
	const storageKey = "checkoutItems";

	const readStorage = (): CartItemType[] => {
		if (typeof window === "undefined") return [];
		try {
			const raw = window.sessionStorage.getItem(storageKey);
			if (!raw) return [];
			const parsed = JSON.parse(raw);
			return Array.isArray(parsed) ? parsed : [];
		} catch {
			return [];
		}
	};

	const writeStorage = (items: CartItemType[]) => {
		if (typeof window === "undefined") return;
		try {
			window.sessionStorage.setItem(storageKey, JSON.stringify(items));
		} catch {
			return;
		}
	};

	const { data } = useQuery<CartItemType[]>({
		queryKey: CHECKOUT_KEY,
		queryFn: async () => readStorage(),
		initialData: readStorage(),
	});

	const setItems = (items: CartItemType[]) => {
		queryClient.setQueryData(CHECKOUT_KEY, items);
		writeStorage(items);
	};

	const addItem = (item: CartItemType) => {
		const current = (queryClient.getQueryData<CartItemType[]>(CHECKOUT_KEY) ?? readStorage());
		const existing = current.find((entry) => entry.productId === item.productId);
		if (existing) {
			setItems(
				current.map((entry) =>
					entry.productId === item.productId
						? { ...entry, quantity: entry.quantity + item.quantity }
						: entry,
				),
			);
			return;
		}

		setItems([...current, item]);
	};

	const updateItemQuantity = (itemId: Id, quantity: number) => {
		if (quantity <= 0) {
			removeItem(itemId);
			return;
		}

		const current = (queryClient.getQueryData<CartItemType[]>(CHECKOUT_KEY) ?? readStorage());
		setItems(
			current.map((entry) =>
				entry.productId === itemId ? { ...entry, quantity } : entry,
			),
		);
	};

	const updateItem = (itemId: Id, patch: Partial<CartItemType>) => {
		const current = (queryClient.getQueryData<CartItemType[]>(CHECKOUT_KEY) ?? readStorage());
		setItems(
			current.map((entry) =>
				entry.productId === itemId ? { ...entry, ...patch } : entry,
			),
		);
	};

	const removeItem = (itemId: Id) => {
		const current = (queryClient.getQueryData<CartItemType[]>(CHECKOUT_KEY) ?? readStorage());
		setItems(current.filter((entry) => entry.productId !== itemId));
	};

	const clear = () => {
		setItems([]);
	};

	return {
		items: data ?? [],
		setItems,
		addItem,
		updateItemQuantity,
		updateItem,
		removeItem,
		clear,
	};
}