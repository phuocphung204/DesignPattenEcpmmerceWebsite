"use client";
import orderRequests, { OrderDetailsResponseType } from "@/services/client-requests/order-requests";
import { PaymentType } from "@/services/client-requests/order-requests";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { Skeleton } from "@/components/ui/skeleton";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { formatAddress, formatCurrency, formatDatetime } from "@/utils/formatter";
import { useSearchParams } from "next/navigation";
import { useEffect, useState } from "react";
import Link from "next/link";
import { CheckCircle2Icon } from "lucide-react";
import { getOrderStatusLabel, getOrderStatusVariant, getPaymentMethodLabel, getPaymentStatusLabel, getPaymentStatusVariant, getShippingLabel } from "@/utils/status-helper";
import Image from "next/image";
export default function OrderDetailsClient() {
  const searchParams = useSearchParams()
  const orderId = searchParams.get("orderId") || "";
  const [orderDetails, setOrderDetails] = useState<OrderDetailsResponseType | null>(null);

  useEffect(() => {
    if (!orderId) return;
    const loadOrderDetails = async () => {
      const res = await orderRequests.getOrderDetails(orderId);
      const orderDetails = res.payload?.data ?? null;
      setOrderDetails(orderDetails);
    }
    console.log("Loading order details for orderId:", orderId);
    loadOrderDetails();
  }, [orderId])

  if (!orderId) {
    return (
      <div className="mx-auto flex w-full max-w-4xl flex-col gap-6 px-4 py-10">
        <Alert variant="destructive">
          <AlertTitle>Không tìm thấy đơn hàng</AlertTitle>
          <AlertDescription>
            Vui lòng kiểm tra lại đường dẫn hoặc liên hệ hỗ trợ.
          </AlertDescription>
        </Alert>
      </div>
    );
  }

  if (!orderDetails) {
    return (
      <div className="mx-auto flex w-full max-w-4xl flex-col gap-6 px-4 py-10">
        <Card>
          <CardHeader>
            <CardTitle>Đang tải thông tin đơn hàng</CardTitle>
            <CardDescription>Vui lòng đợi trong giây lát.</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="flex flex-col gap-3">
              <Skeleton className="h-5 w-3/5" />
              <Skeleton className="h-5 w-2/5" />
              <Skeleton className="h-20 w-full" />
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  const pointsUsed = orderDetails.pointsUsed ?? 0;
  const paidAt = formatDatetime(orderDetails.paymentInfo.paidAt);
  const cardSuffix =
    orderDetails.paymentInfo.type === PaymentType.CreditCard
      ? orderDetails.paymentInfo.cardNumber?.slice(-4)
      : null;
  const address = formatAddress(orderDetails.shippingInfo.shippingAddress);
  return (
    <>
      <Alert>
        <CheckCircle2Icon />
        <AlertTitle>Đặt hàng thành công</AlertTitle>
        <AlertDescription>
          <div className="flex flex-wrap items-center gap-2">
            <span>Cảm ơn bạn đã mua hàng.</span>
            <Badge variant="secondary">Mã đơn hàng: {orderDetails.id}</Badge>
          </div>
        </AlertDescription>
      </Alert>

      <div className="grid gap-6 lg:grid-cols-[1.2fr_0.8fr]">
        <div className="flex flex-col gap-6">
          <Card>
            <CardHeader>
              <CardTitle>Thông tin giao hàng</CardTitle>
              <CardDescription>Địa chỉ và thông tin người nhận.</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="flex flex-col gap-2">
                <div>{orderDetails.shippingInfo.shippingAddress.receiverName}</div>
                <div>{orderDetails.shippingInfo.shippingAddress.receiverPhone}</div>
                <div>{address}</div>
                <Badge variant="outline">{getShippingLabel(orderDetails.shippingInfo.type)}</Badge>
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Sản phẩm</CardTitle>
              <CardDescription>Danh sách sản phẩm đã đặt.</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="flex flex-col gap-4">
                {orderDetails.items.map((item, index) => (
                  <div key={`${item.productId}-${index}`} className="flex flex-col gap-4">
                    <div className="flex gap-4">
                      <Image
                        src={item.thumbnailUrl}
                        alt={item.productName}
                        className="size-16 rounded-md object-cover"
                        width={64}
                        height={64}
                      />
                      <div className="flex flex-1 flex-col gap-2">
                        <div className="flex flex-wrap gap-2">
                          <Badge variant="outline">Số lượng: {item.quantity}</Badge>
                          <Badge variant="secondary">{formatCurrency(item.purchasePrice)}</Badge>
                        </div>
                        {item.attributes.length > 0 && (
                          <div className="flex flex-wrap gap-2">
                            {item.attributes.map((attribute) => (
                              <Badge key={`${attribute.name}-${attribute.value}`} variant="outline">
                                {attribute.name}: {attribute.value}
                              </Badge>
                            ))}
                          </div>
                        )}
                      </div>
                    </div>
                    {index < orderDetails.items.length - 1 && <Separator />}
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </div>

        <div className="flex flex-col gap-6">
          <Card>
            <CardHeader>
              <CardTitle>Tổng quan đơn hàng</CardTitle>
              <CardDescription>Trạng thái và tổng tiền.</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="flex flex-col gap-4">
                <div className="flex flex-wrap items-center gap-2">
                  <Badge variant={getOrderStatusVariant(orderDetails.status)}>
                    {getOrderStatusLabel(orderDetails.status)}
                  </Badge>
                  <Badge variant={getPaymentStatusVariant(orderDetails.paymentStatus)}>
                    {getPaymentStatusLabel(orderDetails.paymentStatus)}
                  </Badge>
                </div>
                <div className="flex flex-col gap-2">
                  <div className="flex items-center justify-between">
                    <span>Tạm tính</span>
                    <span>{formatCurrency(orderDetails.subTotal)}</span>
                  </div>
                  <div className="flex items-center justify-between">
                    <span>Phí vận chuyển</span>
                    <span>{formatCurrency(orderDetails.shippingFee)}</span>
                  </div>
                  {orderDetails.discountAmount > 0 && (
                    <div className="flex items-center justify-between">
                      <span>Giảm giá</span>
                      <span>-{formatCurrency(orderDetails.discountAmount)}</span>
                    </div>
                  )}
                  {pointsUsed > 0 && (
                    <div className="flex items-center justify-between">
                      <span>Điểm sử dụng</span>
                      <span>-{formatCurrency(pointsUsed * 1000)}</span>
                    </div>
                  )}
                  <Separator />
                  <div className="flex items-center justify-between">
                    <span>Tổng cộng</span>
                    <span>{formatCurrency(orderDetails.grandAmount)}</span>
                  </div>
                </div>
                {orderDetails.discountCode && (
                  <Badge variant="outline">Mã giảm giá: {orderDetails.discountCode}</Badge>
                )}
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Thông tin thanh toán</CardTitle>
              <CardDescription>Phương thức và giao dịch.</CardDescription>
            </CardHeader>
            <CardContent>
              <div className="flex flex-col gap-3">
                <div className="flex items-center justify-between">
                  <span>Phương thức</span>
                  <span>{getPaymentMethodLabel(orderDetails.paymentInfo)}</span>
                </div>
                {orderDetails.paymentInfo.type === PaymentType.CreditCard && (
                  <div className="flex items-center justify-between">
                    <span>Thẻ</span>
                    <span>
                      {orderDetails.paymentInfo.cardType}
                      {cardSuffix ? ` **** ${cardSuffix}` : ""}
                    </span>
                  </div>
                )}
                {orderDetails.paymentInfo.type === PaymentType.BankTransfer && (
                  <div className="flex flex-col gap-2">
                    <div className="flex items-center justify-between">
                      <span>Ngân hàng</span>
                      <span>{orderDetails.paymentInfo.bankName}</span>
                    </div>
                    <div className="flex items-center justify-between">
                      <span>Số tài khoản</span>
                      <span>{orderDetails.paymentInfo.accountNumber}</span>
                    </div>
                  </div>
                )}
                {orderDetails.paymentInfo.transactionId && (
                  <div className="flex items-center justify-between">
                    <span>Mã giao dịch</span>
                    <span>{orderDetails.paymentInfo.transactionId}</span>
                  </div>
                )}
                {paidAt && (
                  <div className="flex items-center justify-between">
                    <span>Thời gian thanh toán</span>
                    <span>{paidAt}</span>
                  </div>
                )}
              </div>
            </CardContent>
            <CardFooter>
              <div className="flex flex-wrap gap-2">
                <Button asChild variant="outline">
                  <Link href="/">Về trang chủ</Link>
                </Button>
                <Button asChild>
                  <Link href="/tai-khoan/don-hang">Xem đơn hàng</Link>
                </Button>
              </div>
            </CardFooter>
          </Card>
        </div>
      </div>
    </>
  )
}