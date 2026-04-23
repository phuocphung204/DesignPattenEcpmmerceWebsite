"use client";
import { Button } from "@/components/ui/button"
import { useAddCartItem } from "@/entities/cart"
import { Id } from "@/types/common-type"
import { toast } from "sonner"

type Props = {
  className?: string
  productId: Id
}
export default function AddToCardButton({ className, productId }: Props) {
  const { mutate: addCartItem, isPending } = useAddCartItem(
    () => {
      toast.success("Đã thêm vào giỏ hàng")
    },
    () => {
      toast.error("Máy chủ đang bận")
    }
  )
  const handleAddToCart = () => {
    if (isPending) return
    addCartItem({
      productId: productId,
      quantity: 1,
    })
  }
  return (
    <Button className={className} onClick={handleAddToCart} disabled={isPending}>
      Thêm vào giỏ hàng
    </Button>
  )
}