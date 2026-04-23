"use client";

import Image from "next/image";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Minus, Plus, Trash2 } from "lucide-react";
import { formatCurrency } from "@/utils/formatter";
import { AspectRatio } from "@/components/ui/aspect-ratio";
import { CartItemAttribute, CartItemType } from "@/entities/cart";
import { Checkbox } from "@/components/ui/checkbox";
import { Separator } from "@/components/ui/separator";

function createAttributeLabel(attributes: CartItemAttribute[]): string {
  if (!attributes || attributes.length === 0) {
    return "Không có tùy chọn";
  }

  return attributes
    .map((attribute) => `${attribute.name}: ${attribute.value}`)
    .join(" | ");
}

export type CartItemView = {
  id: string;
  itemId: string | null;
  name: string;
  option: string;
  image: string;
  unitPrice: number;
  quantity: number;
};

type CartItemCardProps = {
  className?: string;
  item: CartItemType;
  showCheckbox?: boolean;
  isSelected: boolean;
  onCheckedChange?: (checked: boolean) => void;
  onDecrease: () => void;
  onIncrease: () => void;
  onRemove: () => void;
  disableDecrease?: boolean;
  disableIncrease?: boolean;
  disableRemove?: boolean;
};

export default function CartItemCard({
  className,
  item,
  showCheckbox = true,
  isSelected,
  onCheckedChange,
  onDecrease,
  onIncrease,
  onRemove,
  disableDecrease,
  disableIncrease,
  disableRemove,
}: CartItemCardProps) {
  const itemTotal = item.sellingPrice * item.quantity;

  return (
    <Card className={`${className} flex flex-row p-3`}>
      <CardHeader className="p-0 w-30 flex flex-row items-center gap-2">
        {showCheckbox && <Checkbox checked={isSelected} onCheckedChange={onCheckedChange} />}
        <AspectRatio ratio={1} className="w-full overflow-hidden rounded-md flex items-center">
          <Image
            src={item.imageLink}
            alt={item.name}
            width={96}
            height={96}
            className="size-24 rounded-md object-cover"
          />
        </AspectRatio>
      </CardHeader>
      <CardContent className="flex flex-col items-start p-0">
        <div className="flex flex-col gap-1">
          <CardTitle>{item.name}</CardTitle>
          <CardDescription>{createAttributeLabel(item.attributes)}</CardDescription>
        </div>
        <Separator className="my-2 mt-auto" />
        <div className="flex gap-2 items-center">
          <Button
            className="cursor-pointer"
            variant="outline"
            size="icon"
            aria-label="Giảm số lượng"
            onClick={onDecrease}
            disabled={disableDecrease}
          >
            <Minus data-icon="inline-start" />
          </Button>
          <div>{item.quantity}</div>
          <Button
            className="cursor-pointer"
            variant="outline"
            size="icon"
            aria-label="Tăng số lượng"
            onClick={onIncrease}
            disabled={disableIncrease}
          >
            <Plus data-icon="inline-start" />
          </Button>
        </div>
      </CardContent>
      <CardFooter className="ml-auto flex flex-col items-end p-0">
        <CardAction className="flex flex-col items-end gap-1">
          <div>{formatCurrency(itemTotal)}</div>
          <CardDescription>{formatCurrency(item.sellingPrice)} / sản phẩm</CardDescription>
        </CardAction>
        <div className="my-auto" />
        <Button className="cursor-pointer gap-1" variant="ghost" size="sm" onClick={onRemove} disabled={disableRemove}>
          <Trash2 data-icon="inline-start" />
          Xóa
        </Button>
      </CardFooter>
    </Card>
  );
}
