"use client";

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
import { formatCurrency } from "@/utils/formatter";
import Link from "next/link";

export type OrderSummaryView = {
  itemCount: number;
  // subtotal: number;
  // shipping: number;
  totalPrice: number;
};

export default function OrderSummaryCard({
  summary,
  onCheckout,
}: {
  summary: OrderSummaryView;
  onCheckout: () => void;
}) {
  return (
    <Card>
      <CardHeader>
        <CardTitle>Tóm tắt đơn hàng</CardTitle>
        <CardDescription>{summary.itemCount} sản phẩm</CardDescription>
      </CardHeader>
      <CardContent className="flex flex-col gap-4">
        {/* <div className="flex items-center justify-between">
          <div>Tam tinh</div>
          <div>{formatCurrency(summary.subtotal)}</div>
        </div>
        <div className="flex items-center justify-between">
          <div>Phi van chuyen</div>
          <div>{formatCurrency(summary.shipping)}</div>
        </div> */}
        <Separator />
        <div className="flex items-center justify-between">
          <div>Tổng cộng</div>
          <div>{formatCurrency(summary.totalPrice)}</div>
        </div>
      </CardContent>
      <CardFooter className="flex flex-col gap-3">
        <Link href="/thanh-toan" className="w-full">
          <Button
            onClick={onCheckout}
            className="w-full cursor-pointer"
          >Tiến hành thanh toán</Button>
        </Link>
      </CardFooter>
    </Card>
  );
}
