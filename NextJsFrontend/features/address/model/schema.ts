import { z } from "zod";

export const addressSchema = z.object({
  receiverName: z.string().min(2, "Tên người nhận phải có ít nhất 2 ký tự"),
  phoneNumber: z.string().regex(/^0\d{9,10}$/, "Số điện thoại nhận hàng không hợp lệ"),
  province: z.string().min(1, "Vui lòng chọn tỉnh/thành phố"),
  country: z.string().min(1, "Vui lòng nhập quốc gia"),
  district: z.string().min(1, "Vui lòng chọn quận/huyện"),
  ward: z.string().min(1, "Vui lòng chọn phường/xã"),
  street: z.string().min(2, "Vui lòng nhập số nhà, tên đường"),
  provinceCode: z.string().min(1),
  districtCode: z.string().min(1),
  wardCode: z.string().min(1),
});