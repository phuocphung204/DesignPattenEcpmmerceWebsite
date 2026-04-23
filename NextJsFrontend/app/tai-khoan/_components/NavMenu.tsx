"use client";;
import { Button } from "@/components/ui/button";
import { AccountMenuItem, AccountSectionKey } from "../_hooks/use-account-management";
import { Separator } from "@/components/ui/separator";
import { LogOut } from "lucide-react";

type NavMenuProps = {
  menuItems: AccountMenuItem[];
  activeSection: AccountSectionKey;
  setActiveSection: (key: AccountSectionKey) => void;
};

export default function NavMenu({ menuItems, activeSection, setActiveSection }: NavMenuProps) {
  return (
    <>
      <nav className="space-y-1" aria-label="Menu tài khoản">
        {menuItems.map((item) => {
          const Icon = item.icon
          const isActive = activeSection === item.key

          return (
            <Button
              key={item.key}
              type="button"
              variant="ghost"
              className={
                `h-11 w-full justify-start rounded-xl px-3 text-sm
                              focus-visible:ring-2 focus-visible:ring-emerald-500
                              ${isActive ? "bg-emerald-50 text-emerald-800 shadow-[inset_0_0_0_1px_rgba(5,150,105,0.3)]" : "text-slate-700 hover:bg-slate-100"}`
              }
              onClick={() => setActiveSection(item.key)}
              aria-label={item.label}
            >
              <Icon className="mr-2 size-4" />
              {item.label}
            </Button>
          )
        })}
      </nav>
      <Separator />
      <Button
        type="button"
        variant="ghost"
        className="h-11 w-full justify-start rounded-xl px-3 text-sm text-rose-600 hover:bg-rose-50 hover:text-rose-700 focus-visible:ring-2 focus-visible:ring-rose-500"
        aria-label="Đăng xuất"
      >
        <LogOut className="mr-2 size-4" />
        Đăng xuất
      </Button>
    </>
  );
}