import { Button } from "@/components/ui/button";
import { Dialog, DialogClose, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { AddressType } from "@/entities/address/type";
import AddressItem from "@/entities/address/ui/AddressItem";
import { useGetUserAddresses } from "@/entities/user/model/hook";
import UseAddressButton from "./UseAddressButton";
import { useState } from "react";

type Props = {
  onSelectAddress: (address: AddressType) => void;
}

export function UserAddressBookDialog({ onSelectAddress }: Props) {
  const { data: addresses, isError } = useGetUserAddresses()
  const [open, setOpen] = useState(false)

  function renderAddress() {
    if (isError) {
      return (
        <div className="p-4 bg-red-100 text-red-700 rounded">
          <p>Máy chủ đang bận! Không thể tải danh sách địa chỉ.</p>
        </div>
      )
    }

    if (addresses && addresses.length === 0) {
      return (
        <div className="p-4 bg-yellow-100 text-yellow-700 rounded">
          <p>Chưa có địa chỉ nào được lưu. Vui lòng thêm địa chỉ mới.</p>
        </div>
      )
    }

    return (
      <>
        {addresses?.map((address) => (
          <AddressItem
            className="mt-3"
            key={address.id}
            address={address}
            isDefault={false}
            actionButtons={
              <UseAddressButton
                onClick={(address) => { onSelectAddress(address); setOpen(false) }}
                address={address}
              />
            }
          />
        ))}
      </>
    )
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button variant="default" size="sm" className="ml-auto mr-3">
          Sổ địa chỉ
        </Button>
      </DialogTrigger>

      <DialogContent className="sm:max-w-lg">

        <DialogHeader>
          <DialogTitle className="font-semibold text-lg">Sổ địa chỉ</DialogTitle>
          <DialogDescription>
            Chọn địa chỉ giao hàng đã lưu hoặc cập nhật khi cần.
          </DialogDescription>
        </DialogHeader>

        <div className="-mx-4 no-scrollbar max-h-[50vh] overflow-y-auto px-4">
          {renderAddress()}
        </div>

        <DialogFooter>
          <DialogClose asChild>
            <Button variant="outline">Đóng</Button>
          </DialogClose>
        </DialogFooter>

      </DialogContent>
    </Dialog>
  );
}

