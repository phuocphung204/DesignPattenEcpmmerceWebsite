import { Separator } from "@/components/ui/separator"
import { ApplyDiscountResponseType } from "@/services/client-requests/order-requests"
import { formatCurrency } from "@/utils/formatter"

export default function SummaryOrderSection({
  subTotal,
  shippingFee,
  orderTotal,
  pointsUsed,
  applyDiscountResponse,
}: {
  subTotal: number
  shippingFee: number
  orderTotal: number
  pointsUsed: number
  applyDiscountResponse: ApplyDiscountResponseType | null // Replace 'any' with the actual type if available
}) {

  function displayAppliedDiscount() {
    if (!applyDiscountResponse) return null;
    if (applyDiscountResponse.discountAmount <= 0) return null;
    if (applyDiscountResponse.discountType === "FixedAmount")
      return (
        <div className="flex items-center justify-between text-sm text-amber-500">
          <span>Mã giảm giá</span>
          <span className="text-amber-500">- {formatCurrency(applyDiscountResponse.discountAmount)}</span>
        </div>
      )
    return (
      <div className="flex items-center justify-between text-sm text-amber-500 text-end">
        <span>Mã giảm giá</span>
        <span className="text-amber-500">
          - {applyDiscountResponse.percent}% ({"Tối đa -"}{formatCurrency(applyDiscountResponse.discountAmount)})<br />
          - {formatCurrency(subTotal - applyDiscountResponse.discountAmount)}
        </span>
      </div>
    )
  }

  return (
    <div className="mt-8 flex flex-col gap-6">
      <div className="flex items-center justify-between text-sm text-muted-foreground">
        <span>Tổng tiền sản phẩm</span>
        <span className="text-foreground">{formatCurrency(subTotal)}</span>
      </div>
      {displayAppliedDiscount()}
      <div className="flex items-center justify-between text-sm text-muted-foreground">
        <span>Phí vận chuyển</span>
        <span className="text-foreground">{formatCurrency(shippingFee)}</span>
      </div>
      {pointsUsed > 0 && (
        <div className="flex items-center justify-between text-sm text-amber-500">
          <span>Điểm sử dụng</span>
          <span className="text-amber-500">-{formatCurrency(pointsUsed * 1000)}</span>
        </div>
      )}
      <Separator />
      <div className="flex items-center justify-between">
        <span className="text-base font-semibold">Tổng cộng</span>
        <span className="text-base font-semibold">{formatCurrency(orderTotal)}</span>
      </div>
    </div>
  )
}