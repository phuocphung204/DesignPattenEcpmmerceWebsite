"use client"

import { useMemo, useState } from "react"
import type { LucideIcon } from "lucide-react"
import { MapPinHouse, PackageSearch, ShieldCheck, UserRound } from "lucide-react"

export type AccountSectionKey =
  | "account-info"
  | "order-history"
  | "address-book"
  | "security-settings"

type SectionStat = {
  label: string
  value: string
  hint: string
}

type SectionAction = {
  label: string
  description: string
}

export type AccountSectionContent = {
  title: string
  subtitle: string
  stats: SectionStat[]
  actions: SectionAction[]
}

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

const accountSectionContentMap: Record<AccountSectionKey, AccountSectionContent> = {
  "account-info": {
    title: "Hồ sơ khách hàng",
    subtitle:
      "Cập nhật thông tin cá nhân để nhận ưu đãi đúng nhu cầu laptop, PC và linh kiện.",
    stats: [
      { label: "Hạng thành viên", value: "Gold", hint: "Hiệu lực đến 12/2026" },
      { label: "Điểm tích lũy", value: "1,250", hint: "Còn 250 điểm lên Platinum" },
      { label: "Email xác thực", value: "Đã xác minh", hint: "Bảo vệ giao dịch tốt hơn" },
    ],
    actions: [
      {
        label: "Cập nhật hồ sơ",
        description: "Sửa tên, số điện thoại và sở thích sản phẩm ngay trong 1 phút.",
      },
      {
        label: "Đồng bộ thông báo",
        description: "Nhận alert khi linh kiện bạn quan tâm giảm giá hoặc về hàng.",
      },
    ],
  },
  "order-history": {
    title: "Theo dõi đơn hàng",
    subtitle:
      "Quản lý tiến độ giao hàng, hóa đơn và trạng thái bảo hành cho từng đơn mua.",
    stats: [
      { label: "Đơn hoàn tất", value: "24", hint: "12 tháng gần nhất" },
      { label: "Đang giao", value: "2", hint: "Dự kiến nhận trong 48 giờ" },
      { label: "Đổi trả", value: "1", hint: "Đã xử lý thành công" },
    ],
    actions: [
      {
        label: "Xem timeline giao hàng",
        description: "Theo dõi chặng vận chuyển theo thời gian thực cho mọi đơn gần đây.",
      },
      {
        label: "Tải hóa đơn VAT",
        description: "Xuất hóa đơn điện tử phục vụ kế toán doanh nghiệp.",
      },
    ],
  },
  "address-book": {
    title: "Sổ địa chỉ giao nhận",
    subtitle:
      "Lưu nhiều địa chỉ cho văn phòng, phòng game hoặc nhà riêng để checkout nhanh hơn.",
    stats: [
      { label: "Địa chỉ đã lưu", value: "3", hint: "1 mặc định cho đơn gấp" },
      { label: "Khu vực ưu tiên", value: "Nội thành", hint: "Hỗ trợ giao nhanh 2h" },
      { label: "Tỉ lệ giao đúng hẹn", value: "98%", hint: "6 tháng gần nhất" },
    ],
    actions: [
      {
        label: "Thêm địa chỉ mới",
        description: "Tạo nhanh điểm nhận hàng theo mẫu có sẵn và gợi ý địa danh.",
      },
      {
        label: "Thiết lập ghi chú shipper",
        description: "Đính kèm lưu ý chi tiết để giao linh kiện an toàn, đúng nơi.",
      },
    ],
  },
  "security-settings": {
    title: "Bảo mật và quyền riêng tư",
    subtitle:
      "Kiểm soát mật khẩu, phiên đăng nhập và phương thức xác thực để bảo vệ tài khoản.",
    stats: [
      { label: "Mật khẩu", value: "Mạnh", hint: "Đổi lần cuối 14 ngày trước" },
      { label: "Thiết bị tin cậy", value: "4", hint: "1 phiên lạ cần rà soát" },
      { label: "Xác thực 2 lớp", value: "Đang bật", hint: "OTP qua ứng dụng" },
    ],
    actions: [
      {
        label: "Quản lý phiên đăng nhập",
        description: "Xem toàn bộ thiết bị đã đăng nhập và thu hồi phiên bất thường.",
      },
      {
        label: "Cập nhật mật khẩu",
        description: "Đặt mật khẩu mới theo chuẩn bảo mật cho giao dịch online.",
      },
    ],
  },
}

export function useAccountManagement() {
  const [activeSection, setActiveSection] = useState<AccountSectionKey>("account-info")

  const activeContent = useMemo(
    () => accountSectionContentMap[activeSection],
    [activeSection]
  )

  return {
    menuItems: accountMenuItems,
    activeSection,
    setActiveSection,
    activeContent,
  }
}