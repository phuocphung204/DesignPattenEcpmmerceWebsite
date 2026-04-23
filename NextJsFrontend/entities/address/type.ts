import { addressSchema } from "@/features/address";
import { Id } from "@/types/common-type";
import { z } from "zod";

export type AddressFormData = z.infer<typeof addressSchema>;
export type AddressType = AddressFormData & {
  id: Id;
}