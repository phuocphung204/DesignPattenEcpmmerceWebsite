"use client"

import { QueryClient, QueryClientProvider } from "@tanstack/react-query"
import { ReactQueryDevtools } from "@tanstack/react-query-devtools"
import { useState } from "react"
/**
 * QueryProvider component cung cấp một instance của QueryClient cho toàn bộ ứng dụng, 
 * cho phép sử dụng TanStack Query để quản lý dữ liệu và trạng thái trên client side.
 * 
 * Có thể cấu hình
 * @param children - React nodes that will have access to the QueryClient 
 * @returns JSX.Element
 */
export default function QueryProvider({
  children
}: Readonly<{
  children: React.ReactNode
}>) {
  const [queryClient] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            // Thiết lập mặc định quan trọng
            staleTime: 5 * 60 * 1000, // Dữ liệu "tươi" trong 5 phút
            gcTime: 10 * 60 * 1000,    // Giữ trong bộ nhớ 10 phút
            retry: 1,                 // Thử lại 1 lần nếu lỗi
            refetchOnWindowFocus: false, // Không load lại khi chuyển tab
          },
        },
      })
  )

  return (
    <QueryClientProvider client={queryClient}>
      {children}
      <ReactQueryDevtools initialIsOpen={true} />
    </QueryClientProvider>
  )
}