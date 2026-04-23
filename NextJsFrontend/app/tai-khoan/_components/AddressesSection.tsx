"use client";;
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu"
import AddressItem from "@/entities/address/ui/AddressItem"
import AddressItemSkeleton from "@/entities/address/ui/SkeletonAddressItem";
import {
  useDeleteUserAddress,
  useGetCurrentUser,
  useGetUserAddresses,
  useUpdateCurrentUser,
} from "@/entities/user";
import CreateAddress from "@/features/address/ui/CreateAddress"
import EditAddress from "@/features/address/ui/EditAddress";
import { Id } from "@/types/common-type"
import { Check, MapPin, MoreVertical, Pencil, Plus, Trash2 } from "lucide-react"
import { useState } from "react";
import { toast } from "sonner"

export default function AddressesSection() {
  const [editingAddressId, setEditingAddressId] = useState<Id | null>(null)
  const { data: defaultAddressId } = useGetCurrentUser((user) => user.defaultAddressId)
  const { data: addresses, isError } = useGetUserAddresses()
  const { mutate: deleteUserAddress } = useDeleteUserAddress(
    (successMessage) => {
      toast.success(successMessage)
    },
    (errorMessage) => toast.error(errorMessage)
  )

  const [showCreateAddress, setShowCreateAddress] = useState(false)
  const [isCreating, setIsCreating] = useState(false)

  const { mutate: updateDefaultAddress } = useUpdateCurrentUser(
    (successMessage) => {
      toast.success(successMessage)
    },
    (errorMessage) => toast.error(errorMessage)
  )

  const handleDeleteAddress = (addressId: Id) => {
    if (confirm("Bạn có chắc chắn muốn xóa địa chỉ này không? Hành động này không thể hoàn tác."))
      deleteUserAddress(addressId)
  }

  const handleSetDefaultAddress = (addressId: Id) => {
    if (confirm("Bạn có chắc chắn muốn đặt địa chỉ này làm mặc định không?"))
      updateDefaultAddress({ defaultAddressId: addressId })
  }

  return (
    <>
      {isError && <p>Error fetching addresses</p>}
      {addresses && addresses.length === 0 && (
        <Card className="p-0">
          <CardContent className="flex flex-col items-center justify-center py-12">
            <MapPin className="mb-4 size-12 text-muted-foreground" />
            <h2 className="text-xl font-semibold">Chưa có địa chỉ nào</h2>
            <p className="mt-2 text-muted-foreground">
              Thêm địa chỉ giao hàng để thuận tiện hơn khi mua sắm và thanh toán nhanh chóng.
            </p>
            <Button className="mt-4" onClick={() => { setIsCreating(true); setShowCreateAddress(true) }}>
              <Plus className="mr-2 size-4" />
              Thêm mới
            </Button>
          </CardContent>
        </Card>
      )}

      {/* Address Items */}
      {addresses?.map((address) => (
        !(editingAddressId === address.id) ? (
          <AddressItem
            key={address.id}
            address={address}
            isDefault={address.id === defaultAddressId}
            actionButtons={
              <AddressActionButtons
                onEditClick={() => setEditingAddressId(address.id)}
                onDefaultClick={() => handleSetDefaultAddress(address.id)}
                onDeleteClick={() => handleDeleteAddress(address.id)}
              />
            }
          />
        ) : (
          <EditAddress
            key={address.id}
            showCloseButton
            showSaveButton
            address={address}
            onPending={() => setEditingAddressId(null)}
            onError={() => setEditingAddressId(address.id)}
            onCloseClick={() => setEditingAddressId(null)}
          />
        )
      ))}
      {/* hiện form thì không hiện skeleton và nguoc lại */}
      {isCreating && !showCreateAddress && <AddressItemSkeleton />}
      {showCreateAddress &&
        <CreateAddress
          showCloseButton
          showSaveButton
          onPending={() => { setIsCreating(true); setShowCreateAddress(false) }}
          // onError={() => setShowCreateAddress(true)}
          onSuccess={() => { setIsCreating(false); setShowCreateAddress(false) }}
          onCloseClick={() => { setIsCreating(false); setShowCreateAddress(false) }}
        />}
      <div className="flex">
        <Button disabled={isCreating} className="ml-auto" onClick={() => { setIsCreating(true); setShowCreateAddress(true) }}>
          <Plus className="mr-2 size-4" />
          Thêm mới
        </Button>
      </div>
      {/* <AddressBook1 /> */}
    </>
  )
}

function AddressActionButtons({
  onEditClick,
  onDefaultClick,
  onDeleteClick,
}: {
  onEditClick?: () => void,
  onDefaultClick?: () => void,
  onDeleteClick?: () => void,
}) {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button
          variant="ghost"
          size="icon"
          className="size-8"
          onClick={(e) => e.stopPropagation()}
        >
          <MoreVertical className="size-4" />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        <DropdownMenuItem
          onClick={onEditClick}
        >
          <Pencil className="mr-2 size-4" />
          Sửa
        </DropdownMenuItem>
        <DropdownMenuItem
          onClick={onDefaultClick}
        >
          <Check className="mr-2 size-4" />
          Mặc định
        </DropdownMenuItem>
        <DropdownMenuItem
          className="text-destructive"
          onClick={onDeleteClick}
        >
          <Trash2 className="mr-2 size-4" />
          Xóa
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  )
}

