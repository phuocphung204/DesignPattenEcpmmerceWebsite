"use client"

import { Button } from "@/components/ui/button";
import { AddressType } from "@/entities/address/type";

export default function UseAddressButton({
  onClick,
  address
}: {
  onClick: (address: AddressType) => void;
  address: AddressType
}) {
  return (
    <Button variant="ghost" onClick={() => onClick(address)}
      className="text-amber-500 hover:bg-amber-100/80 hover:text-amber-600"
    >
      Dùng
    </Button>
  );
}