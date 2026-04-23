"use client"

import type { ReactElement } from "react";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { LucideIcon, CircleUserRound, Settings, LogOut, PackageSearch, MapPinHouse } from "lucide-react";
import UserAvatar from "@/entities/user/ui/UserAvatar";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { useRouter } from "next/navigation";
import Cookies from "js-cookie";

type Props = {
  trigger: ReactElement;
  defaultOpen?: boolean;
  align?: "start" | "center" | "end";
};

type MenuItem = {
  label: string;
  icon: LucideIcon;
  destructive?: boolean;
  href: string;
};

const PROFILE_ITEMS: MenuItem[] = [
  { label: "Tài khoản của tôi", icon: CircleUserRound, href: "/tai-khoan" },
  { label: "Đơn hàng", icon: PackageSearch, href: "/tai-khoan?section=account-info" },
  { label: "Sổ địa chỉ", icon: MapPinHouse, href: "/tai-khoan?section=address-book" },
];

const SETTINGS_ITEMS: MenuItem[] = [
  { label: "Cài đặt tài khoản", icon: Settings, href: "/tai-khoan?section=security-settings" },
];

const LOGOUT_ITEM: MenuItem = {
  label: "Đăng xuất",
  icon: LogOut,
  href: "",
  destructive: true,
};

const itemClass =
  "p-2 text-sm font-medium text-popover-foreground cursor-pointer gap-2";

export default function UserDropdownMenu({ trigger, defaultOpen, align = "end" }: Props) {
  const router = useRouter();
  const handleLogout = () => {
    Cookies.remove("jwtToken");
    router.push("/");
  }

  return (
    <div className="flex items-center justify-center">
      <DropdownMenu defaultOpen={defaultOpen}>
        <DropdownMenuTrigger className="cursor-pointer">
          {trigger}
        </DropdownMenuTrigger>

        <DropdownMenuContent
          align={align}
          className="w-3xs rounded-2xl data-open:slide-in-from-bottom-20! data-closed:slide-out-to-bottom-20 data-open:fade-in-0 data-closed:fade-out-0 data-closed:zoom-out-100 duration-400"
        >
          <DropdownMenuGroup>
            {/* User Info */}
            <DropdownMenuLabel className="flex items-center gap-3 px-4 py-3">
              <div className="relative">
                <UserAvatar className="size-11" />
                <span className="ring-card absolute right-0 bottom-0 size-2 rounded-full bg-green-600 ring-2" />
              </div>

              <div className="flex flex-col">
                <span className="text-popover-foreground text-sm font-medium">
                  David McMichael
                </span>
                <span className="text-muted-foreground text-sm">
                  david@shadcnspace.com
                </span>
              </div>
            </DropdownMenuLabel>

            <DropdownMenuSeparator />

            {/* Main Links */}
            {PROFILE_ITEMS.map(({ label, icon: Icon, href }) => (
              <DropdownMenuItem key={label} className={itemClass}>
                <Link href={href} className="flex items-center gap-2 w-full">
                  <Icon size={20} />
                  <span>{label}</span>
                </Link>
              </DropdownMenuItem>
            ))}

            <DropdownMenuSeparator />

            {/* Settings */}
            <DropdownMenuGroup>
              {SETTINGS_ITEMS.map(({ label, icon: Icon }) => (
                <DropdownMenuItem key={label} className={itemClass}>
                  <Icon size={20} />
                  <span>{label}</span>
                </DropdownMenuItem>
              ))}
            </DropdownMenuGroup>

            <DropdownMenuSeparator />

            {/* Logout */}
            <DropdownMenuItem variant="destructive" className={itemClass}
              onClick={handleLogout}
            >
              <Link
                // href={LOGOUT_ITEM.href}
                href={"#"}
                className="flex items-center gap-2">
                <LOGOUT_ITEM.icon size={20} />
                <span>{LOGOUT_ITEM.label}</span>
              </Link>
            </DropdownMenuItem>
          </DropdownMenuGroup>
        </DropdownMenuContent>
      </DropdownMenu>
    </div>
  );
}
