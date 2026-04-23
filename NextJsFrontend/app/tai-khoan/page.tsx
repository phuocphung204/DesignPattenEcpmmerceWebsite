"use client"
import { LucideIcon, MapPinHouse, PackageSearch, ShieldCheck, Sparkles, Star, UserRound } from "lucide-react";
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { AccountSectionKey } from "./_hooks/use-account-management";
import NavMenu from "./_components/NavMenu"
import ProfileSection from "./_components/ProfileSection"
import AddressesSection from "./_components/AddressesSection";
import { useState } from "react";
import UserAvatar from "@/entities/user/ui/UserAvatar";
import { CurrentUserType, useGetCurrentUser } from "@/entities/user";

type SectionContent = {
  title: string
  subtitle: string
  component: React.ReactNode
}

const AccountSections = {
  "account-info": {
    title: "Hồ sơ khách hàng",
    subtitle:
      "Cập nhật thông tin cá nhân để nhận ưu đãi đúng nhu cầu laptop, PC và linh kiện.",
    component: <ProfileSection />,
  },
  "order-history": {
    title: "Theo dõi đơn hàng",
    subtitle:
      "Quản lý tiến độ giao hàng, hóa đơn và trạng thái bảo hành cho từng đơn mua.",
    component: <div>Order History Content</div>,
  },
  "address-book": {
    title: "Sổ địa chỉ giao nhận",
    subtitle:
      "Lưu nhiều địa chỉ cho văn phòng, phòng game hoặc nhà riêng để checkout nhanh hơn.",
    component: <AddressesSection />,
  },
  "security-settings": {
    title: "Bảo mật và quyền riêng tư",
    subtitle:
      "Kiểm soát mật khẩu, phiên đăng nhập và phương thức xác thực để bảo vệ tài khoản.",
    component: <div>Security Settings Content</div>,
  },
} satisfies Record<AccountSectionKey, SectionContent>

export type AccountMenuItem = {
  key: AccountSectionKey
  label: string
  icon: LucideIcon
}

const accountMenuItems: AccountMenuItem[] = [
  {
    key: "account-info",
    label: "Thông tin tài khoản",
    icon: UserRound,
  },
  {
    key: "order-history",
    label: "Lịch sử đơn hàng",
    icon: PackageSearch,
  },
  {
    key: "address-book",
    label: "Sổ địa chỉ",
    icon: MapPinHouse,
  },
  {
    key: "security-settings",
    label: "Cài đặt bảo mật",
    icon: ShieldCheck,
  },
]
type NeedProfileDataType = Pick<CurrentUserType, "fullName" | "email" | "loyaltyPoints">

export default function AccountManagementProfilePage() {
  const { data: userInfo } = useGetCurrentUser<NeedProfileDataType>((user) => ({
    fullName: user.fullName,
    email: user.email,
    loyaltyPoints: user.loyaltyPoints
  }));
  const [activeSection, setActiveSection] = useState<AccountSectionKey>("account-info");

  return (
    <main className="mx-auto w-full max-w-7xl px-4 py-6 sm:px-6 lg:px-8 lg:py-10">
      <div className="grid grid-cols-1 gap-5 lg:grid-cols-12 lg:gap-6">

        <aside className="lg:col-span-4 xl:col-span-3">
          <Card className="pt-0 h-full overflow-hidden rounded-3xl border-emerald-200/70 bg-white/85 shadow-[0_26px_65px_-36px_rgba(5,150,105,0.5)] backdrop-blur-sm transition-transform duration-200 hover:-translate-y-0.5">
            {/* Profile Header */}
            <CardHeader className="space-y-4 pt-6 bg-[linear-gradient(135deg,rgba(5,150,105,0.12),rgba(249,115,22,0.12),rgba(236,253,245,1))] pb-4">
              <div className="flex items-center gap-3">
                <UserAvatar className="size-14" />
                <div className="space-y-1">
                  <p className="text-lg font-semibold tracking-tight text-slate-900">{userInfo?.fullName}</p>
                  <p className="text-xs text-slate-600">{userInfo?.email}</p>
                </div>
              </div>
              <Badge className="w-fit rounded-full bg-linear-to-r from-emerald-600 to-orange-500 px-3 py-1 text-white">
                <Star className="mr-1 size-3.5 fill-current" /> {userInfo?.loyaltyPoints || 0} điểm thành viên
              </Badge>
            </CardHeader>

            {/* Navigation Menu */}
            <CardContent className="space-y-4 p-4">
              <NavMenu
                menuItems={accountMenuItems}
                activeSection={activeSection}
                setActiveSection={setActiveSection}
              />
            </CardContent>
          </Card>
        </aside>

        <section className="lg:col-span-8 xl:col-span-9">
          <Card className="pt-0 min-h-135 overflow-hidden rounded-3xl border-slate-200 bg-white/90 shadow-[0_28px_70px_-38px_rgba(15,23,42,0.35)] backdrop-blur-sm">
            <CardHeader className="space-y-3 pt-6 border-b bg-[linear-gradient(120deg,rgba(236,253,245,0.85),rgba(255,247,237,0.85),rgba(241,245,249,0.9))] pb-5">
              <div className="inline-flex w-fit items-center gap-2 rounded-full bg-emerald-100 px-3 py-1 text-xs font-semibold text-emerald-800">
                <Sparkles className="size-3.5" />
                Customer Center
              </div>
              <CardTitle className="text-2xl font-bold tracking-tight text-slate-900 sm:text-3xl">
                {AccountSections[activeSection].title}
              </CardTitle>
              <p className="max-w-3xl text-sm leading-relaxed text-slate-600 sm:text-base">
                {AccountSections[activeSection].subtitle}
              </p>
            </CardHeader>

            <CardContent className="space-y-6 p-4 sm:p-6">
              {AccountSections[activeSection].component}
              {/* <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
                {activeContent.stats.map((stat) => (
                  <Card
                    key={stat.label}
                    className="animate-in fade-in zoom-in-95 rounded-2xl border-emerald-100 bg-[linear-gradient(160deg,#ffffff_0%,#f0fdf4_100%)] shadow-[0_20px_45px_-34px_rgba(5,150,105,0.55)] duration-200 hover:-translate-y-0.5"
                  >
                    <CardContent className="space-y-2 p-4">
                      <p className="text-xs uppercase tracking-wide text-slate-500">{stat.label}</p>
                      <p className="text-2xl font-bold text-slate-900">{stat.value}</p>
                      <p className="text-xs text-slate-600">{stat.hint}</p>
                    </CardContent>
                  </Card>
                ))}
              </div> */}

              {/* <div className="grid grid-cols-1 gap-4 xl:grid-cols-2">
                {activeContent.actions.map((action) => (
                  <Card
                    key={action.label}
                    className="rounded-2xl border-slate-200 bg-[linear-gradient(145deg,#ffffff_0%,#f8fafc_50%,#fff7ed_100%)] transition-all duration-200 hover:-translate-y-0.5 hover:shadow-[0_24px_48px_-34px_rgba(249,115,22,0.6)]"
                  >
                    <CardContent className="space-y-4 p-5">
                      <div className="space-y-2">
                        <h3 className="text-lg font-semibold text-slate-900">{action.label}</h3>
                        <p className="text-sm leading-relaxed text-slate-600">{action.description}</p>
                      </div>
                      <Button
                        type="button"
                        size="sm"
                        className="rounded-lg bg-orange-500 text-white hover:bg-orange-600 focus-visible:ring-2 focus-visible:ring-orange-500"
                        aria-label={action.label}
                      >
                        Thực hiện ngay
                      </Button>
                    </CardContent>
                  </Card>
                ))}
              </div> */}
            </CardContent>
          </Card>
        </section>
      </div>
    </main>
  )
}