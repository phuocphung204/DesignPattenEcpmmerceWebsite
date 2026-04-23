"use client";;
import { User } from "lucide-react";
import Link from "next/link";
import UserDropdownMenu from "./UserDropdownMenu";
import UserAvatar from "@/entities/user/ui/UserAvatar";
import { useGetCurrentUser, useIsAuthenticated } from "@/entities/user";

export default function TempName() {
  const isAuthenticated = useIsAuthenticated().data
  const { data: userName } = useGetCurrentUser(
    (user) => user.fullName,
    {
      enabled: isAuthenticated, // Chỉ chạy query nếu người dùng đã đăng nhập
    }
  );
  const safeUserName = userName || "Xin chào";

  if (isAuthenticated === false) {
    return (
      <Link
        href="/dang-nhap"
        className="flex items-center border border-slate-300 gap-2 rounded-3xl px-3 py-2 text-sm font-semibold text-slate-700 transition-colors duration-200 hover:bg-emerald-50 hover:text-emerald-700 focus-visible:ring-2 focus-visible:ring-emerald-500"
        aria-label="Mở tài khoản"
      >
        <User className="h-5 w-5" />
        <span className="hidden sm:block">Đăng nhập</span>
      </Link>
    )
  }

  return (
    <UserDropdownMenu
      trigger={
        <div className="flex max-w-48 items-center border border-slate-300 gap-2 rounded-3xl p-1 text-sm font-semibold text-slate-700 transition-colors duration-200 hover:bg-emerald-50 hover:text-emerald-700 focus-visible:ring-2 focus-visible:ring-emerald-500">
          <UserAvatar className="size-9 mr-0.5" />
          <span title={safeUserName} className="hidden sm:block text-ellipsis overflow-hidden whitespace-nowrap max-w-30">
            {safeUserName}
          </span>
        </div>
      }
    />
  );
}