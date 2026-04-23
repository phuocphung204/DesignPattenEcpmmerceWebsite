import { HttpError } from "@/services/http";
import { Id } from "@/types/common-type";
import { UndefinedInitialDataOptions } from "@tanstack/react-query";

export type CartItemAttribute = {
  name: string;
  value: string;
};

export type CartItemType = {
  productId: Id;
  imageLink: string;
  sku: string;
  name: string;
  quantity: number;
  sellingPrice: number;
  attributes: CartItemAttribute[];
};

export type CartType = {
  userId: Id;
  items: CartItemType[];
};

export type AddCartItemData = {
  productId: Id;
  quantity: number;
};

export type UpdateCartItemData = {
  itemId: Id;
  newQuantity: number;
};

export type CustomUseQueryOptions<T = CartType> = Omit<
  UndefinedInitialDataOptions<T, HttpError>,
  "queryKey" | "queryFn" | "select" | "subscribed"
>;

export const CART_KEY = ["cart"];
export const CHECKOUT_KEY = ["checkoutItems"];
