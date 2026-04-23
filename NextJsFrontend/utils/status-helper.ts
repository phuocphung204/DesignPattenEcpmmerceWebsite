import { OrderStatusEnum, PaymentInfoResponse, PaymentStatusEnum, PaymentType, ShippingType } from "@/services/client-requests/order-requests";

type BadgeVariant = "default" | "secondary" | "outline" | "destructive" | "ghost" | "link";

export function getOrderStatusLabel(status: OrderStatusEnum): string {
  switch (status) {
    case OrderStatusEnum.Pending:
      return "Chưa xử lý";
    case OrderStatusEnum.Processing:
      return "Đang xử lý";
    case OrderStatusEnum.Shipped:
      return "Đang giao";
    case OrderStatusEnum.Delivered:
      return "Đã giao";
    case OrderStatusEnum.Cancelled:
      return "Đã hủy";
    case OrderStatusEnum.Returned:
      return "Đã trả";
    case OrderStatusEnum.Completed:
      return "Hoàn tất";
    default:
      return "Không xác định";
  }
}

export function getOrderStatusVariant(status: OrderStatusEnum): BadgeVariant {
  switch (status) {
    case OrderStatusEnum.Cancelled:
    case OrderStatusEnum.Returned:
      return "destructive";
    case OrderStatusEnum.Completed:
    case OrderStatusEnum.Delivered:
      return "default";
    default:
      return "outline";
  }
}

export function getPaymentStatusLabel(status: PaymentStatusEnum): string {
  switch (status) {
    case PaymentStatusEnum.Pending:
      return "Chờ thanh toán";
    case PaymentStatusEnum.Paid:
      return "Đã thanh toán";
    case PaymentStatusEnum.Refunded:
      return "Đã hoàn tiền";
    case PaymentStatusEnum.PartiallyRefunded:
      return "Hoàn tiền một phần";
    default:
      return "Không xác định";
  }
}

export function getPaymentStatusVariant(status: PaymentStatusEnum): BadgeVariant {
  switch (status) {
    case PaymentStatusEnum.Paid:
      return "default";
    case PaymentStatusEnum.Refunded:
    case PaymentStatusEnum.PartiallyRefunded:
      return "secondary";
    default:
      return "outline";
  }
}

export function getShippingLabel(type: ShippingType): string {
  switch (type) {
    case ShippingType.Standard:
      return "Giao hàng tiêu chuẩn";
    case ShippingType.Express:
      return "Giao hàng nhanh";
    default:
      return "Không xác định";
  }
}

export function getPaymentMethodLabel(paymentInfo: PaymentInfoResponse): string {
  switch (paymentInfo.type) {
    case PaymentType.Cash:
      return "Thanh toán khi nhận hàng";
    case PaymentType.CreditCard:
      return "Thẻ tín dụng";
    case PaymentType.BankTransfer:
      return "Chuyển khoản";
    default:
      return "Không xác định";
  }
}