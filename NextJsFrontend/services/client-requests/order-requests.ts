import { AddressFormData } from "@/entities/address/type";
import { CartItemType } from "@/entities/cart";
import http from "@/services/http";

export enum ShippingType {
  Standard = 0,
  Express = 1,
}

export enum OrderStatusEnum {
  Pending = 0,
  Processing = 1,
  Shipped = 2,
  Delivered = 3,
  Cancelled = 4,
  Returned = 5,
  Completed = 6,
}

export enum PaymentStatusEnum
{
  Pending = 0,
  Paid = 1,
  Refunded = 2,
  PartiallyRefunded = 3
}

export enum PaymentMethodType {
  VnPay = 0,
  Momo = 1,
}

export enum PaymentType {
  // Cash = 0,
  Cash = 0,
  CreditCard = 1,
  BankTransfer = 2,
}

export type BankTransferInfo = {
  bankName: string;
  accountNumber: string;
}

export type CardCreditInfo = {
  cardNumber: string;
  cardHolder: string;
  cardType: CreditType;
}

export type CreditType = "Visa" | "MasterCard" | "JCB" | "AMEX" ;

export type ShippingInfoRequest = {
  type: ShippingType;
  shippingAddressId: string;
}

export type CashPaymentInfoRequest = {
  type: PaymentType.Cash;
}

export type CreditCardPaymentInfoRequest = {
  type: PaymentType.CreditCard;
} & CardCreditInfo;

export type BankTransferPaymentInfoRequest = {
  type: PaymentType.BankTransfer;
} & BankTransferInfo;

export type PaymentInfoRequest =
  | CashPaymentInfoRequest
  | CreditCardPaymentInfoRequest
  | BankTransferPaymentInfoRequest;

export type OrderItemRequest = {
  productId: string;
  requiredQuantity: number;
}

export type CreateOrderRequest = {
  discountCode?: string | null;
  pointsUsed: number;
  shippingInfo: ShippingInfoRequest;
  paymentInfo: PaymentInfoRequest;
  note?: string | null;
  items: OrderItemRequest[];
}

export type CreateOrderResponseItem = {
  productId: string;
  requiredQuantity: number;
}


export type ShippingAddressResponse = {
  receiverName: string;
  receiverPhone: string;
  country: string;
  province: string;
  district: string;
  ward: string;
  street: string;
  provinceCode: string;
  districtCode: string;
  wardCode: string;
}

export type ShippingInfoResponse = {
  type: ShippingType;
  shippingAddress: ShippingAddressResponse;
}

export type PaymentMetaResponse = {
  paymentProvider?: string | null;
  paymentChannel?: string | null;
  transactionId?: string | null;
  paidAt?: string | null;
}

export type CashPaymentInfoResponse = {
  type: PaymentType.Cash;
} & PaymentMetaResponse;

export type CreditCardPaymentInfoResponse = {
  type: PaymentType.CreditCard;
  cardNumber: string;
  cardHolder: string;
  cardType: string;
} & PaymentMetaResponse;

export type BankTransferInfoResponse = {
  type: PaymentType.BankTransfer;
  bankName: string;
  accountNumber: string;
} & PaymentMetaResponse;

export type PaymentInfoResponse =
  | CashPaymentInfoResponse
  | CreditCardPaymentInfoResponse
  | BankTransferInfoResponse;

export type OrderAttributeResponse = {
  name: string;
  value: string;
}

export type OrderItemResponse = CartItemType & {
  purchasePrice: number;
}

export type OrderDetailsItemResponse = {
  productId: string;
  productName: string;
  quantity: number;
  sku: string;
  sellingPrice: number;
  purchasePrice: number;
  thumbnailUrl: string;
  attributes: OrderAttributeResponse[];
}

export type OrderHistoryResponse = {
  status: OrderStatusEnum;
  statusChangedDate: string;
  changedBy?: string | null;
  changedByRole: number;
  note?: string | null;
}

export type InventoryAllocationResponse = {
  warehouseId: string;
  warehouseName?: string | null;
  productId: string;
  productName?: string | null;
  allocatedQuantity: number;
}

export type CreateOrderResponseType = {
  id: string;
  userId: string;
  discountCode?: string | null;
  pointsUsed?: number | null;
  subTotal: number;
  shippingFee: number;
  discountAmount: number;
  grandAmount: number;
  paymentStatus: PaymentStatusEnum;
  status: OrderStatusEnum;
  shippingInfo: { type: ShippingType; shippingAddress: AddressFormData };
  paymentInfo: PaymentInfoResponse;
  note?: string | null;
  items: OrderItemResponse[];
  histories: unknown[];
  inventoryAllocations: unknown[];
}

export type OrderDetailsResponseType = {
  id: string;
  userId: string;
  discountCode?: string | null;
  pointsUsed?: number | null;
  subTotal: number;
  shippingFee: number;
  discountAmount: number;
  grandAmount: number;
  paymentStatus: PaymentStatusEnum;
  status: OrderStatusEnum;
  shippingInfo: ShippingInfoResponse;
  paymentInfo: PaymentInfoResponse;
  note?: string | null;
  items: OrderDetailsItemResponse[];
  histories: OrderHistoryResponse[];
  inventoryAllocations: InventoryAllocationResponse[];
}

export type ApplyDiscountResponseType = 
{
  originalAmount: number;
  discountAmount: number;
  finalAmount: number;
  percent: number | null;
  amount: number | null;
  discountType: "FixedAmount" | "Percentage";
}

export function applyDiscount({ code, subTotal }: { code: string, subTotal: number }) { 
  return http.post<ApplyDiscountResponseType>("/discount-codes/calculate", { code, subTotal });
}

const orderRequests = {
  createOrder(payload: CreateOrderRequest) {
    return http.post<CreateOrderResponseType>("/orders", payload);
  },
  
  getOrderDetails(orderId: string) {
    return http.get<OrderDetailsResponseType>(`/orders/${orderId}`);
  }
}

export default orderRequests