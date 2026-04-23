"use client"

import { useUpdateUserAddress } from "@/entities/user";
import AddressForm from "./AddressForm";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { AddressFormData, AddressType } from "@/entities/address/type";
import { addressSchema } from "../model/schema";
import { toast } from "sonner";
import { useEffect } from "react";

type Props = {
  className?: string,
  showCloseButton?: boolean
  showSaveButton?: boolean
  onCloseClick?: () => void
  onPending?: () => void
  onSuccess?: (message: string) => void
  onError?: (errorMessage: string) => void
  address: AddressType
}

export default function EditAddress({ ...props }: Props) {
  const { onPending } = props
  const { mutate: updateAddress, isPending } = useUpdateUserAddress(
    (successMessage) => {
      toast.success(successMessage,)
      props.onSuccess?.(successMessage)
    },
    (errorMessage) => {
      toast.error(errorMessage)
      props.onError?.(errorMessage)
    }
  )

  useEffect(() => {
    console.log(">>> Is Pending:", isPending)
    if (isPending && typeof onPending === "function") {
      onPending()
    }
  }, [isPending, onPending])

  const form = useForm<AddressFormData>({
    resolver: zodResolver(addressSchema),
    defaultValues: props.address,
    mode: "all"
  })
  const handleUpdateAddress = (data: AddressFormData) => {
    updateAddress({ addressId: props.address.id, data })
  }
  return (
    <AddressForm title="Sửa địa chỉ" formHook={form} onSaveClick={handleUpdateAddress} {...props} />
  )
}