"use client"

import { useCreateUserAddress } from "@/entities/user";
import AddressForm from "./AddressForm";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { AddressFormData } from "@/entities/address/type";
import { addressSchema } from "../model/schema";
import { toast } from "sonner";
import { useEffect } from "react";

type Props = {
  className?: string,
  showCloseButton?: boolean
  showSaveButton?: boolean
  onSaveClick?: (data: AddressFormData) => void
  onPending?: () => void
  onSuccess?: (message: string) => void
  onError?: (errorMessage: string) => void
  onCloseClick?: () => void
}

export default function CreateAddress({ ...props }: Props) {
  const { onPending } = props
  const { mutate: createAddress, isPending } = useCreateUserAddress(
    (successMessage) => {
      toast.success(successMessage)
      props.onSuccess?.(successMessage)
    },
    (errorMessage) => {
      toast.error(errorMessage)
      props.onError?.(errorMessage)
    }
  )
  const form = useForm<AddressFormData>({
    resolver: zodResolver(addressSchema),
    mode: "all"
  })
  const handleCreateAddress = (data: AddressFormData) => {
    createAddress(data)
  }

  useEffect(() => {
    console.log(">>> Is Pending:", isPending)
    if (isPending && typeof onPending === "function") {
      onPending()
    }
  }, [isPending, onPending])

  return (
    <AddressForm formHook={form} onSaveClick={handleCreateAddress} {...props} />
  )
}