import type { FooterColumn } from "@/features/type";

export type ValueIconKey = "gift" | "shield" | "rocket" | "wrench";

export interface ValuePropositionItem {
  id: string;
  iconKey: ValueIconKey;
  title: string;
  description: string;
}

export const siteConfig: {
  site: {
    name: string;
    description: string;
    contact: {
      address: string;
      phone: string;
      email: string;
    };
  };
  valuePropositions: ValuePropositionItem[];
  footerColumns: FooterColumn[];
  newsletterDescription: string;
} = {
  site: {
    name: "TechStore",
    description: "Chuyên cung cấp Laptop, PC và linh kiện máy tính cao cấp với cam kết chất lượng hàng đầu.",
    contact: {
      address: "Khu Công nghệ cao, Thành phố Thủ Đức, TP. Hồ Chí Minh",
      phone: "1900 xxxx",
      email: "support@techstore.vn",
    },
  },
  valuePropositions: [
    {
      id: "vp-1",
      iconKey: "gift",
      title: "Tích điểm lên đến 10%",
      description:
        "Áp dụng cho mọi hóa đơn khi bạn đăng nhập thành viên. Dùng điểm để quy đổi thành mã giảm giá cho lần mua linh kiện tiếp theo.",
    },
    {
      id: "vp-2",
      iconKey: "shield",
      title: "Bảo hành phần cứng chính hãng",
      description:
        "Tất cả máy tính và linh kiện đều nguyên seal, bảo hành theo tiêu chuẩn của nhà sản xuất (ASUS, Dell, Logitech...).",
    },
    {
      id: "vp-3",
      iconKey: "rocket",
      title: "Giao hàng hỏa tốc",
      description:
        "Miễn phí vận chuyển toàn quốc cho đơn hàng từ 2.000.000 VNĐ. Đóng gói chống sốc chuyên dụng cho đồ điện tử.",
    },
    {
      id: "vp-4",
      iconKey: "wrench",
      title: "Hỗ trợ phần mềm & kỹ thuật trọn đời",
      description:
        "Đội ngũ IT sẵn sàng tư vấn cấu hình, lắp ráp linh kiện và xử lý lỗi kỹ thuật 24/7.",
    },
  ],
  footerColumns: [
    {
      title: "Chính sách khách hàng",
      links: [
        { title: "Chính sách bảo hành linh kiện", href: "#" },
        { title: "Chính sách đổi trả & Hoàn tiền (7 ngày)", href: "#" },
        { title: "Chính sách bảo mật thông tin", href: "#" },
        { title: "Quy định giao hàng & Kiểm hàng", href: "#" },
      ],
    },
    {
      title: "Hỗ trợ nhanh",
      links: [
        { title: "Hướng dẫn đặt hàng online", href: "#" },
        { title: "Tra cứu thông tin đơn hàng", href: "#" },
        { title: "Hướng dẫn trả góp", href: "#" },
        { title: "Bảng giá dịch vụ vệ sinh/bảo dưỡng Laptop", href: "#" },
      ],
    },
  ],
  newsletterDescription: "Đăng ký email để nhận mã giảm giá linh kiện độc quyền hàng tháng!",
};

export default siteConfig;