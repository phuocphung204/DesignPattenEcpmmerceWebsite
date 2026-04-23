"use client";

import Link from "next/link";
import { useEffect, useMemo, useState } from "react";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { Button } from "@/components/ui/button";
import {
  Combobox,
  ComboboxContent,
  ComboboxEmpty,
  ComboboxInput,
  ComboboxItem,
  ComboboxList,
} from "@/components/ui/combobox";
import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import {
  AlertCircleIcon,
  Eye,
  EyeOff,
  Lock,
  Mail,
  MapPin,
  Phone,
  User,
} from "lucide-react";
import { District, Province, Ward } from "@/services/address-api";
import { registerAction } from "@/features/auth/register-action";
import getErrorMessage from "@/utils/error-helper";
import { toast } from "sonner";
import { useRouter } from "next/navigation";
import {
  getDistricts as loadDistrictsAction,
  getProvinces as loadProvincesAction,
  getWards as loadWardsAction,
} from "@/features/address-action";
import { registerData } from "./register-data";
import { addressSchema } from "@/features/address";

const registerSchema = z.object({
  fullName: z.string().min(2, "Họ tên phải có ít nhất 2 ký tự"),
  email: z.string().email("Email không hợp lệ"),
  phoneNumber: z.string().min(9, "Số điện thoại không hợp lệ"),
  password: z.string().min(8, "Mật khẩu phải có ít nhất 8 ký tự"),
  confirmPassword: z.string().min(8, "Mật khẩu xác nhận phải có ít nhất 8 ký tự"),
  address: addressSchema,
}).refine((data) =>
  // nếu false thì thêm lỗi vào confirmPassword, nếu true thì không có lỗi
  data.password === data.confirmPassword, {
  path: ["confirmPassword"],
  message: "Mật khẩu xác nhận không khớp",
});

export type RegisterFormData = z.infer<typeof registerSchema>;

type RegisterFormProps = {
  initialProvinces: Province[];
};

const testData = registerData;

export default function RegisterForm({ initialProvinces }: RegisterFormProps) {
  const router = useRouter();
  const [provinces, setProvinces] = useState<Province[]>(initialProvinces);
  const [districts, setDistricts] = useState<District[]>([]);
  const [wards, setWards] = useState<Ward[]>([]);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
  const [isPasswordVisible, setIsPasswordVisible] = useState<boolean>(false);

  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors, isValid },
  } = useForm<RegisterFormData>({
    resolver: zodResolver(registerSchema),
    mode: "onChange",
    defaultValues: {
      ...testData
    }
  });

  const provinceCode = watch("address.provinceCode");
  const districtCode = watch("address.districtCode");
  const wardCode = watch("address.wardCode");

  // load danh sách tỉnh/thành phố khi component mount nếu chưa có dữ liệu từ props
  useEffect(() => {
    if (initialProvinces.length > 0) {
      return;
    }

    const loadProvinces = async () => {
      const data = await loadProvincesAction();
      setProvinces(data);
    };

    loadProvinces();
  }, [initialProvinces]);

  // load danh sách quận/huyện khi người dùng chọn tỉnh/thành phố
  useEffect(() => {
    if (!provinceCode) {
      setDistricts([]);
      setWards([]);
      return;
    }

    const loadDistricts = async () => {
      const districts = await loadDistrictsAction(Number(provinceCode));
      setDistricts(districts);
      setWards([]);

      setValue("address.district", "");
      setValue("address.ward", "");
      setValue("address.districtCode", "");
      setValue("address.wardCode", "");
    };

    loadDistricts();
  }, [provinceCode, setValue]);

  // load danh sách phường/xã khi người dùng chọn quận/huyện
  useEffect(() => {
    if (!districtCode) {
      setWards([]);
      return;
    }

    const loadWards = async () => {
      const wards = await loadWardsAction(Number(districtCode));
      setWards(wards);

      setValue("address.ward", "");
      setValue("address.wardCode", "");
    };

    loadWards();
  }, [districtCode, setValue]);

  const provinceMap = useMemo(() => {
    return new Map(provinces.map((province) => [String(province.code), province.name]));
  }, [provinces]);

  const districtMap = useMemo(() => {
    return new Map(districts.map((district) => [String(district.code), district.name]));
  }, [districts]);

  const wardMap = useMemo(() => {
    return new Map(wards.map((ward) => [String(ward.code), ward.name]));
  }, [wards]);

  const selectedProvince = useMemo(() => {
    return provinces.find((province) => String(province.code) === provinceCode) ?? null;
  }, [provinceCode, provinces]);

  const selectedDistrict = useMemo(() => {
    return districts.find((district) => String(district.code) === districtCode) ?? null;
  }, [districtCode, districts]);

  const selectedWard = useMemo(() => {
    return wards.find((ward) => String(ward.code) === wardCode) ?? null;
  }, [wardCode, wards]);

  const onSubmit = handleSubmit(async (data) => {
    setErrorMessage(null);
    setIsSubmitting(true);

    const result = await registerAction(data);

    if (!result.payload?.success) {
      console.error(">>> Registration failed:", result.payload);
      setErrorMessage(getErrorMessage(result.payload?.errorCode || null));
      setIsSubmitting(false);
      return;
    }

    toast.success("Đăng ký thành công! Hệ thống đang chuyển hướng sang trang đăng nhập.");
    setIsSubmitting(false);
    router.replace("/dang-nhap");
  });

  return (
    <section className="rounded-3xl border border-white/70 bg-white/90 p-6 shadow-[0_28px_80px_-34px_rgba(15,23,42,0.55)] backdrop-blur-sm sm:p-8">
      <div className="mb-6">
        <p className="inline-flex items-center rounded-full bg-orange-50 px-3 py-1 text-xs font-semibold tracking-[0.12em] text-orange-600 uppercase">
          New Member
        </p>
        <h2 className="mt-3 text-2xl font-semibold text-slate-900 sm:text-3xl">Tạo tài khoản TechStore</h2>
        <p className="mt-2 text-sm leading-relaxed text-slate-600">
          Đăng ký để lưu giỏ hàng, theo dõi đơn mua linh kiện và nhận ưu đãi thành viên dành riêng cho bạn.
        </p>
      </div>

      <form onSubmit={onSubmit} className="space-y-5" noValidate>
        <Field>
          <FieldLabel htmlFor="fullName" className="text-slate-700">Họ và tên</FieldLabel>
          <div className="relative">
            <User className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-slate-400" />
            <Input
              id="fullName"
              {...register("fullName")}
              className="h-11 rounded-xl border-slate-200 bg-white pl-9 pr-3 text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200"
            />
          </div>
          <FieldError>{errors.fullName?.message}</FieldError>
        </Field>

        <div className="grid gap-4 sm:grid-cols-2">
          <Field>
            <FieldLabel htmlFor="email" className="text-slate-700">Email</FieldLabel>
            <div className="relative">
              <Mail className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-slate-400" />
              <Input
                id="email"
                type="email"
                autoComplete="email"
                {...register("email")}
                className="h-11 rounded-xl border-slate-200 bg-white pl-9 pr-3 text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200"
              />
            </div>
            <FieldError>{errors.email?.message}</FieldError>
          </Field>

          <Field>
            <FieldLabel htmlFor="phoneNumber" className="text-slate-700">Số điện thoại</FieldLabel>
            <div className="relative">
              <Phone className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-slate-400" />
              <Input
                id="phoneNumber"
                autoComplete="tel"
                {...register("phoneNumber")}
                className="h-11 rounded-xl border-slate-200 bg-white pl-9 pr-3 text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200"
              />
            </div>
            <FieldError>{errors.phoneNumber?.message}</FieldError>
          </Field>
        </div>

        <div className="grid gap-4 sm:grid-cols-2">
          <Field>
            <FieldLabel htmlFor="password" className="text-slate-700">Mật khẩu</FieldLabel>
            <div className="relative">
              <Lock className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-slate-400" />
              <Input
                id="password"
                type={isPasswordVisible ? "text" : "password"}
                autoComplete="new-password"
                {...register("password")}
                className="h-11 rounded-xl border-slate-200 bg-white pl-9 pr-11 text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200"
              />
              <button
                type="button"
                aria-label={isPasswordVisible ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
                onClick={() => setIsPasswordVisible((prev) => !prev)}
                className="absolute top-1/2 right-2 inline-flex h-7 w-7 -translate-y-1/2 items-center justify-center rounded-md text-slate-500 transition hover:bg-slate-100 hover:text-slate-700 focus-visible:ring-2 focus-visible:ring-emerald-500"
              >
                {isPasswordVisible ? <EyeOff className="size-4" /> : <Eye className="size-4" />}
              </button>
            </div>
            <FieldError>{errors.password?.message}</FieldError>
          </Field>

          <Field>
            <FieldLabel htmlFor="confirmPassword" className="text-slate-700">Xác nhận mật khẩu</FieldLabel>
            <div className="relative">
              <Lock className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-slate-400" />
              <Input
                id="confirmPassword"
                type={isPasswordVisible ? "text" : "password"}
                autoComplete="new-password"
                {...register("confirmPassword")}
                className="h-11 rounded-xl border-slate-200 bg-white pl-9 pr-11 text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200"
              />
              <button
                type="button"
                aria-label={isPasswordVisible ? "Ẩn mật khẩu" : "Hiện mật khẩu"}
                onClick={() => setIsPasswordVisible((prev) => !prev)}
                className="absolute top-1/2 right-2 inline-flex h-7 w-7 -translate-y-1/2 items-center justify-center rounded-md text-slate-500 transition hover:bg-slate-100 hover:text-slate-700 focus-visible:ring-2 focus-visible:ring-emerald-500"
              >
                {isPasswordVisible ? <EyeOff className="size-4" /> : <Eye className="size-4" />}
              </button>
            </div>
            <FieldError>{errors.confirmPassword?.message}</FieldError>
          </Field>
        </div>
        <div className="rounded-2xl border border-slate-200 bg-slate-50/70 p-4 sm:p-5">
          <p className="mb-4 text-sm font-semibold text-slate-800">Địa chỉ nhận hàng mặc định</p>

          <div className="grid gap-4 sm:grid-cols-2">
            <Field>
              <FieldLabel htmlFor="address.receiverName" className="text-slate-700">Người nhận</FieldLabel>
              <Input
                id="address.receiverName"
                {...register("address.receiverName")}
                className="h-11 rounded-xl border-slate-200 bg-white text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200"
              />
              <FieldError>{errors.address?.receiverName?.message}</FieldError>
            </Field>

            <Field>
              <FieldLabel htmlFor="address.phoneNumber" className="text-slate-700">SĐT nhận hàng</FieldLabel>
              <Input
                id="address.phoneNumber"
                {...register("address.phoneNumber")}
                className="h-11 rounded-xl border-slate-200 bg-white text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200"
              />
              <FieldError>{errors.address?.phoneNumber?.message}</FieldError>
            </Field>

            <Field>
              <FieldLabel htmlFor="address.country" className="text-slate-700">Quốc gia</FieldLabel>
              <Input
                id="address.country"
                {...register("address.country")}
                className="h-11 rounded-xl border-slate-200 bg-white text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200"
              />
              <FieldError>{errors.address?.country?.message}</FieldError>
            </Field>

            <Field>
              <FieldLabel htmlFor="address.street" className="text-slate-700">Số nhà, tên đường</FieldLabel>
              <div className="relative">
                <MapPin className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-slate-400" />
                <Input
                  id="address.street"
                  {...register("address.street")}
                  className="h-11 rounded-xl border-slate-200 bg-white pl-9 pr-3 text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200"
                />
              </div>
              <FieldError>{errors.address?.street?.message}</FieldError>
            </Field>
          </div>

          {/* phần chọn tỉnh/thành, quận/huyện, phường/xã */}
          <div className="mt-4 grid gap-4 sm:grid-cols-3">
            <Field>
              <FieldLabel htmlFor="address.provinceCode" className="text-slate-700">Tỉnh/Thành phố</FieldLabel>
              <Combobox
                items={provinces}
                value={selectedProvince ?? null}
                itemToStringLabel={(item) => item.name}
                itemToStringValue={(item) => String(item.code)}
                onValueChange={(value) => {
                  if (!value) {
                    setValue("address.provinceCode", "", { shouldDirty: true, shouldValidate: true });
                    setValue("address.province", "", { shouldDirty: true, shouldValidate: true });
                    setValue("address.district", "", { shouldDirty: true, shouldValidate: true });
                    setValue("address.ward", "", { shouldDirty: true, shouldValidate: true });
                    setValue("address.districtCode", "", { shouldDirty: true, shouldValidate: true });
                    setValue("address.wardCode", "", { shouldDirty: true, shouldValidate: true });
                    return;
                  }

                  const code = String(value.code);
                  setValue("address.provinceCode", code, { shouldDirty: true, shouldValidate: true });
                  setValue("address.province", provinceMap.get(code) ?? value.name, {
                    shouldDirty: true,
                    shouldValidate: true,
                  });
                }}
              >
                <ComboboxInput
                  id="address.provinceCode"
                  placeholder="Chọn tỉnh/thành"
                  showClear
                  aria-invalid={Boolean(errors.address?.provinceCode || errors.address?.province)}
                  className="h-11 w-full rounded-xl border-slate-200 bg-white text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200"
                />
                <ComboboxContent>
                  <ComboboxEmpty>Không tìm thấy tỉnh/thành</ComboboxEmpty>
                  <ComboboxList>
                    {(province) => (
                      <ComboboxItem key={province.code} value={province}>
                        {province.name}
                      </ComboboxItem>
                    )}
                  </ComboboxList>
                </ComboboxContent>
              </Combobox>
              <input type="hidden" {...register("address.provinceCode")} />
              <input type="hidden" {...register("address.province")} />
              <FieldError>{errors.address?.province?.message}</FieldError>
            </Field>

            <Field>
              <FieldLabel htmlFor="address.districtCode" className="text-slate-700">Quận/Huyện</FieldLabel>
              <Combobox
                items={districts}
                value={selectedDistrict ?? null}
                itemToStringLabel={(item) => item.name}
                itemToStringValue={(item) => String(item.code)}
                onValueChange={(value) => {
                  if (!value) {
                    setValue("address.districtCode", "", { shouldDirty: true, shouldValidate: true });
                    setValue("address.district", "", { shouldDirty: true, shouldValidate: true });
                    setValue("address.ward", "", { shouldDirty: true, shouldValidate: true });
                    setValue("address.wardCode", "", { shouldDirty: true, shouldValidate: true });
                    return;
                  }

                  const code = String(value.code);
                  setValue("address.districtCode", code, { shouldDirty: true, shouldValidate: true });
                  setValue("address.district", districtMap.get(code) ?? value.name, {
                    shouldDirty: true,
                    shouldValidate: true,
                  });
                }}
              >
                <ComboboxInput
                  id="address.districtCode"
                  placeholder="Chọn quận/huyện"
                  showClear
                  disabled={!provinceCode}
                  aria-invalid={Boolean(errors.address?.districtCode || errors.address?.district)}
                  className="h-11 w-full rounded-xl border-slate-200 bg-white text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200 disabled:cursor-not-allowed disabled:bg-slate-100"
                />
                <ComboboxContent>
                  <ComboboxEmpty>Không tìm thấy quận/huyện</ComboboxEmpty>
                  <ComboboxList>
                    {(district) => (
                      <ComboboxItem key={district.code} value={district}>
                        {district.name}
                      </ComboboxItem>
                    )}
                  </ComboboxList>
                </ComboboxContent>
              </Combobox>
              <input type="hidden" {...register("address.districtCode")} />
              <input type="hidden" {...register("address.district")} />
              <FieldError>{errors.address?.district?.message}</FieldError>
            </Field>

            <Field>
              <FieldLabel htmlFor="address.wardCode" className="text-slate-700">Phường/Xã</FieldLabel>
              <Combobox
                items={wards}
                value={selectedWard ?? null}
                itemToStringLabel={(item) => item.name}
                itemToStringValue={(item) => String(item.code)}
                onValueChange={(value) => {
                  if (!value) {
                    setValue("address.wardCode", "", { shouldDirty: true, shouldValidate: true });
                    setValue("address.ward", "", { shouldDirty: true, shouldValidate: true });
                    return;
                  }

                  const code = String(value.code);
                  setValue("address.wardCode", code, { shouldDirty: true, shouldValidate: true });
                  setValue("address.ward", wardMap.get(code) ?? value.name, {
                    shouldDirty: true,
                    shouldValidate: true,
                  });
                }}
              >
                <ComboboxInput
                  id="address.wardCode"
                  placeholder="Chọn phường/xã"
                  showClear
                  disabled={!districtCode}
                  aria-invalid={Boolean(errors.address?.wardCode || errors.address?.ward)}
                  className="h-11 w-full rounded-xl border-slate-200 bg-white text-slate-800 transition duration-200 focus-visible:border-emerald-500 focus-visible:ring-emerald-200 disabled:cursor-not-allowed disabled:bg-slate-100"
                />
                <ComboboxContent>
                  <ComboboxEmpty>Không tìm thấy phường/xã</ComboboxEmpty>
                  <ComboboxList>
                    {(ward) => (
                      <ComboboxItem key={ward.code} value={ward}>
                        {ward.name}
                      </ComboboxItem>
                    )}
                  </ComboboxList>
                </ComboboxContent>
              </Combobox>
              <input type="hidden" {...register("address.wardCode")} />
              <input type="hidden" {...register("address.ward")} />
              <FieldError>{errors.address?.ward?.message}</FieldError>
            </Field>

          </div>
        </div>

        <Button
          type="submit"
          variant="default"
          disabled={isSubmitting || !isValid}
          className="h-11 w-full rounded-xl bg-emerald-600 text-base font-semibold text-white transition duration-200 hover:bg-emerald-700"
        >
          {isSubmitting ? "Đang tạo tài khoản..." : "Tạo tài khoản"}
        </Button>
      </form>

      {errorMessage && (
        <Alert variant="destructive" className="mt-4 rounded-xl border-red-200 bg-red-50 text-red-800">
          <AlertCircleIcon className="size-4" />
          <AlertTitle>Đăng ký thất bại</AlertTitle>
          <AlertDescription>{errorMessage}</AlertDescription>
        </Alert>
      )}

      <p className="mt-6 text-center text-sm text-slate-600">
        <Link href="/dang-nhap" className="font-semibold mr-1 text-slate-600 hover:underline focus-visible:ring-2 focus-visible:ring-slate-400">
          Đã có tài khoản?
        </Link>
        <Link href="/" className="font-semibold text-orange-600 underline-offset-4 hover:underline focus-visible:ring-2 focus-visible:ring-orange-400">
          Xem ưu đãi linh kiện hôm nay
        </Link>
      </p>
    </section>
  );
}
