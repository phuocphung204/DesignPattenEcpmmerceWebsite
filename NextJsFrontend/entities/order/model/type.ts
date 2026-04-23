export type OrderInfo<Address = unknown, Shipping = unknown, DiscountAndNote = unknown, Payment = unknown> = {
	address?: Address;
	shipping?: Shipping;
	discountAndNote?: DiscountAndNote;
	payment?: Payment;
};

export type OrderInfoPatch<Address = unknown, Shipping = unknown, DiscountAndNote = unknown, Payment = unknown> = Partial<
	OrderInfo<Address, Shipping, DiscountAndNote, Payment>
>;

export const ORDER_INFO_KEY = ["orderInfo"] as const;
