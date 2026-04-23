"use client"
import { Button } from "@/components/ui/button";
import { CartItemType, useCardCheckout } from "@/entities/cart";
import { useRouter } from "next/navigation";
type Props = {
  item: CartItemType
}
export default function BuyNowButton({ item }: Props) {
  const router = useRouter();
  const { setItems } = useCardCheckout()
  const handleOnClickByNow = () => {
    setItems([item])
    router.push("/thanh-toan")
  }
  return (
    <Button
      onClick={handleOnClickByNow}
      type="button"
      className="flex w-full cursor-pointer items-center justify-center rounded-full bg-orange-500 px-6 py-3 text-sm font-semibold text-white shadow-lg shadow-orange-200 transition hover:bg-orange-600 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-orange-500"
      aria-label="Buy now"
    >
      Mua ngay
    </Button>
  )
}