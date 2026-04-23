import envConfig  from "@/config/env-config";
import { safeAuthorizationHeader } from "@/lib/auth-helper";

type CustomOptions = Omit<RequestInit, "body"> & {
  baseUrl?: string | undefined;
  body?: object | undefined;
}

export type MyCustomResponse<TData = null> = {
  success: boolean;
  message: string;
  data: TData | null;
  errors: string[] | null;
  errorCode: string | null;
}

export type FullResponse<TData = null> = {
  status: number;
  payload: MyCustomResponse<TData>;
}

export class HttpError extends Error {
  status: number;
  payload: MyCustomResponse | unknown;

  constructor({ status, payload }: { status: number; payload: MyCustomResponse | unknown }) {
    super(`HTTP Error ${status}`);
    this.status = status;
    this.payload = payload;
    Object.setPrototypeOf(this, HttpError.prototype);
  }
}

const request = async <TData>(
  url: string,
  method: "GET" | "POST" | "PUT" | "PATCH" | "DELETE" | "HEAD",
  options?: CustomOptions | undefined,
  signal?: AbortSignal
) => {
  const body = options?.body ? JSON.stringify(options.body) : undefined;

  const baseHeader = {
    "Content-Type": "application/json"
  }

  const authHeader = typeof window !== "undefined" ? safeAuthorizationHeader() : {};

  const baseUrl = options?.baseUrl || envConfig.NEXT_PUBLIC_BACKEND_API_URL;

  // xử lý url, ví dụ url là /products hay products
  const fullUrl = url.startsWith("/") ? `${baseUrl}${url}` : `${baseUrl}/${url}`;

  console.log(`>>> ${method} ${fullUrl}`);
  // console.log(">>> Request options:", { ...options, body });
  const res = await fetch(fullUrl, {
    ...options,
    headers: {
      ...baseHeader,
      ...authHeader,
      ...options?.headers,
    },
    method,
    body,
    signal,
  });

  try {

    if (method === "HEAD") {
      return {
        status: res.status,
        payload: null,
      }
    }

    const payload: MyCustomResponse<TData> = await res.json();
    // console.log(">>> Response payload:", payload);
    const data: FullResponse<TData> = {
      status: res.status,
      payload: payload,
    }

    // Lỗi 500 ở backend TODO: chuyển sang xử lý lỗi ở backend
    if (res.status == 500) {
      const newPayload = { ...payload, data: null,}

      throw new HttpError({
        status: res.status,
        payload: newPayload,
      });
    }

    if (!res.ok) {
      const newPayload = {...payload, data: null }

      throw new HttpError({
        status: res.status,
        payload: newPayload,
      });
    }
    
    return data
  } catch (error) { 
    if (error instanceof HttpError) throw error;
    console.error("Failed to parse response as JSON:", error);
    const payload: MyCustomResponse<null> = await res.json();
    const data: FullResponse<null> = {
      status: res.status,
      payload: payload,
    }
    return data
  }
}

const http = {
  get: <TData>(
    url: string,
    options?: Omit<CustomOptions, "body"> | undefined,
    signal?: AbortSignal
  ) => request<TData>(url, "GET", options, signal),

  post: <TData>(
    url: string,
    body: object,
    options?: Omit<CustomOptions, "body"> | undefined,
  ) => request<TData>(url, "POST", { ...options, body }),

  put: <TData>(
    url: string,
    body: object,
    options?: Omit<CustomOptions, "body"> | undefined,
  ) => request<TData>(url, "PUT", { ...options, body }),

  patch: <TData>(
    url: string,
    body: object,
    options?: Omit<CustomOptions, "body"> | undefined,
  ) => request<TData>(url, "PATCH", { ...options, body }),

  delete: <TData>(
    url: string,
    body: object | undefined = undefined,
    options?: Omit<CustomOptions, "body"> | undefined,
  ) => request<TData>(url, "DELETE", { ...options, body }),
  
  head: <TData>(
    url: string,
    options?: Omit<CustomOptions, "body"> | undefined,
    signal?: AbortSignal
  ) => request<TData>(url, "HEAD", options, signal),

};

export default http;