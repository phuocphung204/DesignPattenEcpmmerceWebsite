
import { HttpError } from "@/services/http";
import { Id } from "@/types/common-type";
import { AddressFormData } from "@/entities/address/type";
import { UndefinedInitialDataOptions } from "@tanstack/react-query";

export type CurrentUserType = {
  id: Id;
  userId: string;
  email: string;
  fullName: string;
  avatarLink: string;
  role: string;
  loyaltyPoints: number;
  defaultAddressId: Id;
};

export type UserUpdateDataType = Partial<Pick<CurrentUserType, "fullName" | "avatarLink" | "defaultAddressId">>;

export type UserAddressType = {
  id: Id;
  receiverName: string;
  phoneNumber: string;
  country: string;
  province: string;
  district: string;
  ward: string;
  street: string;
  provinceCode: string;
  districtCode: string;
  wardCode: string;
};

export type UserAddressUpsertDataType = AddressFormData;

export type CustomUseQueryOptions<T = CurrentUserType> = Omit<UndefinedInitialDataOptions<T, HttpError>, "queryKey" | "queryFn" | "select" | "subscribed">;

export const CURRENT_USER_KEY = ["currentUser"];
export const USER_ADDRESSES_KEY = ["currentUserAddresses"];

