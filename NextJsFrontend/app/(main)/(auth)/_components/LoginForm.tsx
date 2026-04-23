"use client";;
import Link from "next/link";
import { useEffect, useState } from "react";
import { Field, FieldError, FieldLabel } from "@/components/ui/field";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { toast } from "sonner";
import getErrorMessage from "@/utils/error-helper";
import { AlertCircleIcon, Eye, EyeOff, Lock, Mail } from "lucide-react";
import { useRouter } from "next/navigation";
import { HttpError, MyCustomResponse } from "@/services/http";
import { useQueryClient } from "@tanstack/react-query";
import { CURRENT_USER_KEY, useIsAuthenticated } from "@/entities/user";
import authRequest from "@/services/backend-requests/auth-request";
import Cookies from "js-cookie";

const loginSchema = z.object({
  email: z.string().email("Email không hợp lệ"),
  password: z.string().min(8, "Mật khẩu phải có ít nhất 8 ký tự"),
});

export type LoginFormData = z.infer<typeof loginSchema>;

export default function LoginForm() {
  const { data: isAuthenticated } = useIsAuthenticated();
  const router = useRouter();
  const queryClient = useQueryClient();
  const [shouldRedirect, setShouldRedirect] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
  const [isPasswordVisible, setIsPasswordVisible] = useState<boolean>(false);

  const {
    register,
    handleSubmit,
    formState: { errors, isValid },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      // email: "sangvo200@gmail.com",
      // password: "123333333333333333333",
    },
    mode: "onChange",
  });

  const onSubmit = async (data: LoginFormData) => {
    setErrorMessage(null);
    setIsSubmitting(true);

    try {
      const res = await authRequest.login(data);
      // set store với data user trả về từ API
      const jwtToken = res.payload?.data?.token;
      if (jwtToken) {
        Cookies.set("jwtToken", jwtToken);
      }
      queryClient.setQueryData(CURRENT_USER_KEY, res.payload?.data || null);
      toast.success("Đăng nhập thành công!");
      setShouldRedirect(true)
    } catch (error) {
      // console.log(error, error?.constructor?.name, error instanceof Error);
      // console.error("Login error:", error, error instanceof HttpError);
      if (error instanceof HttpError) {
        const payload = error.payload as MyCustomResponse;
        setErrorMessage(getErrorMessage(payload.errorCode));
      } else {
        setErrorMessage("Đã xảy ra lỗi không xác định.");
      }
    }
    finally {
      setIsSubmitting(false);
    }
  };

  // clean up timer on unmount
  useEffect(() => {
    console.log("isAuthenticated", isAuthenticated, "shouldRedirect", shouldRedirect);
    if (!shouldRedirect && !isAuthenticated) return;
    if (isAuthenticated === false) return;

    const timerId = setTimeout(() => {
      router.replace("/");
    }, 500);

    return () => clearTimeout(timerId);
  }, [shouldRedirect, router, isAuthenticated]);

  return (
    <section className="rounded-3xl border border-white/70 bg-white/90 p-6 shadow-[0_28px_80px_-34px_rgba(15,23,42,0.55)] backdrop-blur-sm sm:p-8">
      <div className="mb-6">
        <p className="inline-flex items-center rounded-full bg-orange-50 px-3 py-1 text-xs font-semibold tracking-[0.12em] text-orange-600 uppercase">
          Member Login
        </p>
        <h2 className="mt-3 text-2xl font-semibold text-slate-900 sm:text-3xl">Chào mừng bạn quay lại</h2>
        <p className="mt-2 text-sm leading-relaxed text-slate-600">
          Đăng nhập để quản lý đơn hàng, nhận ưu đãi cá nhân hóa và đồng bộ cấu hình thiết bị của bạn.
        </p>
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-5" noValidate>
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
          <FieldLabel htmlFor="password" className="text-slate-700">Mật khẩu</FieldLabel>
          <div className="relative">
            <Lock className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-slate-400" />
            <Input
              id="password"
              type={isPasswordVisible ? "text" : "password"}
              autoComplete="current-password"
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

        <div className="flex flex-wrap items-center justify-between gap-3 text-sm">
          <label className="inline-flex items-center gap-2 text-slate-600">
            <input type="checkbox" className="h-4 w-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500" />
            Ghi nhớ đăng nhập
          </label>
          <Link href={"#"} className="text-slate-500 hover:text-slate-700">
            Quên mật khẩu?
          </Link>
        </div>

        <Button
          type="submit"
          variant="default"
          disabled={isSubmitting || !isValid}
          className="h-11 w-full rounded-xl bg-emerald-600 text-base font-semibold text-white transition duration-200 hover:bg-emerald-700"
        >
          {isSubmitting ? "Đang đăng nhập..." : "Đăng nhập"}
        </Button>
      </form>

      {errorMessage && (
        <Alert variant="destructive" className="mt-4 rounded-xl border-red-200 bg-red-50 text-red-800">
          <AlertCircleIcon className="size-4" />
          <AlertTitle>Đăng nhập thất bại</AlertTitle>
          <AlertDescription>{errorMessage}</AlertDescription>
        </Alert>
      )}

      <p className="mt-6 text-center text-sm text-slate-600">
        <Link href="/dang-ky" className="font-semibold mr-1 text-slate-600 hover:underline focus-visible:ring-2 focus-visible:ring-slate-400">
          Chưa có tài khoản?
        </Link>
        <Link href="/" className="font-semibold text-orange-600 underline-offset-4 hover:underline focus-visible:ring-2 focus-visible:ring-orange-400">
          Khám phá sản phẩm ngay
        </Link>
      </p>
    </section>
  );
}