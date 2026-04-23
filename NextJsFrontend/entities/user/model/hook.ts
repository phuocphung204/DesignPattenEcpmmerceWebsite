"use client";

import http, { FullResponse, HttpError, MyCustomResponse } from "@/services/http";
import { UndefinedInitialDataOptions, useMutation, useQuery, UseQueryResult } from "@tanstack/react-query";
import { CURRENT_USER_KEY, CurrentUserType, CustomUseQueryOptions, UserAddressType, UserAddressUpsertDataType, UserUpdateDataType, USER_ADDRESSES_KEY } from "./type";
import { getUserJwtToken, safeAuthorizationHeader } from "@/lib/auth-helper";
import getErrorMessage from "@/utils/error-helper";
import { Id } from "@/types/common-type";
import authRequest from "@/services/backend-requests/auth-request";

/** Phục vụ cho việc lấy thông tin người dùng hiện tại trên client side */
export function useGetCurrentUser(): UseQueryResult<CurrentUserType, HttpError>;
export function useGetCurrentUser<TData>(
  select: (user: CurrentUserType) => TData,
): UseQueryResult<TData, HttpError>;
export function useGetCurrentUser<TData>(
  select: (user: CurrentUserType) => TData,
  options: CustomUseQueryOptions<CurrentUserType>
): UseQueryResult<TData, HttpError>;
export function useGetCurrentUser<TData>(
  select?: (user: CurrentUserType) => TData,
  options?: CustomUseQueryOptions<CurrentUserType>
): UseQueryResult<TData, HttpError> {

  const defaultQueryOptions: UndefinedInitialDataOptions<CurrentUserType, HttpError, TData> = {
    queryKey: CURRENT_USER_KEY,
    queryFn: async ({ signal }) => {
      // console.log(">>> getCurrentUser queryKey:", queryKey);
      // console.log(">>> getCurrentUser called with token:", getUserJwtToken());
      // console.log(">>> safeAuthorizationHeader: ", safeAuthorizationHeader());

      const res = await http.get<CurrentUserType>(
        "/users/me",
        {
          headers: {
            ...safeAuthorizationHeader()
          },
        },
        signal
      );
      const data = res.payload?.data as CurrentUserType;

      return data;
    },
    enabled: () => getUserJwtToken() !== null, // Chỉ chạy query nếu token tồn tại
    // subscribed: getUserJwtToken() !== null, // Chỉ chạy query nếu token tồn tại
    ...(select && { select }),
    ...(options || {}),
  }

  return useQuery(defaultQueryOptions);
}

/** Phục vụ cho việc cập nhật thông tin người dùng hiện tại trên client side
 * @param onSuccessUI callback để hiển thị UI khi cập nhật thành công, nhận vào message từ backend
 * @param onErrorUI callback để hiển thị UI khi cập nhật thất bại, nhận vào errorMessage đã được xử lý từ error code
 */
export function useUpdateCurrentUser(onSuccessUI?: (message: string) => void, onErrorUI?: (errorMessage: string) => void) {

  return useMutation<FullResponse<UserUpdateDataType>, HttpError, UserUpdateDataType>({
    mutationFn: (data: UserUpdateDataType) => {
      return http.patch<UserUpdateDataType & { extraData: Record<string, unknown> }>(
        "/users/profile",
        { ...data },
        {
          headers: {
            ...safeAuthorizationHeader()
          },
        }
      ) as Promise<FullResponse<UserUpdateDataType>>;
    },
    // onMutate: async () => {

    // },
    onError: async (error) => {
      if (typeof onErrorUI === "function") {
        try {
          const errorCode = (error.payload as MyCustomResponse).errorCode;
          const errorMessage = getErrorMessage(errorCode);
          console.error(">>> Update profile error code:", error.payload);
          onErrorUI(errorMessage);
        }
        catch (e) {
          console.error(">>> Failed to parse error response:", e);
          onErrorUI(getErrorMessage(null));
        }
      }
    },
    onSuccess: async (data, _, __, { client }) => {
      // Invalidate the current user query to fetch the updated data
      // client.invalidateQueries({ queryKey: CURRENT_USE_KEY });

      // Optionally, you can also directly update the cache with the new data
      const oldUserData = client.getQueryData<CurrentUserType>(CURRENT_USER_KEY);
      const newUserData = { ...oldUserData, ...data.payload?.data };
      client.setQueryData(CURRENT_USER_KEY, newUserData);

      if (typeof onSuccessUI === "function") {
        const successMessage = data.payload.message
        onSuccessUI(successMessage);
      }
    },
    // onSettled: (data, error, newData, context) => { },
  })
}

export function useGetUserAddresses(): UseQueryResult<UserAddressType[], HttpError>;
export function useGetUserAddresses<TData>(
  select: (addresses: UserAddressType[]) => TData,
): UseQueryResult<TData, HttpError>;
export function useGetUserAddresses<TData>(
  select: (addresses: UserAddressType[]) => TData,
  options: CustomUseQueryOptions<UserAddressType[]>
): UseQueryResult<TData, HttpError>;
export function useGetUserAddresses<TData>(
  select?: (addresses: UserAddressType[]) => TData,
  options?: CustomUseQueryOptions<UserAddressType[]>
): UseQueryResult<TData, HttpError> {
  const defaultQueryOptions: UndefinedInitialDataOptions<UserAddressType[], HttpError, TData> = {
    queryKey: USER_ADDRESSES_KEY,
    queryFn: async ({ signal }) => {
      const res = await http.get<UserAddressType[]>(
        "/users/me/addresses",
        {
          headers: {
            ...safeAuthorizationHeader(),
          },
        },
        signal,
      );

      return res.payload?.data as UserAddressType[];
    },
    enabled: getUserJwtToken() !== null,
    ...(select && { select }),
    ...(options || {}),
  };

  return useQuery(defaultQueryOptions);
}

/** Phục vụ cho việc tạo địa chỉ người dùng mới
 * @param handleSuccess callback để hiển thị UI khi tạo thành công, nhận vào message từ backend
 * @param handleError callback để hiển thị UI khi tạo thất bại, nhận vào errorMessage đã được xử lý từ error code
 */
export function useCreateUserAddress(handleSuccess?: (message: string) => void, handleError?: (errorMessage: string) => void) {
  return useMutation<FullResponse<UserAddressType>, HttpError, UserAddressUpsertDataType>({
    mutationFn: (data: UserAddressUpsertDataType) => {
      return http.post<UserAddressType>(
        "/users/addresses",
        data,
        {
          headers: {
            ...safeAuthorizationHeader(),
          },
        },
      ) as Promise<FullResponse<UserAddressType>>;
    },
    onError: async (error) => {
      if (typeof handleError === "function") {
        try {
          const errorCode = (error.payload as MyCustomResponse).errorCode;
          handleError(getErrorMessage(errorCode));
        } catch {
          handleError(getErrorMessage(null));
        }
      }
    },
    onSuccess: async (data, _, __, { client }) => {
      client.invalidateQueries({ queryKey: USER_ADDRESSES_KEY });

      if (typeof handleSuccess === "function") {
        handleSuccess(data.payload.message);
      }
    },
  });
}

/** Phục vụ cho việc cập nhật địa chỉ người dùng
 * @param onSuccessUI callback để hiển thị UI khi cập nhật thành công, nhận vào message từ backend
 * @param onErrorUI callback để hiển thị UI khi cập nhật thất bại, nhận vào errorMessage đã được xử lý từ error code
 */
type UpdateUserAddressMutationInput = {
  addressId: Id;
  data: UserAddressUpsertDataType;
};

export function useUpdateUserAddress(handleSuccess?: (message: string) => void, handleError?: (errorMessage: string) => void) {
  return useMutation<FullResponse<UserAddressType>, HttpError, UpdateUserAddressMutationInput>({
    mutationFn: ({ addressId, data }) => {
      return http.patch<UserAddressType>(
        `/users/addresses/${addressId}`,
        data,
        {
          headers: {
            ...safeAuthorizationHeader(),
          },
        },
      ) as Promise<FullResponse<UserAddressType>>;
    },
    onError: async (error) => {
      if (typeof handleError === "function") {
        try {
          const errorCode = (error.payload as MyCustomResponse).errorCode;
          handleError(getErrorMessage(errorCode));
        } catch {
          handleError(getErrorMessage(null));
        }
      }
    },
    onSuccess: async (data, _, __, { client }) => {
      client.invalidateQueries({ queryKey: USER_ADDRESSES_KEY });

      if (typeof handleSuccess === "function") {
        handleSuccess(data.payload.message);
      }
    },
  });
}

/** Phục vụ cho việc xóa địa chỉ người dùng
 * @param handleSuccess callback để hiển thị UI khi xóa thành công, nhận vào message từ backend
 * @param handleError callback để hiển thị UI khi xóa thất bại, nhận vào errorMessage đã được xử lý từ error code
 */
export function useDeleteUserAddress(handleSuccess?: (message: string) => void, handleError?: (errorMessage: string) => void) {
  return useMutation<FullResponse<Record<string, never>>, HttpError, Id>({
    mutationFn: (addressId: Id) => {
      return http.delete<Record<string, never>>(
        `/users/addresses/${addressId}`,
        undefined,
        {
          headers: {
            ...safeAuthorizationHeader(),
          },
        },
      ) as Promise<FullResponse<Record<string, never>>>;
    },
    onError: async (error) => {
      if (typeof handleError === "function") {
        try {
          const errorCode = (error.payload as MyCustomResponse).errorCode;
          handleError(getErrorMessage(errorCode));
        } catch {
          handleError(getErrorMessage(null));
        }
      }
    },
    onSuccess: async (data, _, __, { client }) => {
      client.invalidateQueries({ queryKey: USER_ADDRESSES_KEY });

      if (typeof handleSuccess === "function") {
        handleSuccess(data.payload.message);
      }
    },
  });
}

export function useIsAuthenticated() : UseQueryResult<boolean> {
  const jwtToken = getUserJwtToken();
  return useQuery({
    queryKey: [jwtToken],
    queryFn: async () => { 
      try {
        const res = await authRequest.validateToken(jwtToken as string);
        return res.status === 200;
      } catch {
        return false;
      }
    },
  })
}