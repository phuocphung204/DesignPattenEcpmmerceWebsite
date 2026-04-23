"use client"

import { useGetCart } from "@/entities/cart";
import { useIsAuthenticated } from "@/entities/user"

export default function CartCount() {
  const { data: isAuthenticated } = useIsAuthenticated()
  const { data: cartCount } = useGetCart(cart => cart.items.reduce((total, item) => total + item.quantity, 0))

  if (!isAuthenticated) {
    return (
      <span className="absolute -right-2 -top-2 inline-flex min-w-5 items-center justify-center rounded-full bg-orange-500 px-1 text-[10px] font-bold text-white">
        {0}
      </span>
    );
  }

  return (
    <span className="absolute -right-2 -top-2 inline-flex min-w-5 items-center justify-center rounded-full bg-orange-500 px-1 text-[10px] font-bold text-white">
      {cartCount}
    </span>
  )
}