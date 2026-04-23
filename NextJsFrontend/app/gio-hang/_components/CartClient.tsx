"use client";;
import { useMemo, useState } from "react";
import CartItemCard from "@/entities/cart/ui/CartItemCard";
import OrderSummaryCard from "./OrderSummaryCard";
import { CartItemType, useCardCheckout, useDeleteCartItem, useGetCart, useUpdateCartItemQuantity } from "@/entities/cart";

function calculateTotalPriceAndTotalCount(items: CartItemType[]): { totalPrice: number; totalCount: number } {
  const totalPrice = items.reduce((acc, item) => acc + item.sellingPrice * item.quantity, 0);
  const totalCount = items.reduce((acc, item) => acc + item.quantity, 0);
  return { totalPrice, totalCount };
}

export default function CartClient() {
  const { setItems } = useCardCheckout();
  const { data, isLoading } = useGetCart();
  const [selectedItemsIndexes, setSelectedItemsIndexes] = useState<Set<string>>(new Set());
  const updateQuantity = useUpdateCartItemQuantity();
  const deleteItem = useDeleteCartItem();

  const items = useMemo(() => data?.items ?? [], [data]);

  const { totalPrice, totalCount } = useMemo(() => {
    const currentSelectedItems = items.filter((item) => selectedItemsIndexes.has(item.productId));
    return calculateTotalPriceAndTotalCount(currentSelectedItems);
  }, [selectedItemsIndexes, items]);

  const isMutating = updateQuantity.isPending || deleteItem.isPending;

  const handleCheckedChange = (checked: boolean, item: CartItemType) => {
    if (checked) {
      setSelectedItemsIndexes((prev) => {
        const newSelectedItems = new Set(prev);
        newSelectedItems.add(item.productId);
        return newSelectedItems;
      });
    } else {
      setSelectedItemsIndexes((prev) => {
        const newSelectedItems = new Set(prev);
        newSelectedItems.delete(item.productId);
        return newSelectedItems;
      });
    }
  };

  const handleSetCartCheckout = () => {
    const selectedItems = items.filter((item) => selectedItemsIndexes.has(item.productId));
    setItems(selectedItems);
  }

  return (
    <main className="mx-auto w-full max-w-6xl px-4 py-10">
      <div className="flex flex-col gap-8">
        {/* <h1>Giỏ hàng</h1> */}
        {isLoading ? (
          <div>Đang tải giỏ hàng...</div>
        ) : items.length === 0 ? (
          <div>Giỏ hàng của bạn đang trống.</div>
        ) : (
          <div className="grid items-start gap-6 lg:grid-cols-[minmax(0,1fr)_320px]">
            <div className="flex flex-col gap-6">
              {items.map((item) => (
                <CartItemCard
                  isSelected={selectedItemsIndexes.has(item.productId)}
                  key={item.productId}
                  item={item}
                  onCheckedChange={(checked) => handleCheckedChange(checked, item)}
                  onDecrease={() => {
                    if (!item.productId || item.quantity <= 1 || isMutating) return;
                    updateQuantity.mutate({
                      itemId: item.productId,
                      newQuantity: item.quantity - 1,
                    });
                  }}
                  onIncrease={() => {
                    if (!item.productId || isMutating) return;
                    updateQuantity.mutate({
                      itemId: item.productId,
                      newQuantity: item.quantity + 1,
                    });
                  }}
                  onRemove={() => {
                    if (!item.productId || isMutating) return;
                    deleteItem.mutate(item.productId);
                  }}
                  disableDecrease={item.quantity <= 1 || !item.productId || isMutating}
                  disableIncrease={!item.productId || isMutating}
                  disableRemove={!item.productId || isMutating}
                />
              ))}
            </div>
            <OrderSummaryCard
              onCheckout={handleSetCartCheckout}
              summary={{
                itemCount: totalCount,
                // subtotal: totalPrice,
                // shipping,
                totalPrice,
              }}
            />
          </div>
        )}
      </div>
    </main>
  );
}
